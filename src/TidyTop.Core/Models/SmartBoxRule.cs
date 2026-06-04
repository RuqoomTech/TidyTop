namespace TidyTop.Core.Models;

/// <summary>
/// A simple matching rule used by a rule-based SmartBox.
/// </summary>
public sealed class SmartBoxRule
{
    public SmartBoxRuleKind Kind { get; init; }
    public string Value { get; init; } = string.Empty;
    public bool IsEnabled { get; init; } = true;

    public bool Matches(DesktopItem item)
    {
        if (!IsEnabled || string.IsNullOrWhiteSpace(Value))
        {
            return false;
        }

        return Kind switch
        {
            SmartBoxRuleKind.Extension => string.Equals(item.Extension, NormalizeExtension(Value), StringComparison.OrdinalIgnoreCase),
            SmartBoxRuleKind.NameContains => item.Name.Contains(Value, StringComparison.OrdinalIgnoreCase),
            SmartBoxRuleKind.PathContains => item.FullPath.Contains(Value, StringComparison.OrdinalIgnoreCase),
            SmartBoxRuleKind.ItemType => Enum.TryParse<DesktopItemType>(Value, true, out var type) && item.Type == type,
            _ => false
        };
    }

    private static string NormalizeExtension(string extension)
    {
        extension = extension.Trim();
        return extension.StartsWith(".", StringComparison.Ordinal) ? extension.ToLowerInvariant() : $".{extension.ToLowerInvariant()}";
    }
}

public enum SmartBoxRuleKind
{
    Extension,
    NameContains,
    PathContains,
    ItemType
}
