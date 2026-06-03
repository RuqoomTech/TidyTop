using TidyTop.Core.Models;

namespace TidyTop.App.ViewModels;

public sealed class DesktopItemViewModel
{
    public DesktopItemViewModel(DesktopItem item)
    {
        Item = item;
    }

    public DesktopItem Item { get; }
    public string Name => Item.Name;
    public string FullPath => Item.FullPath;
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
}
