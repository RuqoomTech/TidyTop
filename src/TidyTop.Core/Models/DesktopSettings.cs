using System;
using System.Collections.Generic;
using System.Drawing;

namespace TidyTop.Core.Models;

/// <summary>
/// Global TidyTop settings.
/// </summary>
public class DesktopSettings
{
    public Color DefaultSmartBoxBackgroundColor { get; set; } = Color.FromArgb(200, 240, 240, 240);
    public Color DefaultSmartBoxBorderColor { get; set; } = Color.FromArgb(200, 180, 180, 180);
    public Color DefaultSmartBoxTitleColor { get; set; } = Color.Black;
    public double DefaultSmartBoxOpacity { get; set; } = 0.8;
    public Size DefaultIconSize { get; set; } = new(32, 32);
    public int DefaultIconSpacing { get; set; } = 5;
    public int DefaultSmartBoxCornerRadius { get; set; } = 4;
    public int DefaultSmartBoxBorderWidth { get; set; } = 1;
    public bool ShowSmartBoxTitles { get; set; } = true;
    public bool EnableQuickHide { get; set; } = true;
    public string QuickHideHotkey { get; set; } = "Ctrl+Space";
    public bool EnableAutoOrganize { get; set; } = true;
    public int AutoOrganizeInterval { get; set; } = 30;
    public bool StartWithWindows { get; set; }
    public bool ShowNotifications { get; set; } = true;
    public string Language { get; set; } = "en-US";
    public ApplicationTheme Theme { get; set; } = ApplicationTheme.System;
    public List<AutoOrganizeRule> AutoOrganizeRules { get; set; } = new();
    public DateTime LastModified { get; set; } = DateTime.Now;
    public bool EnableDesktopPortals { get; set; }
    public bool EnableAnimations { get; set; } = true;
    public int AnimationSpeed { get; set; } = 300;
    public bool EnableGridSnapping { get; set; } = true;
    public int GridSize { get; set; } = 10;

    /// <summary>
    /// Creates a deep copy of these settings.
    /// </summary>
    public DesktopSettings Clone()
    {
        return new DesktopSettings
        {
            DefaultSmartBoxBackgroundColor = DefaultSmartBoxBackgroundColor,
            DefaultSmartBoxBorderColor = DefaultSmartBoxBorderColor,
            DefaultSmartBoxTitleColor = DefaultSmartBoxTitleColor,
            DefaultSmartBoxOpacity = DefaultSmartBoxOpacity,
            DefaultIconSize = DefaultIconSize,
            DefaultIconSpacing = DefaultIconSpacing,
            DefaultSmartBoxCornerRadius = DefaultSmartBoxCornerRadius,
            DefaultSmartBoxBorderWidth = DefaultSmartBoxBorderWidth,
            ShowSmartBoxTitles = ShowSmartBoxTitles,
            EnableQuickHide = EnableQuickHide,
            QuickHideHotkey = QuickHideHotkey,
            EnableAutoOrganize = EnableAutoOrganize,
            AutoOrganizeInterval = AutoOrganizeInterval,
            StartWithWindows = StartWithWindows,
            ShowNotifications = ShowNotifications,
            Language = Language,
            Theme = Theme,
            AutoOrganizeRules = AutoOrganizeRules.ConvertAll(rule => rule.Clone()),
            LastModified = DateTime.Now,
            EnableDesktopPortals = EnableDesktopPortals,
            EnableAnimations = EnableAnimations,
            AnimationSpeed = AnimationSpeed,
            EnableGridSnapping = EnableGridSnapping,
            GridSize = GridSize
        };
    }
}

/// <summary>
/// Defines the application theme options.
/// </summary>
public enum ApplicationTheme
{
    System,
    Light,
    Dark
}

/// <summary>
/// Represents a rule for placing matching desktop items into a target SmartBox.
/// </summary>
public class AutoOrganizeRule
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public RuleType RuleType { get; set; } = RuleType.Extension;
    public string TargetSmartBoxId { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public int Priority { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public AutoOrganizeRule Clone()
    {
        return new AutoOrganizeRule
        {
            Id = Guid.NewGuid().ToString(),
            Name = Name,
            Condition = Condition,
            RuleType = RuleType,
            TargetSmartBoxId = TargetSmartBoxId,
            IsEnabled = IsEnabled,
            Priority = Priority,
            CreatedDate = CreatedDate
        };
    }
}

/// <summary>
/// Defines supported auto-organization rule types.
/// </summary>
public enum RuleType
{
    Extension,
    Name,
    DateCreated,
    DateModified,
    Size,
    Path
}
