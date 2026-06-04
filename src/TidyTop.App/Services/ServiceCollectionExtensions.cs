using Microsoft.Extensions.DependencyInjection;
using TidyTop.App.ViewModels;
using TidyTop.Core.Services;

namespace TidyTop.App.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTidyTop(this IServiceCollection services)
    {
        services.AddSingleton(AppDataPaths.CreateDefault());
        services.AddSingleton<IAppLogger, FileAppLogger>();
        services.AddSingleton<IDesktopScanner, DesktopScanner>();
        services.AddSingleton<ILayoutStore, JsonLayoutStore>();
        services.AddSingleton<IAppSettingsStore, JsonAppSettingsStore>();
        services.AddSingleton<LayoutReconciler>();
        services.AddSingleton<IDesktopWorkspaceService, DesktopWorkspaceService>();
        services.AddSingleton<IDesktopItemLauncher, DesktopItemLauncher>();
        services.AddSingleton<IDesktopOverlayHost, WindowsDesktopOverlayHost>();
        services.AddSingleton<INativeDesktopIconService, WindowsNativeDesktopIconService>();
        services.AddSingleton<IGlobalHotkeyService, WindowsGlobalHotkeyService>();

        services.AddTransient<MainWindowViewModel>();

        return services;
    }
}
