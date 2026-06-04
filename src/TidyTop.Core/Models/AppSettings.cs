namespace TidyTop.Core.Models;

/// <summary>
/// Settings that are independent from a specific desktop layout.
/// These are intentionally conservative: TidyTop can hide/show its own overlay
/// and can hide the native Windows desktop icon view, but it never moves or deletes real files.
/// </summary>
public sealed class AppSettings
{
    public string Theme { get; set; } = "System";

    public bool StartHidden { get; set; }

    /// <summary>
    /// When true, TidyTop hides the native Windows desktop icon view while it is running.
    /// The real desktop files are not changed.
    /// </summary>
    public bool HideNativeDesktopIcons { get; set; }

    public bool EnableGlobalHotkey { get; set; } = true;
    public string GlobalHotkey { get; set; } = "Ctrl+Alt+T";

    public bool RunOnStartup { get; set; }
    public bool EnableAutoOrganizeOnRefresh { get; set; } = true;

    public DateTimeOffset UpdatedUtc { get; set; } = DateTimeOffset.UtcNow;
}
