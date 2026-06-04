using ReactiveUI;

namespace TidyTop.App.ViewModels;

public sealed class TidyTopRuntimeState : ReactiveObject
{
    private bool _isOverlayVisible = true;
    private bool _areNativeIconsHidden;
    private bool _isDraggingItem;
    private bool _isEditingSmartBox;
    private bool _isShuttingDown;
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

    public bool IsShuttingDown
    {
        get => _isShuttingDown;
        set => this.RaiseAndSetIfChanged(ref _isShuttingDown, value);
    }

    public string LastError
    {
        get => _lastError;
        set => this.RaiseAndSetIfChanged(ref _lastError, value);
    }
}
