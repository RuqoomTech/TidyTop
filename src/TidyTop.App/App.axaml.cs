using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using TidyTop.App.Services;
using TidyTop.App.ViewModels;
using TidyTop.App.Views;
using TidyTop.Core.Services;

namespace TidyTop.App;

public sealed partial class App : Application
{
    public static IServiceProvider Services { get; set; } = new ServiceCollection().BuildServiceProvider();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
            desktop.MainWindow = new MainWindow(
                Services.GetRequiredService<IDesktopOverlayHost>(),
                Services.GetRequiredService<INativeDesktopIconService>(),
                Services.GetRequiredService<IGlobalHotkeyService>())
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
