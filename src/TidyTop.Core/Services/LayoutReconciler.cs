using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

/// <summary>
/// Converts a saved layout and a fresh desktop scan into the runtime workspace shown by the UI.
/// </summary>
public sealed class LayoutReconciler
{
    public DesktopWorkspace Reconcile(DesktopLayout layout, IReadOnlyList<DesktopItem> desktopItems)
    {
        ArgumentNullException.ThrowIfNull(layout);
        ArgumentNullException.ThrowIfNull(desktopItems);

        EnsureDefaultSystemBoxes(layout);

        var itemByPath = desktopItems
            .GroupBy(item => item.NormalizedPath, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        var knownPaths = itemByPath.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var box in layout.SmartBoxes)
        {
            box.NormalizeAssignments(knownPaths);
        }

        RemoveDuplicateAssignments(layout);
        AutoAssignRuleBasedBoxes(layout, desktopItems);
        AssignCatchAll(layout, desktopItems);

        var snapshots = layout.SmartBoxes
            .Where(box => box.IsVisible)
            .Select(box => new SmartBoxSnapshot(
                box,
                box.ItemPaths
                    .Where(itemByPath.ContainsKey)
                    .Select(path => itemByPath[path])
                    .OrderBy(item => item.Type == DesktopItemType.Folder ? 0 : 1)
                    .ThenBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                    .ToList()))
            .ToList();

        layout.UpdatedUtc = DateTimeOffset.UtcNow;
        return new DesktopWorkspace(layout, desktopItems, snapshots);
    }

    private static void EnsureDefaultSystemBoxes(DesktopLayout layout)
    {
        if (layout.SmartBoxes.Count == 0)
        {
            layout.SmartBoxes.AddRange(DefaultSmartBoxFactory.CreateDefaultLayout().SmartBoxes);
            return;
        }

        if (layout.SmartBoxes.All(box => box.Behavior != SmartBoxBehavior.CatchAll))
        {
            layout.SmartBoxes.Add(DefaultSmartBoxFactory.CreateDefaultLayout().SmartBoxes.Single(box => box.Behavior == SmartBoxBehavior.CatchAll));
        }
    }

    private static void RemoveDuplicateAssignments(DesktopLayout layout)
    {
        var assigned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var box in layout.SmartBoxes.Where(box => box.Behavior != SmartBoxBehavior.CatchAll))
        {
            box.ItemPaths = box.ItemPaths.Where(assigned.Add).ToList();
        }

        foreach (var catchAll in layout.SmartBoxes.Where(box => box.Behavior == SmartBoxBehavior.CatchAll))
        {
            catchAll.ItemPaths = catchAll.ItemPaths.Where(assigned.Add).ToList();
        }
    }

    private static void AutoAssignRuleBasedBoxes(DesktopLayout layout, IReadOnlyList<DesktopItem> desktopItems)
    {
        var assigned = layout.GetAssignedPaths().ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var item in desktopItems)
        {
            if (assigned.Contains(item.NormalizedPath))
            {
                continue;
            }

            var target = layout.SmartBoxes.FirstOrDefault(box => box.Matches(item));
            if (target is not null && target.AssignItem(item))
            {
                assigned.Add(item.NormalizedPath);
            }
        }
    }

    private static void AssignCatchAll(DesktopLayout layout, IReadOnlyList<DesktopItem> desktopItems)
    {
        var catchAll = layout.SmartBoxes.FirstOrDefault(box => box.Behavior == SmartBoxBehavior.CatchAll);
        if (catchAll is null)
        {
            return;
        }

        var assignedOutsideCatchAll = layout.SmartBoxes
            .Where(box => box.Behavior != SmartBoxBehavior.CatchAll)
            .SelectMany(box => box.ItemPaths)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        catchAll.ItemPaths.Clear();
        foreach (var item in desktopItems.Where(item => !assignedOutsideCatchAll.Contains(item.NormalizedPath)))
        {
            catchAll.AssignItem(item);
        }
    }
}
