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
        var launcher = new FakeDesktopItemLauncher();
        var settingsStore = new FakeAppSettingsStore();
        var viewModel = new MainWindowViewModel(service, launcher, settingsStore);

        await viewModel.InitializeAsync();

        Assert.Equal("TidyTop", viewModel.Title);
        Assert.Single(viewModel.SmartBoxes);
        Assert.Equal(1, viewModel.TotalItemCount);
        Assert.Contains("1/1", viewModel.SummaryText);
    }

    [Fact]
    public async Task InitializeAsync_LoadsDesktopIntegrationSettings()
    {
        var service = new FakeWorkspaceService(CreateWorkspace());
        var launcher = new FakeDesktopItemLauncher();
        var settingsStore = new FakeAppSettingsStore(new AppSettings
        {
            HideNativeDesktopIcons = true,
            EnableGlobalHotkey = true,
            GlobalHotkey = "Ctrl+Alt+T"
        });
        var viewModel = new MainWindowViewModel(service, launcher, settingsStore);

        await viewModel.InitializeAsync();

        Assert.True(viewModel.HideNativeDesktopIcons);
        Assert.Equal("Managed icons", viewModel.NativeDesktopIconsModeText);
        Assert.Contains("Ctrl+Alt+T", viewModel.DesktopIntegrationText);
    }

    [Fact]
    public async Task SetHideNativeDesktopIconsPreferenceAsync_PersistsSetting()
    {
        var service = new FakeWorkspaceService(CreateWorkspace());
        var launcher = new FakeDesktopItemLauncher();
        var settingsStore = new FakeAppSettingsStore();
        var viewModel = new MainWindowViewModel(service, launcher, settingsStore);

        await viewModel.InitializeAsync();
        await viewModel.SetHideNativeDesktopIconsPreferenceAsync(true);

        Assert.True(settingsStore.LastSaved?.HideNativeDesktopIcons == true);
        Assert.Equal("Show icons", viewModel.NativeDesktopIconsButtonText);
    }

    [Fact]
    public async Task AddSmartBoxCommand_RefreshesFromService()
    {
        var service = new FakeWorkspaceService(CreateWorkspace());
        var launcher = new FakeDesktopItemLauncher();
        var settingsStore = new FakeAppSettingsStore();
        var viewModel = new MainWindowViewModel(service, launcher, settingsStore);

        await viewModel.InitializeAsync();
        await viewModel.AddSmartBoxCommand.ExecuteAsync();

        Assert.Equal(2, viewModel.BoxCount);
    }

    [Fact]
    public async Task OpenDesktopItemAsync_UsesShellLauncher()
    {
        var service = new FakeWorkspaceService(CreateWorkspace());
        var launcher = new FakeDesktopItemLauncher();
        var settingsStore = new FakeAppSettingsStore();
        var viewModel = new MainWindowViewModel(service, launcher, settingsStore);

        await viewModel.InitializeAsync();
        await viewModel.OpenDesktopItemAsync(viewModel.SmartBoxes.Single().Items.Single());

        Assert.Equal(@"C:\Desktop\Report.pdf", launcher.LastPath);
    }

    [Fact]
    public async Task MoveDesktopItemToSmartBoxAsync_RefreshesWorkspace()
    {
        var service = new FakeWorkspaceService(CreateWorkspace(extraBoxes: 1));
        var launcher = new FakeDesktopItemLauncher();
        var settingsStore = new FakeAppSettingsStore();
        var viewModel = new MainWindowViewModel(service, launcher, settingsStore);

        await viewModel.InitializeAsync();
        var item = viewModel.SmartBoxes[0].Items.Single();
        var target = viewModel.SmartBoxes[1];

        await viewModel.MoveDesktopItemToSmartBoxAsync(item, target);

        Assert.Equal(target.Id, service.LastMoveTargetId);
        Assert.Contains("Moved", viewModel.StatusMessage);
    }



    [Fact]
    public async Task SmartBoxEditor_RenamesSmartBoxThroughService()
    {
        var service = new FakeWorkspaceService(CreateWorkspace());
        var launcher = new FakeDesktopItemLauncher();
        var settingsStore = new FakeAppSettingsStore();
        var viewModel = new MainWindowViewModel(service, launcher, settingsStore);

        await viewModel.InitializeAsync();
        viewModel.OpenSmartBoxEditor(viewModel.SmartBoxes.Single());
        viewModel.EditingSmartBoxTitle = "Documents";
        await viewModel.SaveSmartBoxEditorCommand.ExecuteAsync();

        Assert.Equal("Documents", service.LastRenameTitle);
        Assert.False(viewModel.IsSmartBoxEditorOpen);
    }

    [Fact]
    public async Task DragState_HighlightsCurrentDropTargetAndClearsOnEnd()
    {
        var service = new FakeWorkspaceService(CreateWorkspace(extraBoxes: 1));
        var launcher = new FakeDesktopItemLauncher();
        var settingsStore = new FakeAppSettingsStore();
        var viewModel = new MainWindowViewModel(service, launcher, settingsStore);

        await viewModel.InitializeAsync();
        var item = viewModel.SmartBoxes[0].Items.Single();
        var target = viewModel.SmartBoxes[1];

        viewModel.BeginDesktopItemDrag(item, 100, 120);
        viewModel.UpdateDesktopItemDrag(200, 220, target);

        Assert.True(viewModel.IsDraggingItem);
        Assert.True(target.IsDropTarget);
        Assert.Contains(target.Title, viewModel.DragDropHint);

        viewModel.EndDesktopItemDrag();

        Assert.False(viewModel.IsDraggingItem);
        Assert.All(viewModel.SmartBoxes, box => Assert.False(box.IsDropTarget));
    }



    [Fact]
    public async Task SaveSettingsAsync_PersistsSettingsAndUpdatesDiagnostics()
    {
        var service = new FakeWorkspaceService(CreateWorkspace());
        var launcher = new FakeDesktopItemLauncher();
        var settingsStore = new FakeAppSettingsStore();
        var viewModel = new MainWindowViewModel(service, launcher, settingsStore);

        await viewModel.InitializeAsync();
        viewModel.StartHidden = true;
        viewModel.EnableGlobalHotkey = true;
        viewModel.EnableTrayIcon = false;
        viewModel.EnableNativeDesktopIconControl = false;
        viewModel.HideNativeDesktopIcons = true;

        var saved = await viewModel.SaveSettingsAsync();

        Assert.True(saved);
        Assert.True(settingsStore.LastSaved is { StartHidden: true });
        Assert.True(settingsStore.LastSaved is { EnableTrayIcon: false });
        Assert.True(settingsStore.LastSaved is { HideNativeDesktopIcons: false });
        Assert.Equal("Loaded", viewModel.SettingsDiagnosticText);
    }

    [Fact]
    public async Task SaveSettingsAsync_BlocksUnsafeStartHiddenWithoutRecoveryControls()
    {
        var service = new FakeWorkspaceService(CreateWorkspace());
        var launcher = new FakeDesktopItemLauncher();
        var settingsStore = new FakeAppSettingsStore();
        var viewModel = new MainWindowViewModel(service, launcher, settingsStore);

        await viewModel.InitializeAsync();
        viewModel.StartHidden = true;
        viewModel.EnableTrayIcon = false;
        viewModel.EnableGlobalHotkey = false;

        var saved = await viewModel.SaveSettingsAsync();

        Assert.False(saved);
        Assert.Contains("Safety guard", viewModel.LastErrorDiagnosticText);
    }

    [Fact]
    public async Task OpenSettingsPanel_UpdatesRuntimeState()
    {
        var service = new FakeWorkspaceService(CreateWorkspace());
        var launcher = new FakeDesktopItemLauncher();
        var settingsStore = new FakeAppSettingsStore();
        var viewModel = new MainWindowViewModel(service, launcher, settingsStore);

        await viewModel.InitializeAsync();
        viewModel.OpenSettingsPanel();

        Assert.True(viewModel.IsSettingsPanelOpen);
        Assert.True(viewModel.RuntimeState.IsSettingsPanelOpen);
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

        var layout = new DesktopLayout();
        layout.SmartBoxes.Add(box);

        var snapshots = new List<SmartBoxSnapshot> { new(box, new[] { item }) };
        for (var i = 0; i < extraBoxes; i++)
        {
            var extraBox = new SmartBox { Title = $"Manual {i + 1}", X = 400 + (i * 30), Y = 24 + (i * 30) };
            layout.SmartBoxes.Add(extraBox);
            snapshots.Add(new SmartBoxSnapshot(extraBox, Array.Empty<DesktopItem>()));
        }

        return new DesktopWorkspace(layout, new[] { item }, snapshots);
    }

    private sealed class FakeWorkspaceService : IDesktopWorkspaceService
    {
        private DesktopWorkspace _workspace;

        public FakeWorkspaceService(DesktopWorkspace workspace)
        {
            _workspace = workspace;
        }

        public Guid? LastMoveTargetId { get; private set; }
        public string? LastRenameTitle { get; private set; }
        public Guid? LastDeleteId { get; private set; }

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

        public Task<DesktopWorkspace> RenameSmartBoxAsync(Guid smartBoxId, string title, CancellationToken cancellationToken = default)
        {
            LastRenameTitle = title;
            var box = _workspace.Layout.FindBox(smartBoxId);
            if (box is not null)
            {
                box.Title = title;
            }

            return Task.FromResult(_workspace);
        }

        public Task<DesktopWorkspace> DeleteSmartBoxAsync(Guid smartBoxId, CancellationToken cancellationToken = default)
        {
            LastDeleteId = smartBoxId;
            var box = _workspace.Layout.FindBox(smartBoxId);
            if (box is not null)
            {
                _workspace.Layout.SmartBoxes.Remove(box);
            }

            return Task.FromResult(_workspace);
        }

        public Task<DesktopWorkspace> ResetLayoutAsync(CancellationToken cancellationToken = default)
        {
            _workspace = CreateWorkspace();
            return Task.FromResult(_workspace);
        }


        public Task<DesktopWorkspace> AutoArrangeAsync(int surfaceWidth, int surfaceHeight, CancellationToken cancellationToken = default)
        {
            foreach (var box in _workspace.Layout.SmartBoxes)
            {
                box.SetGeometry(28, 86, 320, 220);
            }

            return Task.FromResult(_workspace);
        }

        public Task UpdateSmartBoxGeometryAsync(Guid smartBoxId, int x, int y, int width, int height, CancellationToken cancellationToken = default)
        {
            var box = _workspace.Layout.FindBox(smartBoxId);
            box?.SetGeometry(x, y, width, height);
            return Task.CompletedTask;
        }

        public Task<DesktopWorkspace> MoveItemToSmartBoxAsync(string itemPath, Guid targetSmartBoxId, CancellationToken cancellationToken = default)
        {
            LastMoveTargetId = targetSmartBoxId;
            return Task.FromResult(_workspace);
        }

        public Task<DesktopWorkspace> MoveItemToUnboxedAsync(string itemPath, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_workspace);
        }

        public Task SaveAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeAppSettingsStore : IAppSettingsStore
    {
        private AppSettings _settings;

        public FakeAppSettingsStore(AppSettings? settings = null)
        {
            _settings = settings ?? new AppSettings();
        }

        public AppSettings? LastSaved { get; private set; }

        public Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_settings);
        }

        public Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
        {
            LastSaved = settings;
            _settings = settings;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeDesktopItemLauncher : IDesktopItemLauncher
    {
        public string? LastPath { get; private set; }

        public Task LaunchAsync(DesktopItem item, CancellationToken cancellationToken = default)
        {
            LastPath = item.FullPath;
            return Task.CompletedTask;
        }

        public Task LaunchAsync(string path, CancellationToken cancellationToken = default)
        {
            LastPath = path;
            return Task.CompletedTask;
        }
    }
}
