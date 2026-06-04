using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

public interface IDesktopWorkspaceService
{
    Task<DesktopWorkspace> LoadAsync(CancellationToken cancellationToken = default);
    Task<DesktopWorkspace> RefreshAsync(CancellationToken cancellationToken = default);
    Task<DesktopWorkspace> AddSmartBoxAsync(string title, CancellationToken cancellationToken = default);
    Task<DesktopWorkspace> ResetLayoutAsync(CancellationToken cancellationToken = default);
    Task UpdateSmartBoxGeometryAsync(Guid smartBoxId, int x, int y, int width, int height, CancellationToken cancellationToken = default);
    Task<DesktopWorkspace> MoveItemToSmartBoxAsync(string itemPath, Guid targetSmartBoxId, CancellationToken cancellationToken = default);
    Task<DesktopWorkspace> MoveItemToUnboxedAsync(string itemPath, CancellationToken cancellationToken = default);
    Task SaveAsync(CancellationToken cancellationToken = default);
}
