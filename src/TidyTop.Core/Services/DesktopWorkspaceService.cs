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

    public async Task<DesktopWorkspace> ResetLayoutAsync(CancellationToken cancellationToken = default)
    {
        _layout = DefaultSmartBoxFactory.CreateDefaultLayout();
        _lastItems = await _scanner.ScanAsync(cancellationToken);
        var workspace = _reconciler.Reconcile(_layout, _lastItems);
        await _layoutStore.SaveAsync(workspace.Layout, cancellationToken);
        return workspace;
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (_layout is not null)
        {
            await _layoutStore.SaveAsync(_layout, cancellationToken);
        }
    }
}
