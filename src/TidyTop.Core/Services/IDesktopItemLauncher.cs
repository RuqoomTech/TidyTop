using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

public interface IDesktopItemLauncher
{
    Task LaunchAsync(DesktopItem item, CancellationToken cancellationToken = default);
    Task LaunchAsync(string path, CancellationToken cancellationToken = default);
}
