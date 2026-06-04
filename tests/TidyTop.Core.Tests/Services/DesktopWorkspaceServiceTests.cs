using TidyTop.Core.Models;
using TidyTop.Core.Services;

namespace TidyTop.Core.Tests.Services;

public class DesktopWorkspaceServiceTests
{
    [Fact]
    public async Task MoveItemToSmartBoxAsync_RemovesItemFromPreviousBoxAndSaves()
    {
        var item = CreateItem("Tool", @"C:\Desktop\Tool.lnk", ".lnk", DesktopItemType.Shortcut);
        var source = new SmartBox { Title = "Source", Behavior = SmartBoxBehavior.Manual };
        var target = new SmartBox { Title = "Target", Behavior = SmartBoxBehavior.Manual };
        source.AssignItem(item);

        var layout = new DesktopLayout();
        layout.SmartBoxes.Add(source);
        layout.SmartBoxes.Add(target);

        var store = new MemoryLayoutStore(layout);
        var scanner = new FakeDesktopScanner(new[] { item });
        var service = new DesktopWorkspaceService(scanner, store, new LayoutReconciler());

        await service.LoadAsync();
        var workspace = await service.MoveItemToSmartBoxAsync(item.FullPath, target.Id);

        Assert.DoesNotContain(source.ItemPaths, path => path.Equals(item.NormalizedPath, StringComparison.OrdinalIgnoreCase));
        Assert.Contains(target.ItemPaths, path => path.Equals(item.NormalizedPath, StringComparison.OrdinalIgnoreCase));
        Assert.True(store.SaveCount >= 2);
        Assert.Contains(workspace.SmartBoxes.Single(box => box.SmartBox.Id == target.Id).Items, moved => moved.NormalizedPath == item.NormalizedPath);
    }

    [Fact]
    public async Task MoveItemToUnboxedAsync_PreventsRuleBasedReassignmentByPinningToCatchAll()
    {
        var item = CreateItem("Report", @"C:\Desktop\Report.pdf", ".pdf", DesktopItemType.File);
        var layout = DefaultSmartBoxFactory.CreateDefaultLayout();
        var office = layout.SmartBoxes.Single(box => box.Title == "Office & Documents");
        var catchAll = layout.SmartBoxes.Single(box => box.Behavior == SmartBoxBehavior.CatchAll);
        office.AssignItem(item);

        var store = new MemoryLayoutStore(layout);
        var scanner = new FakeDesktopScanner(new[] { item });
        var service = new DesktopWorkspaceService(scanner, store, new LayoutReconciler());

        await service.LoadAsync();
        await service.MoveItemToUnboxedAsync(item.FullPath);

        Assert.DoesNotContain(office.ItemPaths, path => path.Equals(item.NormalizedPath, StringComparison.OrdinalIgnoreCase));
        Assert.Contains(catchAll.ItemPaths, path => path.Equals(item.NormalizedPath, StringComparison.OrdinalIgnoreCase));
    }

    private static DesktopItem CreateItem(string name, string path, string extension, DesktopItemType type)
    {
        return new DesktopItem
        {
            Id = name,
            Name = name,
            FullPath = path,
            NormalizedPath = DesktopItem.NormalizePath(path),
            Extension = extension,
            Type = type
        };
    }

    private sealed class FakeDesktopScanner : IDesktopScanner
    {
        private readonly IReadOnlyList<DesktopItem> _items;

        public FakeDesktopScanner(IReadOnlyList<DesktopItem> items)
        {
            _items = items;
        }

        public Task<IReadOnlyList<DesktopItem>> ScanAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_items);
        }
    }

    private sealed class MemoryLayoutStore : ILayoutStore
    {
        private DesktopLayout? _layout;

        public MemoryLayoutStore(DesktopLayout? layout)
        {
            _layout = layout;
        }

        public int SaveCount { get; private set; }

        public Task<DesktopLayout?> LoadAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_layout);
        }

        public Task SaveAsync(DesktopLayout layout, CancellationToken cancellationToken = default)
        {
            _layout = layout;
            SaveCount++;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            _layout = null;
            return Task.CompletedTask;
        }
    }
}
