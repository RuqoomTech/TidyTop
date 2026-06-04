using System.IO;
using TidyTop.Core.Models;

namespace TidyTop.App.ViewModels;

public sealed class DesktopItemViewModel
{
    public DesktopItemViewModel(DesktopItem item, Guid smartBoxId, string smartBoxTitle)
    {
        Item = item;
        SmartBoxId = smartBoxId;
        SmartBoxTitle = smartBoxTitle;
    }

    public DesktopItem Item { get; }
    public Guid SmartBoxId { get; }
    public string SmartBoxTitle { get; }
    public string Name => Item.Name;
    public string FullPath => Item.FullPath;
    public string NormalizedPath => Item.NormalizedPath;
    public string Extension => Item.Extension;
    public string Icon => Item.Type switch
    {
        DesktopItemType.Folder => "📁",
        DesktopItemType.Shortcut => "↗️",
        DesktopItemType.UrlShortcut => "🌐",
        _ => "📄"
    };

    public string Subtitle => Item.Type switch
    {
        DesktopItemType.Folder => "Folder",
        DesktopItemType.Shortcut => "Shortcut",
        DesktopItemType.UrlShortcut => "Web shortcut",
        _ when string.IsNullOrWhiteSpace(Item.Extension) => "File",
        _ => Item.Extension.TrimStart('.').ToUpperInvariant()
    };

    public string LocationName
    {
        get
        {
            var directory = Path.GetDirectoryName(Item.FullPath);
            if (string.IsNullOrWhiteSpace(directory))
            {
                return "Desktop";
            }

            var name = Path.GetFileName(directory);
            return string.IsNullOrWhiteSpace(name) ? "Desktop" : name;
        }
    }

    public string DetailsText => $"{Subtitle} • {LocationName}";
}
