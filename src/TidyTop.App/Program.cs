using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using TidyTop.App.Services;

namespace TidyTop.App;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var services = new ServiceCollection()
            .AddTidyTop()
            .BuildServiceProvider(validateScopes: true);

        App.Services = services;

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            services.Dispose();
        }
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}
