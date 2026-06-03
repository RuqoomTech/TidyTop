using Microsoft.Extensions.DependencyInjection;
using TidyTop.Core.Services;

namespace TidyTop.App.Services;

/// <summary>
/// Extension methods for configuring services in the dependency injection container
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds core TidyTop services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddTidyTopServices(this IServiceCollection services)
    {
        // Register services
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IDesktopIconService, DesktopIconService>();
        services.AddSingleton<ISmartBoxService, SmartBoxService>();
        services.AddSingleton<IDesktopLayoutService, DesktopLayoutService>();

        return services;
    }
}