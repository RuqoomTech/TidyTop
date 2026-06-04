namespace TidyTop.Core.Models;

/// <summary>
/// A persisted desktop group. It stores item paths, not duplicated item objects.
/// </summary>
public sealed class SmartBox
{
    public const int MinimumWidth = 220;
    public const int MinimumHeight = 150;

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "New SmartBox";
    public string Emoji { get; set; } = "📦";
    public string AccentColor { get; set; } = "#3B82F6";
    public SmartBoxBehavior Behavior { get; set; } = SmartBoxBehavior.Manual;

    public int X { get; set; } = 24;
    public int Y { get; set; } = 24;
    public int Width { get; set; } = 320;
    public int Height { get; set; } = 260;

    public bool IsVisible { get; set; } = true;
    public bool IsCollapsed { get; set; }
    public bool IsLocked { get; set; }
    public bool IsSystemBox { get; set; }

    public List<SmartBoxRule> Rules { get; set; } = new();
    public List<string> ItemPaths { get; set; } = new();

    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;

    public string HeaderText => $"{Emoji} {Title} ({ItemPaths.Count})";

    public bool Matches(DesktopItem item)
    {
        return Behavior == SmartBoxBehavior.RuleBased && Rules.Any(rule => rule.Matches(item));
    }


    public void SetGeometry(int x, int y, int width, int height)
    {
        X = Math.Max(0, x);
        Y = Math.Max(0, y);
        Width = Math.Max(MinimumWidth, width);
        Height = Math.Max(MinimumHeight, height);
        UpdatedUtc = DateTimeOffset.UtcNow;
    }

    public bool ContainsItem(string path)
    {
        var normalized = DesktopItem.NormalizePath(path);
        return ItemPaths.Any(itemPath => string.Equals(itemPath, normalized, StringComparison.OrdinalIgnoreCase));
    }

    public bool AssignItem(DesktopItem item)
    {
        return AssignPath(item.NormalizedPath);
    }

    public bool AssignPath(string path)
    {
        var normalized = DesktopItem.NormalizePath(path);
        if (ItemPaths.Any(existing => string.Equals(existing, normalized, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        ItemPaths.Add(normalized);
        UpdatedUtc = DateTimeOffset.UtcNow;
        return true;
    }

    public bool RemovePath(string path)
    {
        var normalized = DesktopItem.NormalizePath(path);
        var removed = ItemPaths.RemoveAll(existing => string.Equals(existing, normalized, StringComparison.OrdinalIgnoreCase)) > 0;
        if (removed)
        {
            UpdatedUtc = DateTimeOffset.UtcNow;
        }

        return removed;
    }

    public void NormalizeAssignments(IReadOnlySet<string> knownItemPaths)
    {
        ItemPaths = ItemPaths
            .Select(DesktopItem.NormalizePath)
            .Where(knownItemPaths.Contains)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}

public enum SmartBoxBehavior
{
    Manual,
    RuleBased,
    CatchAll
}
