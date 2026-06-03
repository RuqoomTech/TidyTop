using TidyTop.App.ViewModels;
using TidyTop.Core.Models;
using TidyTop.Core.Services;

namespace TidyTop.App.Tests.ViewModels;

public class MainWindowViewModelTests
{
    [Fact]
    public async Task InitializeAsync_LoadsSmartBoxesAndUpdatesSummary()
    {
        var service = new FakeWorkspaceService(CreateWorkspace());
        var viewModel = new MainWindowViewModel(service);

        await viewModel.InitializeAsync();

        Assert.Equal("TidyTop", viewModel.Title);
        Assert.Single(viewModel.SmartBoxes);
        Assert.Equal(1, viewModel.TotalItemCount);
        Assert.Contains("1/1", viewModel.SummaryText);
    }

    [Fact]
    public async Task AddSmartBoxCommand_RefreshesFromService()
    {
        var service = new FakeWorkspaceService(CreateWorkspace());
        var viewModel = new MainWindowViewModel(service);

        await viewModel.InitializeAsync();
        await viewModel.AddSmartBoxCommand.ExecuteAsync();

        Assert.Equal(2, viewModel.BoxCount);
    }

    private static DesktopWorkspace CreateWorkspace(int extraBoxes = 0)
    {
        var item = new DesktopItem
        {
            Id = "item-1",
            Name = "Report",
            FullPath = @"C:\Desktop\Report.pdf",
            NormalizedPath = DesktopItem.NormalizePath(@"C:\Desktop\Report.pdf"),
            Extension = ".pdf",
            Type = DesktopItemType.File
        };

        var box = new SmartBox { Title = "Office", Emoji = "📊", Behavior = SmartBoxBehavior.Manual };
        box.AssignItem(item);

        var snapshots = new List<SmartBoxSnapshot> { new(box, new[] { item }) };
        for (var i = 0; i < extraBoxes; i++)
        {
            snapshots.Add(new SmartBoxSnapshot(new SmartBox { Title = $"Manual {i + 1}" }, Array.Empty<DesktopItem>()));
        }

        return new DesktopWorkspace(new DesktopLayout { SmartBoxes = { box } }, new[] { item }, snapshots);
    }

    private sealed class FakeWorkspaceService : IDesktopWorkspaceService
    {
        private DesktopWorkspace _workspace;

        public FakeWorkspaceService(DesktopWorkspace workspace)
        {
            _workspace = workspace;
        }

        public Task<DesktopWorkspace> LoadAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_workspace);
        }

        public Task<DesktopWorkspace> RefreshAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_workspace);
        }

        public Task<DesktopWorkspace> AddSmartBoxAsync(string title, CancellationToken cancellationToken = default)
        {
            _workspace = CreateWorkspace(extraBoxes: 1);
            return Task.FromResult(_workspace);
        }

        public Task<DesktopWorkspace> ResetLayoutAsync(CancellationToken cancellationToken = default)
        {
            _workspace = CreateWorkspace();
            return Task.FromResult(_workspace);
        }

        public Task SaveAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
