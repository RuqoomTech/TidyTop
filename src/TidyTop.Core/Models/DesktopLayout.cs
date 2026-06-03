using System;
using System.Collections.Generic;
using System.Drawing;

namespace TidyTop.Core.Models;

/// <summary>
/// Represents a saved desktop arrangement.
/// </summary>
public class DesktopLayout
{
    /// <summary>
    /// Gets or sets the unique identifier for the layout.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the layout name.
    /// </summary>
    public string Name { get; set; } = "Default Layout";

    /// <summary>
    /// Gets or sets the layout description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SmartBoxes saved in this layout.
    /// </summary>
    public List<SmartBox> SmartBoxes { get; set; } = new();

    /// <summary>
    /// Gets or sets desktop items not assigned to any SmartBox.
    /// </summary>
    public List<DesktopIcon> UnboxedIcons { get; set; } = new();

    /// <summary>
    /// Gets or sets the desktop resolution when this layout was saved.
    /// </summary>
    public Size DesktopResolution { get; set; } = new(1920, 1080);

    /// <summary>
    /// Gets or sets the date when this layout was created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the date when this layout was last modified.
    /// </summary>
    public DateTime ModifiedDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets whether this is the default layout.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Gets or sets the application version that created this layout.
    /// </summary>
    public string Version { get; set; } = "0.1.0";

    /// <summary>
    /// Gets or sets layout-level settings.
    /// </summary>
    public DesktopSettings Settings { get; set; } = new();

    /// <summary>
    /// Creates a deep copy of this layout.
    /// </summary>
    public DesktopLayout Clone()
    {
        return new DesktopLayout
        {
            Id = Guid.NewGuid(),
            Name = $"{Name} (Copy)",
            Description = Description,
            SmartBoxes = SmartBoxes.ConvertAll(CloneSmartBox),
            UnboxedIcons = UnboxedIcons.ConvertAll(CloneIcon),
            DesktopResolution = DesktopResolution,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now,
            IsDefault = false,
            Version = Version,
            Settings = Settings.Clone()
        };
    }

    private static SmartBox CloneSmartBox(SmartBox smartBox)
    {
        return new SmartBox
        {
            Id = Guid.NewGuid().ToString(),
            Title = smartBox.Title,
            Category = smartBox.Category,
            Position = smartBox.Position,
            Size = smartBox.Size,
            IsVisible = smartBox.IsVisible,
            IsCollapsed = smartBox.IsCollapsed,
            AutoOrganize = smartBox.AutoOrganize,
            IsLocked = smartBox.IsLocked,
            BackgroundColor = smartBox.BackgroundColor,
            BorderColor = smartBox.BorderColor,
            TitleColor = smartBox.TitleColor,
            TextColor = smartBox.TextColor,
            BorderWidth = smartBox.BorderWidth,
            CornerRadius = smartBox.CornerRadius,
            Opacity = smartBox.Opacity,
            Icons = smartBox.Icons.ConvertAll(CloneIcon),
            Layout = smartBox.Layout,
            IconSize = smartBox.IconSize,
            IconSpacing = smartBox.IconSpacing,
            SortOrder = smartBox.SortOrder,
            ShowTitle = smartBox.ShowTitle,
            ShowCategoryIcon = smartBox.ShowCategoryIcon,
            ShowIconCount = smartBox.ShowIconCount,
            CreatedDate = smartBox.CreatedDate,
            ModifiedDate = smartBox.ModifiedDate,
            CustomRules = new List<string>(smartBox.CustomRules),
            MaxIcons = smartBox.MaxIcons,
            ShowOverflow = smartBox.ShowOverflow,
            AnimationSettings = smartBox.AnimationSettings
        };
    }

    private static DesktopIcon CloneIcon(DesktopIcon icon)
    {
        return new DesktopIcon
        {
            Name = icon.Name,
            Extension = icon.Extension,
            FullPath = icon.FullPath,
            Icon = icon.Icon,
            Position = icon.Position,
            IsShortcut = icon.IsShortcut,
            Size = icon.Size,
            SmartBoxId = icon.SmartBoxId,
            CreatedDate = icon.CreatedDate,
            ModifiedDate = icon.ModifiedDate,
            FileSize = icon.FileSize,
            IsDirectory = icon.IsDirectory,
            IsVisible = icon.IsVisible
        };
    }
}
