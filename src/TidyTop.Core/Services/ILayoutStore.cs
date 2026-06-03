using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

public interface ILayoutStore
{
    Task<DesktopLayout?> LoadAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(DesktopLayout layout, CancellationToken cancellationToken = default);
    Task DeleteAsync(CancellationToken cancellationToken = default);
}
