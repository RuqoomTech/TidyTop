using ReactiveUI;

namespace TidyTop.App.ViewModels;

public sealed class TidyTopRuntimeState : ReactiveObject
{
    private bool _isOverlayVisible = true;
    private bool _areNativeIconsHidden;
    private bool _isDraggingItem;
    private bool _isEditingSmartBox;
    private bool _isSettingsPanelOpen;
    private bool _isShuttingDown;
    private bool _isDesktopOverlayAttached;
    private bool _isTrayActive;
    private bool _isGlobalHotkeyRegistered;
    private bool _isLayoutLoaded;
    private bool _areSettingsLoaded;
    private string _lastError = string.Empty;

    public bool IsOverlayVisible
    {
        get => _isOverlayVisible;
        set => this.RaiseAndSetIfChanged(ref _isOverlayVisible, value);
    }

    public bool AreNativeIconsHidden
    {
        get => _areNativeIconsHidden;
        set => this.RaiseAndSetIfChanged(ref _areNativeIconsHidden, value);
    }

    public bool IsDraggingItem
    {
        get => _isDraggingItem;
        set => this.RaiseAndSetIfChanged(ref _isDraggingItem, value);
    }

    public bool IsEditingSmartBox
    {
        get => _isEditingSmartBox;
        set => this.RaiseAndSetIfChanged(ref _isEditingSmartBox, value);
    }

    public bool IsSettingsPanelOpen
    {
        get => _isSettingsPanelOpen;
        set => this.RaiseAndSetIfChanged(ref _isSettingsPanelOpen, value);
    }

    public bool IsShuttingDown
    {
        get => _isShuttingDown;
        set => this.RaiseAndSetIfChanged(ref _isShuttingDown, value);
    }

    public bool IsDesktopOverlayAttached
    {
        get => _isDesktopOverlayAttached;
        set => this.RaiseAndSetIfChanged(ref _isDesktopOverlayAttached, value);
    }

    public bool IsTrayActive
    {
        get => _isTrayActive;
        set => this.RaiseAndSetIfChanged(ref _isTrayActive, value);
    }

    public bool IsGlobalHotkeyRegistered
    {
        get => _isGlobalHotkeyRegistered;
        set => this.RaiseAndSetIfChanged(ref _isGlobalHotkeyRegistered, value);
    }

    public bool IsLayoutLoaded
    {
        get => _isLayoutLoaded;
        set => this.RaiseAndSetIfChanged(ref _isLayoutLoaded, value);
    }

    public bool AreSettingsLoaded
    {
        get => _areSettingsLoaded;
        set => this.RaiseAndSetIfChanged(ref _areSettingsLoaded, value);
    }

    public string LastError
    {
        get => _lastError;
        set => this.RaiseAndSetIfChanged(ref _lastError, value);
    }
}
