namespace TidyTop.Core.Models;

/// <summary>
/// The persisted state of the user's TidyTop organization.
/// </summary>
public sealed class DesktopLayout
{
    public const string CurrentSchemaVersion = "1.0";

    public string SchemaVersion { get; set; } = CurrentSchemaVersion;
    public string Name { get; set; } = "Default";
    public List<SmartBox> SmartBoxes { get; set; } = new();
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;

    public int AssignedItemCount => SmartBoxes.Sum(box => box.ItemPaths.Count);

    public SmartBox? FindBox(Guid id)
    {
        return SmartBoxes.FirstOrDefault(box => box.Id == id);
    }

    public IEnumerable<string> GetAssignedPaths()
    {
        return SmartBoxes.SelectMany(box => box.ItemPaths).Distinct(StringComparer.OrdinalIgnoreCase);
    }

    public DesktopLayout Clone(string? name = null)
    {
        return new DesktopLayout
        {
            SchemaVersion = SchemaVersion,
            Name = string.IsNullOrWhiteSpace(name) ? $"{Name} Copy" : name.Trim(),
            SmartBoxes = SmartBoxes.Select(CloneBox).ToList(),
            CreatedUtc = DateTimeOffset.UtcNow,
            UpdatedUtc = DateTimeOffset.UtcNow
        };
    }

    private static SmartBox CloneBox(SmartBox box)
    {
        return new SmartBox
        {
            Id = Guid.NewGuid(),
            Title = box.Title,
            Emoji = box.Emoji,
            AccentColor = box.AccentColor,
            Behavior = box.Behavior,
            X = box.X,
            Y = box.Y,
            Width = box.Width,
            Height = box.Height,
            IsVisible = box.IsVisible,
            IsCollapsed = box.IsCollapsed,
            IsLocked = box.IsLocked,
            IsSystemBox = box.IsSystemBox,
            Rules = box.Rules.Select(rule => new SmartBoxRule
            {
                Kind = rule.Kind,
                Value = rule.Value,
                IsEnabled = rule.IsEnabled
            }).ToList(),
            ItemPaths = box.ItemPaths.ToList(),
            CreatedUtc = box.CreatedUtc,
            UpdatedUtc = DateTimeOffset.UtcNow
        };
    }
}
