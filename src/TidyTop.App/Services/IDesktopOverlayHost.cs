using Avalonia.Controls;

namespace TidyTop.App.Services;

/// <summary>
/// Hosts the main TidyTop window as a desktop-level surface instead of a normal application window.
/// </summary>
public interface IDesktopOverlayHost
{
    /// <summary>
    /// Attempts to attach the supplied window to the Windows desktop host.
    /// Implementations must fail safely and leave the window usable when the desktop host cannot be found.
    /// </summary>
    void AttachToDesktop(Window window);
}
