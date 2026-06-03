using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

/// <summary>
/// Service for managing SmartBox containers.
/// </summary>
public interface ISmartBoxService
{
    /// <summary>
    /// Gets all SmartBoxes.
    /// </summary>
    Task<IEnumerable<SmartBox>> GetSmartBoxesAsync();

    /// <summary>
    /// Gets a SmartBox by ID.
    /// </summary>
    Task<SmartBox?> GetSmartBoxAsync(Guid id);

    /// <summary>
    /// Adds a new SmartBox.
    /// </summary>
    Task<Guid> AddSmartBoxAsync(SmartBox smartBox);

    /// <summary>
    /// Updates an existing SmartBox.
    /// </summary>
    Task<bool> UpdateSmartBoxAsync(SmartBox smartBox);

    /// <summary>
    /// Removes a SmartBox.
    /// </summary>
    Task<bool> RemoveSmartBoxAsync(Guid id);

    /// <summary>
    /// Adds a desktop item to a SmartBox.
    /// </summary>
    Task<bool> AddIconToSmartBoxAsync(Guid smartBoxId, string iconPath);

    /// <summary>
    /// Removes a desktop item from a SmartBox.
    /// </summary>
    Task<bool> RemoveIconFromSmartBoxAsync(Guid smartBoxId, string iconPath);

    /// <summary>
    /// Gets all desktop items in a SmartBox.
    /// </summary>
    Task<IEnumerable<DesktopIcon>> GetIconsInSmartBoxAsync(Guid smartBoxId);

    /// <summary>
    /// Moves a SmartBox to a new position.
    /// </summary>
    Task<bool> MoveSmartBoxAsync(Guid smartBoxId, int x, int y);

    /// <summary>
    /// Resizes a SmartBox.
    /// </summary>
    Task<bool> ResizeSmartBoxAsync(Guid smartBoxId, int width, int height);
}
