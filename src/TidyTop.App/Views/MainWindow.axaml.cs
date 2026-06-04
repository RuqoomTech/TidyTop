using Avalonia.Controls;
using TidyTop.App.Services;
using TidyTop.App.ViewModels;

namespace TidyTop.App.Views;

public partial class MainWindow : Window
{
    private readonly IDesktopOverlayHost _desktopOverlayHost;
    private bool _initialized;

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
}
