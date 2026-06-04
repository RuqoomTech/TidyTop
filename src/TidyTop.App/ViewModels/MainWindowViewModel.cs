using System.Collections.ObjectModel;
using ReactiveUI;
using TidyTop.App.Commands;
using TidyTop.Core.Models;
using TidyTop.Core.Services;

namespace TidyTop.App.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly IDesktopWorkspaceService _workspaceService;
    private readonly IDesktopItemLauncher _desktopItemLauncher;
    private readonly IAppSettingsStore _settingsStore;
    private bool _hasLoaded;
    private AppSettings _settings = new();
    private bool _isOverlayVisible = true;
    private bool _startHidden;
    private bool _hideNativeDesktopIcons;
    private bool _enableDesktopOverlayHost = true;
    private bool _enableNativeDesktopIconControl = true;
    private bool _enableTrayIcon = true;
    private bool _enableGlobalHotkey = true;
    private bool _enableDragDrop = true;
    private bool _runOnStartup;
    private bool _enableAutoOrganizeOnRefresh = true;
    private string _globalHotkey = "Ctrl+Alt+T";
    private int _totalItemCount;
    private int _organizedItemCount;
    private int _boxCount;
    private bool _isDraggingItem;
    private string _dragGhostText = string.Empty;
    private string _dragDropHint = string.Empty;
    private int _dragGhostX;
    private int _dragGhostY;
    private bool _isSmartBoxEditorOpen;
    private bool _isSettingsPanelOpen;
    private Guid? _editingSmartBoxId;
    private string _editingSmartBoxTitle = string.Empty;
    private string _editingSmartBoxSubtitle = string.Empty;
    private bool _editingSmartBoxCanDelete;

    public MainWindowViewModel(
        IDesktopWorkspaceService workspaceService,
        IDesktopItemLauncher desktopItemLauncher,
        IAppSettingsStore settingsStore)
    {
        _workspaceService = workspaceService;
        _desktopItemLauncher = desktopItemLauncher;
        _settingsStore = settingsStore;
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        AddSmartBoxCommand = new AsyncRelayCommand(AddSmartBoxAsync);
        ResetLayoutCommand = new AsyncRelayCommand(ResetLayoutAsync);
        SaveLayoutCommand = new AsyncRelayCommand(SaveLayoutAsync);
        SaveSettingsCommand = new AsyncRelayCommand(async () => { await SaveSettingsAsync(); });
        ResetSettingsCommand = new AsyncRelayCommand(ResetSettingsAsync);
        ResetEverythingCommand = new AsyncRelayCommand(ResetEverythingAsync);
        OpenSettingsCommand = new AsyncRelayCommand(() =>
        {
            OpenSettingsPanel();
            return Task.CompletedTask;
        });
        CloseSettingsCommand = new AsyncRelayCommand(() =>
        {
            CloseSettingsPanel();
            return Task.CompletedTask;
        });
        SaveSmartBoxEditorCommand = new AsyncRelayCommand(SaveSmartBoxEditorAsync);
        DeleteSmartBoxCommand = new AsyncRelayCommand(DeleteEditingSmartBoxAsync, () => EditingSmartBoxCanDelete);
        CancelSmartBoxEditorCommand = new AsyncRelayCommand(() =>
        {
            CloseSmartBoxEditor();
            return Task.CompletedTask;
        });
    }

    public string Title => "TidyTop";
    public string Subtitle => "Windows-first desktop organizer MVP";
    public ObservableCollection<SmartBoxViewModel> SmartBoxes { get; } = new();
    public TidyTopRuntimeState RuntimeState { get; } = new();

    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand AddSmartBoxCommand { get; }
    public AsyncRelayCommand ResetLayoutCommand { get; }
    public AsyncRelayCommand SaveLayoutCommand { get; }
    public AsyncRelayCommand SaveSettingsCommand { get; }
    public AsyncRelayCommand ResetSettingsCommand { get; }
    public AsyncRelayCommand ResetEverythingCommand { get; }
    public AsyncRelayCommand OpenSettingsCommand { get; }
    public AsyncRelayCommand CloseSettingsCommand { get; }
    public AsyncRelayCommand SaveSmartBoxEditorCommand { get; }
    public AsyncRelayCommand DeleteSmartBoxCommand { get; }
    public AsyncRelayCommand CancelSmartBoxEditorCommand { get; }

    public int TotalItemCount
    {
        get => _totalItemCount;
        private set => this.RaiseAndSetIfChanged(ref _totalItemCount, value);
    }

    public int OrganizedItemCount
    {
        get => _organizedItemCount;
        private set => this.RaiseAndSetIfChanged(ref _organizedItemCount, value);
    }

    public int BoxCount
    {
        get => _boxCount;
        private set => this.RaiseAndSetIfChanged(ref _boxCount, value);
    }

    public bool IsOverlayVisible
    {
        get => _isOverlayVisible;
        private set
        {
            this.RaiseAndSetIfChanged(ref _isOverlayVisible, value);
            RuntimeState.IsOverlayVisible = value;
            RaiseDesktopIntegrationPropertiesChanged();
        }
    }

    public bool StartHidden
    {
        get => _startHidden;
        set
        {
            this.RaiseAndSetIfChanged(ref _startHidden, value);
            this.RaisePropertyChanged(nameof(StartHiddenStatusText));
        }
    }

    public bool HideNativeDesktopIcons
    {
        get => _hideNativeDesktopIcons;
        set
        {
            this.RaiseAndSetIfChanged(ref _hideNativeDesktopIcons, EnableNativeDesktopIconControl && value);
            RuntimeState.AreNativeIconsHidden = _hideNativeDesktopIcons;
            RaiseDesktopIntegrationPropertiesChanged();
        }
    }

    public bool EnableDesktopOverlayHost
    {
        get => _enableDesktopOverlayHost;
        set
        {
            this.RaiseAndSetIfChanged(ref _enableDesktopOverlayHost, value);
            RaiseDesktopIntegrationPropertiesChanged();
        }
    }

    public bool EnableNativeDesktopIconControl
    {
        get => _enableNativeDesktopIconControl;
        set
        {
            this.RaiseAndSetIfChanged(ref _enableNativeDesktopIconControl, value);
            if (!value)
            {
                HideNativeDesktopIcons = false;
            }

            RaiseDesktopIntegrationPropertiesChanged();
        }
    }

    public bool EnableTrayIcon
    {
        get => _enableTrayIcon;
        set
        {
            this.RaiseAndSetIfChanged(ref _enableTrayIcon, value);
            RaiseDesktopIntegrationPropertiesChanged();
        }
    }

    public bool EnableGlobalHotkey
    {
        get => _enableGlobalHotkey;
        set
        {
            this.RaiseAndSetIfChanged(ref _enableGlobalHotkey, value);
            RaiseDesktopIntegrationPropertiesChanged();
        }
    }

    public bool EnableDragDrop
    {
        get => _enableDragDrop;
        set => this.RaiseAndSetIfChanged(ref _enableDragDrop, value);
    }

    public bool RunOnStartup
    {
        get => _runOnStartup;
        set => this.RaiseAndSetIfChanged(ref _runOnStartup, value);
    }

    public bool EnableAutoOrganizeOnRefresh
    {
        get => _enableAutoOrganizeOnRefresh;
        set => this.RaiseAndSetIfChanged(ref _enableAutoOrganizeOnRefresh, value);
    }

    public string GlobalHotkey
    {
        get => _globalHotkey;
        set
        {
            this.RaiseAndSetIfChanged(ref _globalHotkey, string.IsNullOrWhiteSpace(value) ? "Ctrl+Alt+T" : value.Trim());
            RaiseDesktopIntegrationPropertiesChanged();
        }
    }

    public bool IsSettingsPanelOpen
    {
        get => _isSettingsPanelOpen;
        private set
        {
            this.RaiseAndSetIfChanged(ref _isSettingsPanelOpen, value);
            RuntimeState.IsSettingsPanelOpen = value;
        }
    }

    public string OverlayVisibilityText => IsOverlayVisible ? "Hide" : "Show";
    public string NativeDesktopIconsButtonText => HideNativeDesktopIcons ? "Show icons" : "Hide icons";
    public string NativeDesktopIconsModeText => !EnableNativeDesktopIconControl
        ? "Icon control off"
        : HideNativeDesktopIcons ? "Managed icons" : "Safe mode";
    public string HotkeyStatusText => EnableGlobalHotkey ? GlobalHotkey : "Hotkey off";
    public string DesktopIntegrationText => $"{NativeDesktopIconsModeText} • {HotkeyStatusText}";
    public string StartHiddenStatusText => StartHidden ? "Starts hidden" : "Starts visible";

    public string OverlayDiagnosticText => RuntimeState.IsOverlayVisible ? "Visible" : "Hidden";
    public string OverlayHostDiagnosticText => RuntimeState.IsDesktopOverlayAttached ? "Attached to desktop host" : "Normal overlay window";
    public string NativeIconsDiagnosticText => RuntimeState.AreNativeIconsHidden ? "Hidden by TidyTop" : "Visible / safe";
    public string HotkeyDiagnosticText => RuntimeState.IsGlobalHotkeyRegistered ? $"Registered ({GlobalHotkey})" : "Not registered";
    public string TrayDiagnosticText => RuntimeState.IsTrayActive ? "Active" : "Inactive";
    public string LayoutDiagnosticText => RuntimeState.IsLayoutLoaded ? "Loaded" : "Not loaded";
    public string SettingsDiagnosticText => RuntimeState.AreSettingsLoaded ? "Loaded" : "Not loaded";
    public string LastErrorDiagnosticText => string.IsNullOrWhiteSpace(RuntimeState.LastError) ? "None" : RuntimeState.LastError;

    public bool IsDraggingItem
    {
        get => _isDraggingItem;
        private set
        {
            this.RaiseAndSetIfChanged(ref _isDraggingItem, value);
            RuntimeState.IsDraggingItem = value;
        }
    }

    public string DragGhostText
    {
        get => _dragGhostText;
        private set => this.RaiseAndSetIfChanged(ref _dragGhostText, value);
    }

    public string DragDropHint
    {
        get => _dragDropHint;
        private set => this.RaiseAndSetIfChanged(ref _dragDropHint, value);
    }

    public int DragGhostX
    {
        get => _dragGhostX;
        private set => this.RaiseAndSetIfChanged(ref _dragGhostX, value);
    }

    public int DragGhostY
    {
        get => _dragGhostY;
        private set => this.RaiseAndSetIfChanged(ref _dragGhostY, value);
    }

    public bool IsSmartBoxEditorOpen
    {
        get => _isSmartBoxEditorOpen;
        private set
        {
            this.RaiseAndSetIfChanged(ref _isSmartBoxEditorOpen, value);
            RuntimeState.IsEditingSmartBox = value;
        }
    }

    public string EditingSmartBoxTitle
    {
        get => _editingSmartBoxTitle;
        set => this.RaiseAndSetIfChanged(ref _editingSmartBoxTitle, value);
    }

    public string EditingSmartBoxSubtitle
    {
        get => _editingSmartBoxSubtitle;
        private set => this.RaiseAndSetIfChanged(ref _editingSmartBoxSubtitle, value);
    }

    public bool EditingSmartBoxCanDelete
    {
        get => _editingSmartBoxCanDelete;
        private set
        {
            this.RaiseAndSetIfChanged(ref _editingSmartBoxCanDelete, value);
            DeleteSmartBoxCommand.RaiseCanExecuteChanged();
        }
    }

    public string SummaryText => $"{OrganizedItemCount}/{TotalItemCount} items organized across {BoxCount} boxes";

    public async Task InitializeAsync()
    {
        if (_hasLoaded)
        {
            return;
        }

        _hasLoaded = true;
        await LoadSettingsAsync();
        await LoadAsync();
    }

    private async Task LoadSettingsAsync()
    {
        try
        {
            _settings = await _settingsStore.LoadAsync();
            ApplySettingsToViewModel(_settings);
            RuntimeState.AreSettingsLoaded = true;
            RaiseDiagnosticsChanged();
        }
        catch (Exception ex)
        {
            FailWithRuntimeError($"Could not load settings: {ex.Message}");
        }
    }

    private void ApplySettingsToViewModel(AppSettings settings)
    {
        StartHidden = settings.StartHidden;
        EnableDesktopOverlayHost = settings.EnableDesktopOverlayHost;
        EnableNativeDesktopIconControl = settings.EnableNativeDesktopIconControl;
        EnableTrayIcon = settings.EnableTrayIcon;
        HideNativeDesktopIcons = EnableNativeDesktopIconControl && settings.HideNativeDesktopIcons;
        EnableGlobalHotkey = settings.EnableGlobalHotkey;
        EnableDragDrop = settings.EnableDragDrop;
        RunOnStartup = settings.RunOnStartup;
        EnableAutoOrganizeOnRefresh = settings.EnableAutoOrganizeOnRefresh;
        GlobalHotkey = string.IsNullOrWhiteSpace(settings.GlobalHotkey) ? "Ctrl+Alt+T" : settings.GlobalHotkey;
    }

    private void ApplyViewModelToSettings(AppSettings settings)
    {
        settings.StartHidden = StartHidden;
        settings.EnableDesktopOverlayHost = EnableDesktopOverlayHost;
        settings.EnableNativeDesktopIconControl = EnableNativeDesktopIconControl;
        settings.HideNativeDesktopIcons = EnableNativeDesktopIconControl && HideNativeDesktopIcons;
        settings.EnableTrayIcon = EnableTrayIcon;
        settings.EnableGlobalHotkey = EnableGlobalHotkey;
        settings.GlobalHotkey = string.IsNullOrWhiteSpace(GlobalHotkey) ? "Ctrl+Alt+T" : GlobalHotkey.Trim();
        settings.EnableDragDrop = EnableDragDrop;
        settings.RunOnStartup = RunOnStartup;
        settings.EnableAutoOrganizeOnRefresh = EnableAutoOrganizeOnRefresh;
    }

    public bool ShouldStartHidden => _settings.StartHidden;

    public void SetOverlayVisible(bool isVisible)
    {
        IsOverlayVisible = isVisible;
        RaiseDiagnosticsChanged();
    }

    public void SetShuttingDown(bool isShuttingDown)
    {
        RuntimeState.IsShuttingDown = isShuttingDown;
        RaiseDiagnosticsChanged();
    }

    public void SetDesktopOverlayAttached(bool isAttached)
    {
        RuntimeState.IsDesktopOverlayAttached = isAttached;
        RaiseDiagnosticsChanged();
    }

    public void SetTrayActive(bool isActive)
    {
        RuntimeState.IsTrayActive = isActive;
        RaiseDiagnosticsChanged();
    }

    public void SetGlobalHotkeyRegistered(bool isRegistered)
    {
        RuntimeState.IsGlobalHotkeyRegistered = isRegistered;
        RaiseDiagnosticsChanged();
    }

    public void RecordRuntimeError(string message)
    {
        RuntimeState.LastError = message;
        RaiseDiagnosticsChanged();
    }

    public async Task SetHideNativeDesktopIconsPreferenceAsync(bool hideNativeDesktopIcons)
    {
        if (hideNativeDesktopIcons && !EnableNativeDesktopIconControl)
        {
            HideNativeDesktopIcons = false;
            _settings.HideNativeDesktopIcons = false;
            await _settingsStore.SaveAsync(_settings);
            StatusMessage = "Native desktop icon control is disabled in settings. Windows icons were left visible.";
            return;
        }

        HideNativeDesktopIcons = hideNativeDesktopIcons;
        _settings.HideNativeDesktopIcons = hideNativeDesktopIcons;
        await _settingsStore.SaveAsync(_settings);
        StatusMessage = hideNativeDesktopIcons
            ? "Managed mode enabled. Native desktop icons are hidden while TidyTop runs."
            : "Safe mode enabled. Native desktop icons are visible.";
    }

    public async Task ForceRestoreNativeDesktopIconsPreferenceAsync()
    {
        HideNativeDesktopIcons = false;
        _settings.HideNativeDesktopIcons = false;
        await _settingsStore.SaveAsync(_settings);
        StatusMessage = "Windows desktop icons restored. TidyTop will not hide them on next launch.";
    }

    public async Task ToggleHideNativeDesktopIconsPreferenceAsync()
    {
        await SetHideNativeDesktopIconsPreferenceAsync(!HideNativeDesktopIcons);
    }

    public void OpenSettingsPanel()
    {
        IsSettingsPanelOpen = true;
        StatusMessage = "Settings and diagnostics opened.";
    }

    public void CloseSettingsPanel()
    {
        IsSettingsPanelOpen = false;
    }

    public async Task<bool> SaveSettingsAsync()
    {
        try
        {
            if (!EnableNativeDesktopIconControl)
            {
                HideNativeDesktopIcons = false;
            }

            if (StartHidden && !EnableTrayIcon && !EnableGlobalHotkey)
            {
                FailWithRuntimeError("Safety guard: keep tray icon or global hotkey enabled when Start hidden is on.");
                return false;
            }

            ApplyViewModelToSettings(_settings);
            await _settingsStore.SaveAsync(_settings);
            StatusMessage = "Settings saved. Runtime controls were refreshed.";
            RuntimeState.AreSettingsLoaded = true;
            RaiseDesktopIntegrationPropertiesChanged();
            RaiseDiagnosticsChanged();
            return true;
        }
        catch (Exception ex)
        {
            FailWithRuntimeError($"Could not save settings: {ex.Message}");
            return false;
        }
    }

    private async Task ResetSettingsAsync()
    {
        try
        {
            BeginBusy("Resetting settings...");
            _settings = new AppSettings();
            ApplySettingsToViewModel(_settings);
            await _settingsStore.SaveAsync(_settings);
            EndBusy("Settings reset to safe defaults.");
            RaiseDesktopIntegrationPropertiesChanged();
            RaiseDiagnosticsChanged();
        }
        catch (Exception ex)
        {
            FailWithRuntimeError($"Could not reset settings: {ex.Message}");
        }
    }

    private async Task ResetEverythingAsync()
    {
        try
        {
            BeginBusy("Resetting TidyTop data...");
            _settings = new AppSettings();
            ApplySettingsToViewModel(_settings);
            await _settingsStore.SaveAsync(_settings);
            var workspace = await _workspaceService.ResetLayoutAsync();
            ApplyWorkspace(workspace);
            EndBusy("TidyTop settings and layout were reset safely.");
            RaiseDesktopIntegrationPropertiesChanged();
            RaiseDiagnosticsChanged();
        }
        catch (Exception ex)
        {
            FailWithRuntimeError($"Could not reset TidyTop data: {ex.Message}");
        }
    }

    private async Task LoadAsync()
    {
        await RunWorkspaceOperationAsync("Loading desktop layout...", () => _workspaceService.LoadAsync());
    }

    private async Task RefreshAsync()
    {
        await RunWorkspaceOperationAsync("Refreshing desktop...", () => _workspaceService.RefreshAsync());
    }

    private async Task AddSmartBoxAsync()
    {
        await RunWorkspaceOperationAsync("Adding SmartBox...", () => _workspaceService.AddSmartBoxAsync(""));
    }

    private async Task ResetLayoutAsync()
    {
        await RunWorkspaceOperationAsync("Resetting layout...", () => _workspaceService.ResetLayoutAsync());
    }

    private async Task SaveLayoutAsync()
    {
        try
        {
            BeginBusy("Saving layout...");
            await _workspaceService.SaveAsync();
            EndBusy("Layout saved.");
            RuntimeState.IsLayoutLoaded = true;
            RaiseDiagnosticsChanged();
        }
        catch (Exception ex)
        {
            FailWithRuntimeError($"Could not save layout: {ex.Message}");
        }
    }

    public async Task OpenDesktopItemAsync(DesktopItemViewModel desktopItem)
    {
        ArgumentNullException.ThrowIfNull(desktopItem);

        try
        {
            StatusMessage = $"Opening {desktopItem.Name}...";
            await _desktopItemLauncher.LaunchAsync(desktopItem.Item);
            StatusMessage = $"Opened {desktopItem.Name}.";
        }
        catch (Exception ex)
        {
            FailWithRuntimeError($"Could not open {desktopItem.Name}: {ex.Message}");
        }
    }

    public async Task MoveDesktopItemToSmartBoxAsync(DesktopItemViewModel desktopItem, SmartBoxViewModel targetSmartBox)
    {
        ArgumentNullException.ThrowIfNull(desktopItem);
        ArgumentNullException.ThrowIfNull(targetSmartBox);

        if (desktopItem.SmartBoxId == targetSmartBox.Id)
        {
            StatusMessage = $"{desktopItem.Name} is already in {targetSmartBox.Title}.";
            return;
        }

        try
        {
            BeginBusy($"Moving {desktopItem.Name} to {targetSmartBox.Title}...");
            var workspace = await _workspaceService.MoveItemToSmartBoxAsync(desktopItem.FullPath, targetSmartBox.Id);
            ApplyWorkspace(workspace);
            EndBusy($"Moved {desktopItem.Name} to {targetSmartBox.Title}.");
        }
        catch (Exception ex)
        {
            FailWithRuntimeError($"Could not move {desktopItem.Name}: {ex.Message}");
        }
    }

    public async Task MoveDesktopItemToUnboxedAsync(DesktopItemViewModel desktopItem)
    {
        ArgumentNullException.ThrowIfNull(desktopItem);

        try
        {
            BeginBusy($"Moving {desktopItem.Name} to Other / Unboxed...");
            var workspace = await _workspaceService.MoveItemToUnboxedAsync(desktopItem.FullPath);
            ApplyWorkspace(workspace);
            EndBusy($"Moved {desktopItem.Name} to Other / Unboxed.");
        }
        catch (Exception ex)
        {
            FailWithRuntimeError($"Could not move {desktopItem.Name}: {ex.Message}");
        }
    }

    public IReadOnlyList<SmartBoxViewModel> GetMoveTargets(DesktopItemViewModel desktopItem)
    {
        ArgumentNullException.ThrowIfNull(desktopItem);

        return SmartBoxes
            .Where(box => box.IsVisible && box.Id != desktopItem.SmartBoxId)
            .OrderBy(box => box.Title, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public SmartBoxViewModel? FindSmartBoxAt(double x, double y)
    {
        return SmartBoxes
            .Where(box => box.IsVisible)
            .Where(box => x >= box.X && x <= box.X + box.Width && y >= box.Y && y <= box.Y + box.Height)
            .OrderByDescending(box => box.X + box.Y)
            .FirstOrDefault();
    }

    public void BeginDesktopItemDrag(DesktopItemViewModel desktopItem, int x, int y)
    {
        ArgumentNullException.ThrowIfNull(desktopItem);

        IsDraggingItem = true;
        DragGhostText = desktopItem.Name;
        DragDropHint = "Drop onto a SmartBox";
        UpdateDesktopItemDrag(x, y, null);
    }

    public void UpdateDesktopItemDrag(int x, int y, SmartBoxViewModel? targetSmartBox)
    {
        DragGhostX = Math.Max(0, x + 14);
        DragGhostY = Math.Max(0, y + 14);

        foreach (var smartBox in SmartBoxes)
        {
            smartBox.SetDropTarget(targetSmartBox is not null && smartBox.Id == targetSmartBox.Id);
        }

        DragDropHint = targetSmartBox is null
            ? "Drop onto a SmartBox"
            : $"Move to {targetSmartBox.Title}";
    }

    public void EndDesktopItemDrag()
    {
        foreach (var smartBox in SmartBoxes)
        {
            smartBox.SetDropTarget(false);
        }

        IsDraggingItem = false;
        DragGhostText = string.Empty;
        DragDropHint = string.Empty;
    }

    public async Task AutoArrangeAsync(int surfaceWidth, int surfaceHeight)
    {
        try
        {
            BeginBusy("Tidying desktop layout...");
            var workspace = await _workspaceService.AutoArrangeAsync(surfaceWidth, surfaceHeight);
            ApplyWorkspace(workspace);
            EndBusy("Layout tidied. Drag boxes to fine-tune.");
        }
        catch (Exception ex)
        {
            FailWithRuntimeError($"Could not auto-arrange layout: {ex.Message}");
        }
    }

    public async Task CommitSmartBoxGeometryAsync(SmartBoxViewModel smartBox)
    {
        ArgumentNullException.ThrowIfNull(smartBox);

        try
        {
            await _workspaceService.UpdateSmartBoxGeometryAsync(
                smartBox.Id,
                smartBox.X,
                smartBox.Y,
                smartBox.Width,
                smartBox.Height);

            StatusMessage = $"Saved position for {smartBox.Title}.";
        }
        catch (Exception ex)
        {
            FailWithRuntimeError($"Could not save SmartBox position: {ex.Message}");
        }
    }

    public void OpenSmartBoxEditor(SmartBoxViewModel smartBox)
    {
        ArgumentNullException.ThrowIfNull(smartBox);

        _editingSmartBoxId = smartBox.Id;
        EditingSmartBoxTitle = smartBox.Title;
        EditingSmartBoxSubtitle = smartBox.IsSystemBox
            ? "Default SmartBox • can be renamed, but not deleted yet"
            : "Manual SmartBox • can be renamed or deleted";
        EditingSmartBoxCanDelete = smartBox.CanDelete;
        IsSmartBoxEditorOpen = true;
        StatusMessage = $"Editing {smartBox.Title}.";
    }

    public void CloseSmartBoxEditor()
    {
        IsSmartBoxEditorOpen = false;
        _editingSmartBoxId = null;
        EditingSmartBoxTitle = string.Empty;
        EditingSmartBoxSubtitle = string.Empty;
        EditingSmartBoxCanDelete = false;
    }

    private async Task SaveSmartBoxEditorAsync()
    {
        if (_editingSmartBoxId is null)
        {
            CloseSmartBoxEditor();
            return;
        }

        try
        {
            BeginBusy("Saving SmartBox...");
            var workspace = await _workspaceService.RenameSmartBoxAsync(_editingSmartBoxId.Value, EditingSmartBoxTitle);
            CloseSmartBoxEditor();
            ApplyWorkspace(workspace);
            EndBusy("SmartBox saved.");
        }
        catch (Exception ex)
        {
            FailWithRuntimeError($"Could not save SmartBox: {ex.Message}");
        }
    }

    private async Task DeleteEditingSmartBoxAsync()
    {
        if (_editingSmartBoxId is null || !EditingSmartBoxCanDelete)
        {
            return;
        }

        try
        {
            BeginBusy("Deleting SmartBox...");
            var workspace = await _workspaceService.DeleteSmartBoxAsync(_editingSmartBoxId.Value);
            CloseSmartBoxEditor();
            ApplyWorkspace(workspace);
            EndBusy("SmartBox deleted. Its items were returned to the layout safely.");
        }
        catch (Exception ex)
        {
            FailWithRuntimeError($"Could not delete SmartBox: {ex.Message}");
        }
    }

    private async Task RunWorkspaceOperationAsync(string busyMessage, Func<Task<DesktopWorkspace>> operation)
    {
        try
        {
            BeginBusy(busyMessage);
            var workspace = await operation();
            ApplyWorkspace(workspace);
            EndBusy($"Loaded {workspace.TotalItemCount} desktop items into {workspace.BoxCount} SmartBoxes.");
        }
        catch (Exception ex)
        {
            FailWithRuntimeError($"TidyTop could not update the workspace: {ex.Message}");
        }
    }

    private void FailWithRuntimeError(string message)
    {
        RuntimeState.LastError = message;
        Fail(message);
        RaiseDiagnosticsChanged();
    }

    private void RaiseDesktopIntegrationPropertiesChanged()
    {
        this.RaisePropertyChanged(nameof(OverlayVisibilityText));
        this.RaisePropertyChanged(nameof(NativeDesktopIconsButtonText));
        this.RaisePropertyChanged(nameof(NativeDesktopIconsModeText));
        this.RaisePropertyChanged(nameof(HotkeyStatusText));
        this.RaisePropertyChanged(nameof(DesktopIntegrationText));
        this.RaisePropertyChanged(nameof(EnableDesktopOverlayHost));
        this.RaisePropertyChanged(nameof(EnableNativeDesktopIconControl));
        this.RaisePropertyChanged(nameof(EnableTrayIcon));
        RaiseDiagnosticsChanged();
    }

    private void RaiseDiagnosticsChanged()
    {
        this.RaisePropertyChanged(nameof(OverlayDiagnosticText));
        this.RaisePropertyChanged(nameof(OverlayHostDiagnosticText));
        this.RaisePropertyChanged(nameof(NativeIconsDiagnosticText));
        this.RaisePropertyChanged(nameof(HotkeyDiagnosticText));
        this.RaisePropertyChanged(nameof(TrayDiagnosticText));
        this.RaisePropertyChanged(nameof(LayoutDiagnosticText));
        this.RaisePropertyChanged(nameof(SettingsDiagnosticText));
        this.RaisePropertyChanged(nameof(LastErrorDiagnosticText));
    }

    private void ApplyWorkspace(DesktopWorkspace workspace)
    {
        SmartBoxes.Clear();
        foreach (var smartBox in workspace.SmartBoxes)
        {
            SmartBoxes.Add(new SmartBoxViewModel(smartBox));
        }

        TotalItemCount = workspace.TotalItemCount;
        OrganizedItemCount = workspace.OrganizedItemCount;
        BoxCount = workspace.BoxCount;
        RuntimeState.IsLayoutLoaded = true;
        this.RaisePropertyChanged(nameof(SummaryText));
        RaiseDiagnosticsChanged();
    }
}
