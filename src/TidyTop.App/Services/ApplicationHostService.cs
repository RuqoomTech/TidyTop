using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TidyTop.Core.Services;

namespace TidyTop.App.Services;

/// <summary>
/// Host service for managing the application's dependency injection container
/// </summary>
public class ApplicationHostService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationHostService"/> class
    /// </summary>
    /// <param name="serviceProvider">The service provider</param>
    /// <param name="hostApplicationLifetime">The host application lifetime</param>
    public ApplicationHostService(
        IServiceProvider serviceProvider,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
    }

    /// <summary>
    /// Gets the service provider for resolving services
    /// </summary>
    public IServiceProvider Services => _serviceProvider;

    /// <summary>
    /// Starts the host service
    /// </summary>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns>A task representing the start operation</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Initialize services that need to be started when the application starts
        var settingsService = _serviceProvider.GetRequiredService<ISettingsService>();
        var desktopIconService = _serviceProvider.GetRequiredService<IDesktopIconService>();
        var smartBoxService = _serviceProvider.GetRequiredService<ISmartBoxService>();
        var desktopLayoutService = _serviceProvider.GetRequiredService<IDesktopLayoutService>();

        // TODO: Initialize services as needed
        // For example, load settings, scan desktop icons, etc.

        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the host service
    /// </summary>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <returns>A task representing the stop operation</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Clean up resources if needed
        return Task.CompletedTask;
    }

    /// <summary>
    /// Gets a service of the specified type
    /// </summary>
    /// <typeparam name="T">The type of service to get</typeparam>
    /// <returns>A service of the specified type</returns>
    public T GetService<T>() where T : notnull
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Gets a service of the specified type
    /// </summary>
    /// <param name="serviceType">The type of service to get</param>
    /// <returns>A service object of the specified type</returns>
    public object GetService(Type serviceType)
    {
        return _serviceProvider.GetRequiredService(serviceType);
    }

    /// <summary>
    /// Gets all services of the specified type
    /// </summary>
    /// <typeparam name="T">The type of service to get</typeparam>
    /// <returns>An enumeration of services of the specified type</returns>
    public IEnumerable<T> GetServices<T>() where T : notnull
    {
        return _serviceProvider.GetServices<T>();
    }

    /// <summary>
    /// Gets all services of the specified type
    /// </summary>
    /// <param name="serviceType">The type of service to get</param>
    /// <returns>An enumeration of service objects of the specified type</returns>
    public IEnumerable<object> GetServices(Type serviceType)
    {
        return _serviceProvider.GetServices(serviceType).Where(s => s is not null)!;
    }
}