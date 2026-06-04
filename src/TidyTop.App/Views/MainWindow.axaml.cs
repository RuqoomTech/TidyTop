using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.Threading;
using TidyTop.App.Services;
using TidyTop.App.ViewModels;
using TidyTop.Core.Services;

namespace TidyTop.App.Views;

public partial class MainWindow : Window
{
    private const double ItemDragThreshold = 8;

    private readonly IDesktopOverlayHost _desktopOverlayHost;
    private readonly INativeDesktopIconService _nativeDesktopIconService;
    private readonly IGlobalHotkeyService _globalHotkeyService;
    private readonly IAppLogger _logger;
    private bool _initialized;
    private bool _isClosingFromTray;
    private bool _hasHiddenNativeIconsDuringSession;
    private bool _forceKeepNativeIconsVisibleOnExit;
    private SmartBoxInteraction? _activeInteraction;
    private DesktopItemDrag? _activeItemDrag;
    private TrayIcon? _trayIcon;
    private NativeMenuItem? _showHideTrayItem;
    private NativeMenuItem? _nativeIconsTrayItem;
    private NativeMenuItem? _restoreNativeIconsTrayItem;
    private NativeMenuItem? _refreshTrayItem;
    private NativeMenuItem? _autoLayoutTrayItem;

    public MainWindow(
        IDesktopOverlayHost desktopOverlayHost,
        INativeDesktopIconService nativeDesktopIconService,
        IGlobalHotkeyService globalHotkeyService,
        IAppLogger? logger = null)
    {
        _desktopOverlayHost = desktopOverlayHost;
        _nativeDesktopIconService = nativeDesktopIconService;
        _globalHotkeyService = globalHotkeyService;
        _logger = logger ?? NullAppLogger.Instance;
        InitializeComponent();
        Opened += OnOpened;
        Closing += OnClosing;
        Closed += OnClosed;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        ConfigureDesktopBounds();
        _nativeDesktopIconService.CaptureInitialState();

        if (DataContext is MainWindowViewModel viewModel)
        {
            await viewModel.InitializeAsync();

            if (viewModel.EnableDesktopOverlayHost)
            {
                AttachToDesktopSafely();
            }

            ApplyNativeDesktopIconPreference(viewModel.HideNativeDesktopIcons);
            ConfigureGlobalHotkey(viewModel.EnableGlobalHotkey);

            if (viewModel.EnableTrayIcon)
            {
                ConfigureTrayIcon();
            }

            RefreshTrayMenuLabels();

            if (viewModel.ShouldStartHidden)
            {
                Dispatcher.UIThread.Post(HideTidyTop);
            }
        }
    }

    private void ConfigureDesktopBounds()
    {
        var screen = Screens.ScreenFromWindow(this) ?? Screens.Primary;
        if (screen is null)
        {
            return;
        }

        WindowState = WindowState.Normal;
        Position = screen.Bounds.Position;
        Width = screen.Bounds.Width / screen.Scaling;
        Height = screen.Bounds.Height / screen.Scaling;
    }

    private void AttachToDesktopSafely()
    {
        try
        {
            _desktopOverlayHost.AttachToDesktop(this);
        }
        catch (Exception ex)
        {
            _logger.Error("Could not attach TidyTop to the Windows desktop host. Continuing as a normal borderless window.", ex);
        }
    }

    private void ConfigureTrayIcon()
    {
        if (_trayIcon is not null)
        {
            return;
        }

        try
        {
            var menu = new NativeMenu();

            _showHideTrayItem = new NativeMenuItem { Header = "Hide TidyTop" };
            _showHideTrayItem.Click += (_, _) => ToggleTidyTopVisibility();
            menu.Items.Add(_showHideTrayItem);

            _refreshTrayItem = new NativeMenuItem { Header = "Refresh items" };
            _refreshTrayItem.Click += async (_, _) => await RunRefreshFromTrayAsync();
            menu.Items.Add(_refreshTrayItem);

            _autoLayoutTrayItem = new NativeMenuItem { Header = "Auto layout" };
            _autoLayoutTrayItem.Click += async (_, _) => await RunAutoLayoutFromTrayAsync();
            menu.Items.Add(_autoLayoutTrayItem);

            _nativeIconsTrayItem = new NativeMenuItem { Header = "Hide native desktop icons" };
            _nativeIconsTrayItem.Click += async (_, _) => await ToggleNativeDesktopIconsAsync();
            menu.Items.Add(_nativeIconsTrayItem);

            _restoreNativeIconsTrayItem = new NativeMenuItem { Header = "Restore Windows icons" };
            _restoreNativeIconsTrayItem.Click += async (_, _) => await RestoreNativeDesktopIconsAsync();
            menu.Items.Add(_restoreNativeIconsTrayItem);

            var exitItem = new NativeMenuItem { Header = "Exit safely" };
            exitItem.Click += (_, _) => ExitFromTray();
            menu.Items.Add(exitItem);

            _trayIcon = new TrayIcon
            {
                ToolTipText = "TidyTop",
                Menu = menu,
                IsVisible = true
            };

            TrySetTrayIconImage(_trayIcon);
        }
        catch (Exception ex)
        {
            _logger.Error("Could not initialize the TidyTop tray icon.", ex);
        }
    }

    private static void TrySetTrayIconImage(TrayIcon trayIcon)
    {
        try
        {
            using var stream = AssetLoader.Open(new Uri("avares://TidyTop.App/Assets/logo.png"));
            trayIcon.Icon = new WindowIcon(stream);
        }
        catch
        {
            // The tray still works without a custom icon. Packaging should include Assets/logo.png.
        }
    }

    private void ConfigureGlobalHotkey(bool enabled)
    {
        try
        {
            _globalHotkeyService.ToggleRequested -= OnGlobalHotkeyToggleRequested;
            _globalHotkeyService.Stop();

            if (!enabled)
            {
                return;
            }

            _globalHotkeyService.ToggleRequested += OnGlobalHotkeyToggleRequested;
            _globalHotkeyService.Start();
        }
        catch (Exception ex)
        {
            _logger.Error("Could not start the global TidyTop hotkey.", ex);
        }
    }

    private void OnGlobalHotkeyToggleRequested(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(ToggleTidyTopVisibility);
    }

    private void ToggleTidyTopVisibility()
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        if (viewModel.IsOverlayVisible)
        {
            HideTidyTop();
        }
        else
        {
            ShowTidyTop();
        }
    }

    private void HideTidyTop()
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.SetOverlayVisible(false);
        }

        Hide();
        RefreshTrayMenuLabels();
    }

    private void ShowTidyTop()
    {
        ConfigureDesktopBounds();
        Show();

        if (DataContext is MainWindowViewModel { EnableDesktopOverlayHost: true })
        {
            AttachToDesktopSafely();
        }

        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.SetOverlayVisible(true);
        }

        RefreshTrayMenuLabels();
    }

    private async Task ToggleNativeDesktopIconsAsync()
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        var hideNativeIcons = !viewModel.HideNativeDesktopIcons;
        ApplyNativeDesktopIconPreference(hideNativeIcons);
        await viewModel.SetHideNativeDesktopIconsPreferenceAsync(hideNativeIcons);
        RefreshTrayMenuLabels();
    }

    private async Task RestoreNativeDesktopIconsAsync()
    {
        _nativeDesktopIconService.SetIconsVisible(true);
        _hasHiddenNativeIconsDuringSession = false;
        _forceKeepNativeIconsVisibleOnExit = true;

        if (DataContext is MainWindowViewModel viewModel)
        {
            await viewModel.ForceRestoreNativeDesktopIconsPreferenceAsync();
        }

        RefreshTrayMenuLabels();
    }

    private void ApplyNativeDesktopIconPreference(bool hideNativeIcons)
    {
        if (DataContext is MainWindowViewModel { EnableNativeDesktopIconControl: false } && hideNativeIcons)
        {
            _nativeDesktopIconService.SetIconsVisible(true);
            return;
        }

        _nativeDesktopIconService.CaptureInitialState();
        _nativeDesktopIconService.SetIconsVisible(!hideNativeIcons);
        _hasHiddenNativeIconsDuringSession |= hideNativeIcons;
    }

    private void RefreshTrayMenuLabels()
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        if (_showHideTrayItem is not null)
        {
            _showHideTrayItem.Header = viewModel.IsOverlayVisible ? "Hide TidyTop" : "Show TidyTop";
        }

        if (_nativeIconsTrayItem is not null)
        {
            _nativeIconsTrayItem.Header = viewModel.HideNativeDesktopIcons
                ? "Show native desktop icons"
                : "Hide native desktop icons";
            _nativeIconsTrayItem.IsEnabled = viewModel.EnableNativeDesktopIconControl;
        }

        if (_restoreNativeIconsTrayItem is not null)
        {
            _restoreNativeIconsTrayItem.IsEnabled = _nativeDesktopIconService.IsSupported;
        }
    }

    private async Task RunRefreshFromTrayAsync()
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            await viewModel.RefreshCommand.ExecuteAsync();
        }
    }

    private async Task RunAutoLayoutFromTrayAsync()
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            await viewModel.AutoArrangeAsync((int)Math.Round(ClientSize.Width), (int)Math.Round(ClientSize.Height));
        }
    }

    private void ExitFromTray()
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.SetShuttingDown(true);
        }

        _isClosingFromTray = true;
        Close();
    }

    private void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        // Closing from the window manager should act like Hide for a tray app. The explicit tray
        // Exit command performs a real shutdown and restores native desktop icons.
        if (!_isClosingFromTray)
        {
            e.Cancel = true;
            HideTidyTop();
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        try
        {
            _globalHotkeyService.ToggleRequested -= OnGlobalHotkeyToggleRequested;
            _globalHotkeyService.Stop();
        }
        catch (Exception ex)
        {
            _logger.Error("Could not stop the global hotkey cleanly.", ex);
        }

        try
        {
            if (_forceKeepNativeIconsVisibleOnExit || _hasHiddenNativeIconsDuringSession)
            {
                _nativeDesktopIconService.SetIconsVisible(true);
            }
            else
            {
                _nativeDesktopIconService.RestoreCapturedState();
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Could not restore native desktop icons during shutdown.", ex);
            _nativeDesktopIconService.SetIconsVisible(true);
        }

        _trayIcon?.Dispose();
        _trayIcon = null;
    }

    private async void OnAutoArrangeClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            await viewModel.AutoArrangeAsync((int)Math.Round(ClientSize.Width), (int)Math.Round(ClientSize.Height));
            e.Handled = true;
        }
    }

    private void OnHideTidyTopClick(object? sender, RoutedEventArgs e)
    {
        HideTidyTop();
        e.Handled = true;
    }

    private async void OnToggleNativeDesktopIconsClick(object? sender, RoutedEventArgs e)
    {
        await ToggleNativeDesktopIconsAsync();
        e.Handled = true;
    }

    private async void OnRestoreNativeDesktopIconsClick(object? sender, RoutedEventArgs e)
    {
        await RestoreNativeDesktopIconsAsync();
        e.Handled = true;
    }

    private void OnSmartBoxEditClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: SmartBoxViewModel smartBox } && DataContext is MainWindowViewModel viewModel)
        {
            viewModel.OpenSmartBoxEditor(smartBox);
            e.Handled = true;
        }
    }

    private void OnSmartBoxHeaderPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginSmartBoxInteraction(sender, e, SmartBoxInteractionMode.Move);
    }

    private void OnSmartBoxResizePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginSmartBoxInteraction(sender, e, SmartBoxInteractionMode.Resize);
    }

    private void BeginSmartBoxInteraction(object? sender, PointerPressedEventArgs e, SmartBoxInteractionMode mode)
    {
        if (_activeItemDrag is not null || IsInsideButton(e.Source))
        {
            return;
        }

        if (sender is not Control control || control.DataContext is not SmartBoxViewModel smartBox)
        {
            return;
        }

        var pointerPoint = e.GetCurrentPoint(this);
        if (!pointerPoint.Properties.IsLeftButtonPressed)
        {
            return;
        }

        _activeInteraction = new SmartBoxInteraction(
            smartBox,
            pointerPoint.Position,
            smartBox.X,
            smartBox.Y,
            smartBox.Width,
            smartBox.Height,
            mode);

        if (mode == SmartBoxInteractionMode.Move)
        {
            smartBox.BeginMove();
        }
        else
        {
            smartBox.BeginResize();
        }

        e.Pointer.Capture(control);
        e.Handled = true;
    }

    private void OnSmartBoxInteractionPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_activeInteraction is null)
        {
            return;
        }

        var pointerPoint = e.GetCurrentPoint(this);
        if (!pointerPoint.Properties.IsLeftButtonPressed)
        {
            return;
        }

        var delta = pointerPoint.Position - _activeInteraction.StartPointer;
        if (_activeInteraction.Mode == SmartBoxInteractionMode.Move)
        {
            MoveActiveSmartBox(delta);
        }
        else
        {
            ResizeActiveSmartBox(delta);
        }

        e.Handled = true;
    }

    private async void OnSmartBoxInteractionPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        await FinishSmartBoxInteractionAsync(e.Pointer);
        e.Handled = true;
    }

    private async void OnSmartBoxInteractionPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        await FinishSmartBoxInteractionAsync(null);
    }

    private void MoveActiveSmartBox(Vector delta)
    {
        if (_activeInteraction is null)
        {
            return;
        }

        var smartBox = _activeInteraction.SmartBox;
        var x = _activeInteraction.StartX + (int)Math.Round(delta.X);
        var y = _activeInteraction.StartY + (int)Math.Round(delta.Y);
        smartBox.SetPosition(ClampX(x, smartBox.Width), ClampY(y, smartBox.Height));
    }

    private void ResizeActiveSmartBox(Vector delta)
    {
        if (_activeInteraction is null)
        {
            return;
        }

        var smartBox = _activeInteraction.SmartBox;
        var width = _activeInteraction.StartWidth + (int)Math.Round(delta.X);
        var height = _activeInteraction.StartHeight + (int)Math.Round(delta.Y);

        width = ClampWidth(width, smartBox.X);
        height = ClampHeight(height, smartBox.Y);
        smartBox.SetSize(width, height);
    }

    private async Task FinishSmartBoxInteractionAsync(IPointer? pointer)
    {
        if (_activeInteraction is null)
        {
            return;
        }

        var interaction = _activeInteraction;
        _activeInteraction = null;
        interaction.SmartBox.EndInteraction();
        pointer?.Capture(null);

        if (DataContext is MainWindowViewModel viewModel)
        {
            await viewModel.CommitSmartBoxGeometryAsync(interaction.SmartBox);
        }
    }

    private async void OnDesktopItemOpenClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: DesktopItemViewModel desktopItem } && DataContext is MainWindowViewModel viewModel)
        {
            await viewModel.OpenDesktopItemAsync(desktopItem);
            e.Handled = true;
        }
    }

    private async void OnDesktopItemDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Control { DataContext: DesktopItemViewModel desktopItem } && DataContext is MainWindowViewModel viewModel)
        {
            await viewModel.OpenDesktopItemAsync(desktopItem);
            e.Handled = true;
        }
    }

    private void OnDesktopItemPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_activeInteraction is not null || IsInsideButton(e.Source))
        {
            return;
        }

        if (DataContext is MainWindowViewModel { EnableDragDrop: false })
        {
            return;
        }

        if (sender is not Control control || control.DataContext is not DesktopItemViewModel desktopItem)
        {
            return;
        }

        var pointerPoint = e.GetCurrentPoint(this);
        if (pointerPoint.Properties.IsRightButtonPressed)
        {
            ShowDesktopItemContextMenu(control, desktopItem);
            e.Handled = true;
            return;
        }

        if (!pointerPoint.Properties.IsLeftButtonPressed)
        {
            return;
        }

        _activeItemDrag = new DesktopItemDrag(desktopItem, pointerPoint.Position, false, false);
        e.Pointer.Capture(control);
    }

    private void OnDesktopItemPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_activeItemDrag is null || DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        var pointerPoint = e.GetCurrentPoint(this);
        if (!pointerPoint.Properties.IsLeftButtonPressed)
        {
            return;
        }

        var delta = pointerPoint.Position - _activeItemDrag.StartPointer;
        if (!_activeItemDrag.IsDragging && Math.Sqrt((delta.X * delta.X) + (delta.Y * delta.Y)) >= ItemDragThreshold)
        {
            _activeItemDrag = _activeItemDrag with { IsDragging = true, HasVisualStarted = true };
            viewModel.BeginDesktopItemDrag(
                _activeItemDrag.DesktopItem,
                (int)Math.Round(pointerPoint.Position.X),
                (int)Math.Round(pointerPoint.Position.Y));
        }

        if (_activeItemDrag.IsDragging)
        {
            var targetBox = viewModel.FindSmartBoxAt(pointerPoint.Position.X, pointerPoint.Position.Y);
            if (targetBox?.Id == _activeItemDrag.DesktopItem.SmartBoxId)
            {
                targetBox = null;
            }

            viewModel.UpdateDesktopItemDrag(
                (int)Math.Round(pointerPoint.Position.X),
                (int)Math.Round(pointerPoint.Position.Y),
                targetBox);
        }

        e.Handled = true;
    }

    private async void OnDesktopItemPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        e.Handled = await FinishDesktopItemDragAsync(e.Pointer, e.GetPosition(this));
    }

    private void OnDesktopItemPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (_activeItemDrag is { HasVisualStarted: true } && DataContext is MainWindowViewModel viewModel)
        {
            viewModel.EndDesktopItemDrag();
        }

        _activeItemDrag = null;
    }

    private async Task<bool> FinishDesktopItemDragAsync(IPointer? pointer, Point releasePosition)
    {
        if (_activeItemDrag is null)
        {
            return false;
        }

        var drag = _activeItemDrag;
        _activeItemDrag = null;
        pointer?.Capture(null);

        if (!drag.IsDragging || DataContext is not MainWindowViewModel viewModel)
        {
            return false;
        }

        try
        {
            var targetBox = viewModel.FindSmartBoxAt(releasePosition.X, releasePosition.Y);
            if (targetBox is null || targetBox.Id == drag.DesktopItem.SmartBoxId)
            {
                return true;
            }

            await viewModel.MoveDesktopItemToSmartBoxAsync(drag.DesktopItem, targetBox);
            return true;
        }
        finally
        {
            viewModel.EndDesktopItemDrag();
        }
    }

    private void ShowDesktopItemContextMenu(Control placementTarget, DesktopItemViewModel desktopItem)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        var menu = new ContextMenu();

        var openItem = new MenuItem { Header = "Open" };
        openItem.Click += async (_, _) => await viewModel.OpenDesktopItemAsync(desktopItem);
        menu.Items.Add(openItem);

        var moveMenu = new MenuItem { Header = "Move to SmartBox" };
        var moveTargets = viewModel.GetMoveTargets(desktopItem);
        if (moveTargets.Count == 0)
        {
            moveMenu.IsEnabled = false;
        }
        else
        {
            foreach (var target in moveTargets)
            {
                var targetItem = new MenuItem { Header = target.Title };
                targetItem.Click += async (_, _) => await viewModel.MoveDesktopItemToSmartBoxAsync(desktopItem, target);
                moveMenu.Items.Add(targetItem);
            }
        }

        menu.Items.Add(moveMenu);

        var moveToUnboxed = new MenuItem { Header = "Move to Other / Unboxed" };
        moveToUnboxed.Click += async (_, _) => await viewModel.MoveDesktopItemToUnboxedAsync(desktopItem);
        menu.Items.Add(moveToUnboxed);

        menu.Open(placementTarget);
    }

    private static bool IsInsideButton(object? source)
    {
        var current = source as Control;
        while (current is not null)
        {
            if (current is Button)
            {
                return true;
            }

            current = current.Parent as Control;
        }

        return false;
    }

    private int ClampX(int x, int boxWidth)
    {
        var max = Math.Max(0, (int)Math.Round(ClientSize.Width) - Math.Min(boxWidth, 80));
        return Math.Clamp(x, 0, max);
    }

    private int ClampY(int y, int boxHeight)
    {
        var max = Math.Max(0, (int)Math.Round(ClientSize.Height) - Math.Min(boxHeight, 48));
        return Math.Clamp(y, 0, max);
    }

    private int ClampWidth(int width, int x)
    {
        var max = Math.Max(SmartBoxViewModel.MinimumWidth, (int)Math.Round(ClientSize.Width) - x);
        return Math.Clamp(width, SmartBoxViewModel.MinimumWidth, max);
    }

    private int ClampHeight(int height, int y)
    {
        var max = Math.Max(SmartBoxViewModel.MinimumHeight, (int)Math.Round(ClientSize.Height) - y);
        return Math.Clamp(height, SmartBoxViewModel.MinimumHeight, max);
    }

    private sealed record SmartBoxInteraction(
        SmartBoxViewModel SmartBox,
        Point StartPointer,
        int StartX,
        int StartY,
        int StartWidth,
        int StartHeight,
        SmartBoxInteractionMode Mode);

    private sealed record DesktopItemDrag(
        DesktopItemViewModel DesktopItem,
        Point StartPointer,
        bool IsDragging,
        bool HasVisualStarted);

    private enum SmartBoxInteractionMode
    {
        Move,
        Resize
    }
}
