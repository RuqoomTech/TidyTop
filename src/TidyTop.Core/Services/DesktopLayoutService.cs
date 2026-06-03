using System.Collections.Concurrent;
using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

/// <summary>
/// In-memory layout service. Disk persistence is a planned MVP task.
/// </summary>
public class DesktopLayoutService : IDesktopLayoutService
{
    private readonly ConcurrentDictionary<Guid, DesktopLayout> _layouts = new();
    private readonly ISmartBoxService _smartBoxService;
    private readonly IDesktopIconService _desktopIconService;
    private Guid? _activeLayoutId;

    public DesktopLayoutService(ISmartBoxService smartBoxService, IDesktopIconService desktopIconService)
    {
        _smartBoxService = smartBoxService ?? throw new ArgumentNullException(nameof(smartBoxService));
        _desktopIconService = desktopIconService ?? throw new ArgumentNullException(nameof(desktopIconService));
    }

    public Task<IEnumerable<DesktopLayout>> GetLayoutsAsync()
    {
        return Task.FromResult(_layouts.Values.ToList().AsEnumerable());
    }

    public Task<DesktopLayout?> GetLayoutAsync(Guid id)
    {
        _layouts.TryGetValue(id, out var layout);
        return Task.FromResult(layout);
    }

    public async Task<DesktopLayout?> GetCurrentLayoutAsync()
    {
        if (_activeLayoutId.HasValue && _layouts.TryGetValue(_activeLayoutId.Value, out var layout))
        {
            return layout;
        }

        var defaultLayout = new DesktopLayout
        {
            Id = Guid.NewGuid(),
            Name = "Default Layout",
            IsDefault = true,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };

        await AddLayoutAsync(defaultLayout);
        await SetActiveLayoutAsync(defaultLayout.Id);
        return defaultLayout;
    }

    public Task<Guid> AddLayoutAsync(DesktopLayout layout)
    {
        ArgumentNullException.ThrowIfNull(layout);

        layout.Id = layout.Id == Guid.Empty ? Guid.NewGuid() : layout.Id;
        layout.CreatedDate = layout.CreatedDate == default ? DateTime.Now : layout.CreatedDate;
        layout.ModifiedDate = DateTime.Now;

        var added = _layouts.TryAdd(layout.Id, layout);
        return Task.FromResult(added ? layout.Id : Guid.Empty);
    }

    public Task<bool> UpdateLayoutAsync(DesktopLayout layout)
    {
        ArgumentNullException.ThrowIfNull(layout);

        if (!_layouts.TryGetValue(layout.Id, out var existing))
        {
            return Task.FromResult(false);
        }

        layout.ModifiedDate = DateTime.Now;
        return Task.FromResult(_layouts.TryUpdate(layout.Id, layout, existing));
    }

    public Task<bool> RemoveLayoutAsync(Guid id)
    {
        var removed = _layouts.TryRemove(id, out _);
        if (removed && _activeLayoutId == id)
        {
            _activeLayoutId = null;
        }

        return Task.FromResult(removed);
    }

    public Task<bool> SetActiveLayoutAsync(Guid id)
    {
        if (!_layouts.ContainsKey(id))
        {
            return Task.FromResult(false);
        }

        _activeLayoutId = id;
        return Task.FromResult(true);
    }

    public async Task<Guid> SaveCurrentLayoutAsync(string name)
    {
        var smartBoxes = (await _smartBoxService.GetSmartBoxesAsync()).ToList();
        var icons = (await _desktopIconService.GetDesktopIconsAsync()).ToList();

        var layout = new DesktopLayout
        {
            Id = Guid.NewGuid(),
            Name = string.IsNullOrWhiteSpace(name) ? "Untitled Layout" : name.Trim(),
            SmartBoxes = smartBoxes,
            UnboxedIcons = icons.Where(icon => string.IsNullOrWhiteSpace(icon.SmartBoxId)).ToList(),
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };

        await AddLayoutAsync(layout);
        await SetActiveLayoutAsync(layout.Id);
        return layout.Id;
    }

    public async Task<bool> RestoreLayoutAsync(Guid id)
    {
        var layout = await GetLayoutAsync(id);
        if (layout is null)
        {
            return false;
        }

        return await SetActiveLayoutAsync(id);
    }

    public async Task<Guid> CopyLayoutAsync(Guid id, string newName)
    {
        var originalLayout = await GetLayoutAsync(id);
        if (originalLayout is null)
        {
            return Guid.Empty;
        }

        var copiedLayout = originalLayout.Clone();
        copiedLayout.Name = string.IsNullOrWhiteSpace(newName) ? $"{originalLayout.Name} Copy" : newName.Trim();

        return await AddLayoutAsync(copiedLayout);
    }
}
