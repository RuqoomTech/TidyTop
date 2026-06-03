using System;
using System.Collections.Generic;
using System.Drawing;

namespace TidyTop.Core.Models
{
    /// <summary>
    /// Represents a smart box that can automatically organize desktop icons
    /// </summary>
    public class SmartBox
    {
        /// <summary>
        /// Gets or sets the unique identifier for the box
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the title of the box
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the category this box represents
        /// </summary>
        public ApplicationCategory? Category { get; set; }

        /// <summary>
        /// Gets or sets the position of the box on the desktop
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// Gets or sets the size of the box
        /// </summary>
        public Size Size { get; set; } = new Size(200, 150);

        /// <summary>
        /// Gets or sets whether the box is visible
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the box is collapsed
        /// </summary>
        public bool IsCollapsed { get; set; } = false;

        /// <summary>
        /// Gets or sets whether the box automatically organizes icons
        /// </summary>
        public bool AutoOrganize { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the box is locked (cannot be moved or resized)
        /// </summary>
        public bool IsLocked { get; set; } = false;

        /// <summary>
        /// Gets or sets the background color of the box
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.FromArgb(200, 255, 255, 255);

        /// <summary>
        /// Gets or sets the border color of the box
        /// </summary>
        public Color BorderColor { get; set; } = Color.FromArgb(100, 0, 0, 0);

        /// <summary>
        /// Gets or sets the title color of the box
        /// </summary>
        public Color TitleColor { get; set; } = Color.FromArgb(240, 248, 255);

        /// <summary>
        /// Gets or sets the text color
        /// </summary>
        public Color TextColor { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets the border width
        /// </summary>
        public int BorderWidth { get; set; } = 1;

        /// <summary>
        /// Gets or sets the corner radius for rounded corners
        /// </summary>
        public int CornerRadius { get; set; } = 8;

        /// <summary>
        /// Gets or sets the opacity of the box (0.0 to 1.0)
        /// </summary>
        public double Opacity { get; set; } = 0.85;

        /// <summary>
        /// Gets or sets the icons contained in this box
        /// </summary>
        public List<DesktopIcon> Icons { get; set; } = new();

        /// <summary>
        /// Gets or sets the layout type for icons within the box
        /// </summary>
        public BoxLayout Layout { get; set; } = BoxLayout.Grid;

        /// <summary>
        /// Gets or sets the icon size within the box
        /// </summary>
        public IconSize IconSize { get; set; } = IconSize.Medium;

        /// <summary>
        /// Gets or sets the icon spacing
        /// </summary>
        public int IconSpacing { get; set; } = 5;

        /// <summary>
        /// Gets or sets the sort order for icons
        /// </summary>
        public SortOrder SortOrder { get; set; } = SortOrder.Name;

        /// <summary>
        /// Gets or sets whether to show the box title
        /// </summary>
        public bool ShowTitle { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to show the category icon
        /// </summary>
        public bool ShowCategoryIcon { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to show the icon count
        /// </summary>
        public bool ShowIconCount { get; set; } = true;

        /// <summary>
        /// Gets or sets the date when the box was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the date when the box was last modified
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets custom rules for this box (implementation coming soon)
        /// </summary>
        public List<string> CustomRules { get; set; } = new();

        /// <summary>
        /// Gets or sets the maximum number of icons to display (0 = unlimited)
        /// </summary>
        public int MaxIcons { get; set; } = 0;

        /// <summary>
        /// Gets or sets whether to show overflow indicator when max icons exceeded
        /// </summary>
        public bool ShowOverflow { get; set; } = true;

        /// <summary>
        /// Gets or sets the animation settings for this box
        /// </summary>
        public BoxAnimationSettings AnimationSettings { get; set; } = new();

        /// <summary>
        /// Gets the formatted title with icon count
        /// </summary>
        public string FormattedTitle
        {
            get
            {
                var title = Title;
                if (ShowCategoryIcon && Category != null)
                {
                    title = $"{Category.Icon} {title}";
                }
                if (ShowIconCount)
                {
                    title += $" ({Icons.Count})";
                }
                return title;
            }
        }

        /// <summary>
        /// Checks if an icon matches this box's organization criteria
        /// </summary>
        public bool MatchesIcon(DesktopIcon icon)
        {
            if (Category == null || !AutoOrganize)
                return false;

            // Check file extension
            if (Category.FileExtensions.Contains(icon.Extension.ToLowerInvariant()))
                return true;

            // Check application patterns
            var iconName = icon.Name.ToLowerInvariant();
            var iconPath = icon.FullPath.ToLowerInvariant();
            
            foreach (var pattern in Category.ApplicationPatterns)
            {
                if (iconName.Contains(pattern.ToLowerInvariant()) || 
                    iconPath.Contains(pattern.ToLowerInvariant()))
                    return true;
            }

            // Check keywords (in file name or path)
            foreach (var keyword in Category.Keywords)
            {
                if (iconName.Contains(keyword.ToLowerInvariant()) || 
                    iconPath.Contains(keyword.ToLowerInvariant()))
                    return true;
            }

            // Check custom rules (implementation coming soon)
            // foreach (var rule in CustomRules)
            // {
            //     if (rule.Matches(icon))
            //         return true;
            // }

            return false;
        }

        /// <summary>
        /// Adds an icon to this box
        /// </summary>
        public void AddIcon(DesktopIcon icon)
        {
            if (!Icons.Contains(icon))
            {
                Icons.Add(icon);
                icon.SmartBoxId = Id;
                ModifiedDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Removes an icon from this box
        /// </summary>
        public void RemoveIcon(DesktopIcon icon)
        {
            if (Icons.Remove(icon))
            {
                icon.SmartBoxId = null;
                ModifiedDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Organizes icons according to the current sort order
        /// </summary>
        public void OrganizeIcons()
        {
            Icons.Sort((x, y) => SortOrder switch
            {
                SortOrder.Name => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase),
                SortOrder.Type => string.Compare(x.Extension, y.Extension, StringComparison.OrdinalIgnoreCase),
                SortOrder.DateCreated => x.CreatedDate.CompareTo(y.CreatedDate),
                SortOrder.DateModified => x.ModifiedDate.CompareTo(y.ModifiedDate),
                SortOrder.Size => x.FileSize.CompareTo(y.FileSize),
                _ => 0
            });
            
            ModifiedDate = DateTime.Now;
        }
    }

    /// <summary>
    /// Box layout options
    /// </summary>
    public enum BoxLayout
    {
        Grid,
        Horizontal,
        Vertical,
        Freeform
    }

    /// <summary>
    /// Icon size options
    /// </summary>
    public enum IconSize
    {
        Small,
        Medium,
        Large,
        ExtraLarge
    }

    /// <summary>
    /// Sort order options
    /// </summary>
    public enum SortOrder
    {
        Name,
        Type,
        DateCreated,
        DateModified,
        Size,
        Custom
    }

    /// <summary>
    /// Animation settings for boxes
    /// </summary>
    public class BoxAnimationSettings
    {
        public bool EnableAnimations { get; set; } = true;
        public int AnimationDuration { get; set; } = 300;
        public AnimationType ExpandCollapseAnimation { get; set; } = AnimationType.Slide;
        public AnimationType IconEnterAnimation { get; set; } = AnimationType.FadeIn;
        public AnimationType IconExitAnimation { get; set; } = AnimationType.FadeOut;
    }

    /// <summary>
    /// Animation types
    /// </summary>
    public enum AnimationType
    {
        None,
        Fade,
        FadeIn,
        FadeOut,
        Slide,
        SlideUp,
        SlideDown,
        SlideLeft,
        SlideRight,
        Scale,
        Bounce
    }
}
