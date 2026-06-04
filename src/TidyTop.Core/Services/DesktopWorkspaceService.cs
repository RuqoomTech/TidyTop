using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

/// <summary>
/// High-level application service for scan → reconcile → persist.
/// </summary>
public sealed class DesktopWorkspaceService : IDesktopWorkspaceService
{
    private readonly IDesktopScanner _scanner;
    private readonly ILayoutStore _layoutStore;
    private readonly LayoutReconciler _reconciler;
    private DesktopLayout? _layout;
    private IReadOnlyList<DesktopItem> _lastItems = Array.Empty<DesktopItem>();

    public DesktopWorkspaceService(IDesktopScanner scanner, ILayoutStore layoutStore, LayoutReconciler reconciler)
    {
        _scanner = scanner;
        _layoutStore = layoutStore;
        _reconciler = reconciler;
    }

    public async Task<DesktopWorkspace> LoadAsync(CancellationToken cancellationToken = default)
    {
        _layout = await _layoutStore.LoadAsync(cancellationToken) ?? DefaultSmartBoxFactory.CreateDefaultLayout();
        _lastItems = await _scanner.ScanAsync(cancellationToken);
        var workspace = _reconciler.Reconcile(_layout, _lastItems);
        await _layoutStore.SaveAsync(workspace.Layout, cancellationToken);
        return workspace;
    }

    public async Task<DesktopWorkspace> RefreshAsync(CancellationToken cancellationToken = default)
    {
        _layout ??= await _layoutStore.LoadAsync(cancellationToken) ?? DefaultSmartBoxFactory.CreateDefaultLayout();
        _lastItems = await _scanner.ScanAsync(cancellationToken);
        var workspace = _reconciler.Reconcile(_layout, _lastItems);
        await _layoutStore.SaveAsync(workspace.Layout, cancellationToken);
        return workspace;
    }

    public async Task<DesktopWorkspace> AddSmartBoxAsync(string title, CancellationToken cancellationToken = default)
    {
        _layout ??= await _layoutStore.LoadAsync(cancellationToken) ?? DefaultSmartBoxFactory.CreateDefaultLayout();

        var manualBoxCount = _layout.SmartBoxes.Count(box => !box.IsSystemBox) + 1;
        _layout.SmartBoxes.Insert(Math.Max(0, _layout.SmartBoxes.Count - 1), new SmartBox
        {
            Title = string.IsNullOrWhiteSpace(title) ? $"New SmartBox {manualBoxCount}" : title.Trim(),
            Emoji = "📦",
            AccentColor = "#64748B",
            Behavior = SmartBoxBehavior.Manual,
            IsSystemBox = false,
            X = 24 + (manualBoxCount * 28),
            Y = 24 + (manualBoxCount * 28)
        });

        var workspace = _reconciler.Reconcile(_layout, _lastItems);
        await _layoutStore.SaveAsync(workspace.Layout, cancellationToken);
        return workspace;
    }


    public async Task<DesktopWorkspace> RenameSmartBoxAsync(
        Guid smartBoxId,
        string title,
        CancellationToken cancellationToken = default)
    {
        _layout ??= await _layoutStore.LoadAsync(cancellationToken) ?? DefaultSmartBoxFactory.CreateDefaultLayout();
        if (_lastItems.Count == 0)
        {
            _lastItems = await _scanner.ScanAsync(cancellationToken);
        }

        var smartBox = _layout.FindBox(smartBoxId);
        if (smartBox is null)
        {
            throw new InvalidOperationException("The SmartBox no longer exists.");
        }

        var cleanedTitle = string.IsNullOrWhiteSpace(title) ? "Untitled SmartBox" : title.Trim();
        smartBox.Title = cleanedTitle.Length > 48 ? cleanedTitle[..48] : cleanedTitle;
        smartBox.UpdatedUtc = DateTimeOffset.UtcNow;
        _layout.UpdatedUtc = DateTimeOffset.UtcNow;

        var workspace = _reconciler.Reconcile(_layout, _lastItems);
        await _layoutStore.SaveAsync(workspace.Layout, cancellationToken);
        return workspace;
    }

    public async Task<DesktopWorkspace> DeleteSmartBoxAsync(Guid smartBoxId, CancellationToken cancellationToken = default)
    {
        _layout ??= await _layoutStore.LoadAsync(cancellationToken) ?? DefaultSmartBoxFactory.CreateDefaultLayout();
        if (_lastItems.Count == 0)
        {
            _lastItems = await _scanner.ScanAsync(cancellationToken);
        }

        var smartBox = _layout.FindBox(smartBoxId);
        if (smartBox is null)
        {
            throw new InvalidOperationException("The SmartBox no longer exists.");
        }

        if (smartBox.IsSystemBox)
        {
            throw new InvalidOperationException("Default SmartBoxes cannot be deleted yet.");
        }

        _layout.SmartBoxes.Remove(smartBox);
        _layout.UpdatedUtc = DateTimeOffset.UtcNow;

        var workspace = _reconciler.Reconcile(_layout, _lastItems);
        await _layoutStore.SaveAsync(workspace.Layout, cancellationToken);
        return workspace;
    }

    public async Task<DesktopWorkspace> ResetLayoutAsync(CancellationToken cancellationToken = default)
    {
        _layout = DefaultSmartBoxFactory.CreateDefaultLayout();
        _lastItems = await _scanner.ScanAsync(cancellationToken);
        var workspace = _reconciler.Reconcile(_layout, _lastItems);
        await _layoutStore.SaveAsync(workspace.Layout, cancellationToken);
        return workspace;
    }



    public async Task<DesktopWorkspace> AutoArrangeAsync(int surfaceWidth, int surfaceHeight, CancellationToken cancellationToken = default)
    {
        _layout ??= await _layoutStore.LoadAsync(cancellationToken) ?? DefaultSmartBoxFactory.CreateDefaultLayout();
        if (_lastItems.Count == 0)
        {
            _lastItems = await _scanner.ScanAsync(cancellationToken);
        }

        ArrangeSmartBoxes(_layout, surfaceWidth, surfaceHeight);
        _layout.UpdatedUtc = DateTimeOffset.UtcNow;

        var workspace = _reconciler.Reconcile(_layout, _lastItems);
        await _layoutStore.SaveAsync(workspace.Layout, cancellationToken);
        return workspace;
    }

    public async Task UpdateSmartBoxGeometryAsync(
        Guid smartBoxId,
        int x,
        int y,
        int width,
        int height,
        CancellationToken cancellationToken = default)
    {
        _layout ??= await _layoutStore.LoadAsync(cancellationToken) ?? DefaultSmartBoxFactory.CreateDefaultLayout();

        var smartBox = _layout.FindBox(smartBoxId);
        if (smartBox is null)
        {
            return;
        }

        smartBox.SetGeometry(x, y, width, height);
        _layout.UpdatedUtc = DateTimeOffset.UtcNow;
        await _layoutStore.SaveAsync(_layout, cancellationToken);
    }

    public async Task<DesktopWorkspace> MoveItemToSmartBoxAsync(
        string itemPath,
        Guid targetSmartBoxId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(itemPath))
        {
            throw new ArgumentException("Desktop item path cannot be empty.", nameof(itemPath));
        }

        _layout ??= await _layoutStore.LoadAsync(cancellationToken) ?? DefaultSmartBoxFactory.CreateDefaultLayout();
        if (_lastItems.Count == 0)
        {
            _lastItems = await _scanner.ScanAsync(cancellationToken);
        }

        var normalizedPath = DesktopItem.NormalizePath(itemPath);
        var targetBox = _layout.FindBox(targetSmartBoxId);
        if (targetBox is null)
        {
            throw new InvalidOperationException("The target SmartBox no longer exists.");
        }

        RemoveItemFromAllBoxes(normalizedPath);
        targetBox.AssignPath(normalizedPath);

        _layout.UpdatedUtc = DateTimeOffset.UtcNow;
        var workspace = _reconciler.Reconcile(_layout, _lastItems);
        await _layoutStore.SaveAsync(workspace.Layout, cancellationToken);
        return workspace;
    }

    public async Task<DesktopWorkspace> MoveItemToUnboxedAsync(string itemPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(itemPath))
        {
            throw new ArgumentException("Desktop item path cannot be empty.", nameof(itemPath));
        }

        _layout ??= await _layoutStore.LoadAsync(cancellationToken) ?? DefaultSmartBoxFactory.CreateDefaultLayout();
        if (_lastItems.Count == 0)
        {
            _lastItems = await _scanner.ScanAsync(cancellationToken);
        }

        var normalizedPath = DesktopItem.NormalizePath(itemPath);
        RemoveItemFromAllBoxes(normalizedPath);

        var catchAll = _layout.SmartBoxes.FirstOrDefault(box => box.Behavior == SmartBoxBehavior.CatchAll);
        catchAll?.AssignPath(normalizedPath);

        _layout.UpdatedUtc = DateTimeOffset.UtcNow;
        var workspace = _reconciler.Reconcile(_layout, _lastItems);
        await _layoutStore.SaveAsync(workspace.Layout, cancellationToken);
        return workspace;
    }


    private static void ArrangeSmartBoxes(DesktopLayout layout, int surfaceWidth, int surfaceHeight)
    {
        const int margin = 28;
        const int gap = 18;
        const int reservedTop = 86;
        const int minimumColumnWidth = 300;
        const int maximumColumnWidth = 430;
        const int minimumBoxHeight = 198;
        const int maximumBoxHeight = 340;

        var safeWidth = Math.Max(900, surfaceWidth);
        var columnCount = safeWidth switch
        {
            >= 1700 => 4,
            >= 1180 => 3,
            >= 760 => 2,
            _ => 1
        };

        var availableWidth = Math.Max(minimumColumnWidth, safeWidth - (margin * 2) - (gap * (columnCount - 1)));
        var columnWidth = Math.Clamp(availableWidth / columnCount, minimumColumnWidth, maximumColumnWidth);
        var xPositions = Enumerable.Range(0, columnCount)
            .Select(index => margin + (index * (columnWidth + gap)))
            .ToArray();
        var yPositions = Enumerable.Repeat(reservedTop, columnCount).ToArray();

        var orderedBoxes = layout.SmartBoxes
            .OrderBy(box => box.Behavior == SmartBoxBehavior.CatchAll ? 1 : 0)
            .ThenBy(box => box.CreatedUtc)
            .ThenBy(box => box.Title, StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var smartBox in orderedBoxes)
        {
            var column = IndexOfShortestColumn(yPositions);
            var estimatedHeight = EstimateBoxHeight(smartBox.ItemPaths.Count, smartBox.Behavior == SmartBoxBehavior.CatchAll);
            var x = xPositions[column];
            var y = yPositions[column];

            smartBox.SetGeometry(x, y, columnWidth, estimatedHeight);
            yPositions[column] += estimatedHeight + gap;
        }
    }

    private static int EstimateBoxHeight(int itemCount, bool isCatchAll)
    {
        const int minimumBoxHeight = 198;
        const int maximumBoxHeight = 340;

        if (itemCount == 0)
        {
            return isCatchAll ? 220 : minimumBoxHeight;
        }

        return Math.Clamp(152 + (itemCount * 52), minimumBoxHeight, maximumBoxHeight);
    }

    private static int IndexOfShortestColumn(IReadOnlyList<int> yPositions)
    {
        var index = 0;
        var shortest = yPositions[0];
        for (var i = 1; i < yPositions.Count; i++)
        {
            if (yPositions[i] < shortest)
            {
                index = i;
                shortest = yPositions[i];
            }
        }

        return index;
    }

    private void RemoveItemFromAllBoxes(string normalizedPath)
    {
        if (_layout is null)
        {
            return;
        }

        foreach (var box in _layout.SmartBoxes)
        {
            box.RemovePath(normalizedPath);
        }
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (_layout is not null)
        {
            await _layoutStore.SaveAsync(_layout, cancellationToken);
        }
    }
}
