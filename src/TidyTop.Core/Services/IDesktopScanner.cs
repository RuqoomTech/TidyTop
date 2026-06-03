using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

public interface IDesktopScanner
{
    Task<IReadOnlyList<DesktopItem>> ScanAsync(CancellationToken cancellationToken = default);
}
