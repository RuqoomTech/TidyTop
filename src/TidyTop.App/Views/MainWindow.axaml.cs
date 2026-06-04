using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using TidyTop.App.Services;
using TidyTop.App.ViewModels;

namespace TidyTop.App.Views;

public partial class MainWindow : Window
{
    private readonly IDesktopOverlayHost _desktopOverlayHost;
    private bool _initialized;
    private SmartBoxInteraction? _activeInteraction;

    public MainWindow(IDesktopOverlayHost desktopOverlayHost)
    {
        _desktopOverlayHost = desktopOverlayHost;
        InitializeComponent();
        Opened += OnOpened;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        ConfigureDesktopBounds();
        _desktopOverlayHost.AttachToDesktop(this);

        if (DataContext is MainWindowViewModel viewModel)
        {
            await viewModel.InitializeAsync();
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
        if (sender is not Control control || control.DataContext is not SmartBoxViewModel smartBox)
        {
            return;
        }

        var pointerPoint = e.GetCurrentPoint(this);
        if (!pointerPoint.Properties.IsLeftButtonPressed || smartBox.IsLocked)
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

    private enum SmartBoxInteractionMode
    {
        Move,
        Resize
    }
}
