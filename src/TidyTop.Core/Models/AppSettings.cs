namespace TidyTop.Core.Models;

/// <summary>
/// Settings that are independent from a specific desktop layout.
/// </summary>
public sealed class AppSettings
{
    public string Theme { get; set; } = "System";
    public bool StartWithWindows { get; set; }
    public bool EnableQuickHide { get; set; } = true;
    public string QuickHideHotkey { get; set; } = "Ctrl+Space";
    public bool EnableAutoOrganizeOnRefresh { get; set; } = true;
    public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;
}
