using System.Collections.Concurrent;
using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

/// <summary>
/// In-memory implementation of SmartBox management.
/// </summary>
public class SmartBoxService : ISmartBoxService
{
    private readonly ConcurrentDictionary<Guid, SmartBox> _smartBoxes = new();
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, DesktopIcon>> _smartBoxIcons = new();
    private readonly IDesktopIconService _desktopIconService;

    public SmartBoxService(IDesktopIconService desktopIconService)
    {
        _desktopIconService = desktopIconService ?? throw new ArgumentNullException(nameof(desktopIconService));
    }

    public Task<IEnumerable<SmartBox>> GetSmartBoxesAsync()
    {
        return Task.FromResult(_smartBoxes.Values.ToList().AsEnumerable());
    }

    public Task<SmartBox?> GetSmartBoxAsync(Guid id)
    {
        _smartBoxes.TryGetValue(id, out var smartBox);
        return Task.FromResult(smartBox);
    }

    public Task<Guid> AddSmartBoxAsync(SmartBox smartBox)
    {
        ArgumentNullException.ThrowIfNull(smartBox);

        var smartBoxId = TryParseId(smartBox.Id) ?? Guid.NewGuid();
        smartBox.Id = smartBoxId.ToString();
        smartBox.CreatedDate = smartBox.CreatedDate == default ? DateTime.Now : smartBox.CreatedDate;
        smartBox.ModifiedDate = DateTime.Now;

        var added = _smartBoxes.TryAdd(smartBoxId, smartBox);
        if (added)
        {
            _smartBoxIcons.TryAdd(smartBoxId, new ConcurrentDictionary<string, DesktopIcon>());
        }

        return Task.FromResult(added ? smartBoxId : Guid.Empty);
    }

    public Task<bool> UpdateSmartBoxAsync(SmartBox smartBox)
    {
        ArgumentNullException.ThrowIfNull(smartBox);

        var smartBoxId = TryParseId(smartBox.Id);
        if (smartBoxId is null || !_smartBoxes.TryGetValue(smartBoxId.Value, out var existing))
        {
            return Task.FromResult(false);
        }

        smartBox.ModifiedDate = DateTime.Now;
        return Task.FromResult(_smartBoxes.TryUpdate(smartBoxId.Value, smartBox, existing));
    }

    public Task<bool> RemoveSmartBoxAsync(Guid id)
    {
        var removed = _smartBoxes.TryRemove(id, out _);
        _smartBoxIcons.TryRemove(id, out _);
        return Task.FromResult(removed);
    }

    public async Task<bool> AddIconToSmartBoxAsync(Guid smartBoxId, string iconPath)
    {
        if (!_smartBoxes.ContainsKey(smartBoxId))
        {
            return false;
        }

        var icon = await _desktopIconService.GetDesktopIconAsync(iconPath);
        if (icon is null)
        {
            return false;
        }

        icon.SmartBoxId = smartBoxId.ToString();
        var icons = _smartBoxIcons.GetOrAdd(smartBoxId, _ => new ConcurrentDictionary<string, DesktopIcon>());
        return icons.TryAdd(iconPath, icon);
    }

    public Task<bool> RemoveIconFromSmartBoxAsync(Guid smartBoxId, string iconPath)
    {
        if (!_smartBoxIcons.TryGetValue(smartBoxId, out var icons))
        {
            return Task.FromResult(false);
        }

        var removed = icons.TryRemove(iconPath, out var icon);
        if (removed && icon is not null)
        {
            icon.SmartBoxId = null;
        }

        return Task.FromResult(removed);
    }

    public Task<IEnumerable<DesktopIcon>> GetIconsInSmartBoxAsync(Guid smartBoxId)
    {
        if (!_smartBoxIcons.TryGetValue(smartBoxId, out var icons))
        {
            return Task.FromResult(Enumerable.Empty<DesktopIcon>());
        }

        return Task.FromResult(icons.Values.ToList().AsEnumerable());
    }

    public async Task<bool> MoveSmartBoxAsync(Guid smartBoxId, int x, int y)
    {
        var smartBox = await GetSmartBoxAsync(smartBoxId);
        if (smartBox is null || smartBox.IsLocked)
        {
            return false;
        }

        smartBox.Position = new System.Drawing.Point(x, y);
        return await UpdateSmartBoxAsync(smartBox);
    }

    public async Task<bool> ResizeSmartBoxAsync(Guid smartBoxId, int width, int height)
    {
        var smartBox = await GetSmartBoxAsync(smartBoxId);
        if (smartBox is null || smartBox.IsLocked || width <= 0 || height <= 0)
        {
            return false;
        }

        smartBox.Size = new System.Drawing.Size(width, height);
        return await UpdateSmartBoxAsync(smartBox);
    }

    private static Guid? TryParseId(string? id)
    {
        return Guid.TryParse(id, out var parsed) ? parsed : null;
    }
}
