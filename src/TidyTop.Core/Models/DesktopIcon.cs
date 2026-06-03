using System;
using System.Drawing;

namespace TidyTop.Core.Models;

/// <summary>
/// Represents a desktop item discovered by TidyTop.
/// </summary>
public class DesktopIcon
{
    /// <summary>
    /// Gets or sets the display name of the desktop item.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file extension. Folders use an empty extension.
    /// </summary>
    public string Extension { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full file system path.
    /// </summary>
    public string FullPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets extracted icon image data when available.
    /// </summary>
    public byte[]? Icon { get; set; }

    /// <summary>
    /// Gets or sets the desktop position when available.
    /// </summary>
    public Point Position { get; set; }

    /// <summary>
    /// Gets or sets whether this item is a shortcut.
    /// </summary>
    public bool IsShortcut { get; set; }

    /// <summary>
    /// Gets or sets the icon size used for rendering.
    /// </summary>
    public Size Size { get; set; } = new(32, 32);

    /// <summary>
    /// Gets or sets the SmartBox this item belongs to. Null means unboxed.
    /// </summary>
    public string? SmartBoxId { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the backing file/folder.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the last modified date of the backing file/folder.
    /// </summary>
    public DateTime ModifiedDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the file size in bytes. Folders use 0.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Gets or sets whether this desktop item is a folder.
    /// </summary>
    public bool IsDirectory { get; set; }

    /// <summary>
    /// Gets or sets whether this item should be visible in TidyTop.
    /// </summary>
    public bool IsVisible { get; set; } = true;
}
