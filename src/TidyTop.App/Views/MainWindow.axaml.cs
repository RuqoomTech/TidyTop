using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TidyTop.App.Views;

public partial class MainWindow : Window
{
    private readonly List<DesktopEntry> _desktopEntries = new();
    private readonly Dictionary<string, CategoryDefinition> _categories = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, List<DesktopEntry>> _groupedEntries = new(StringComparer.OrdinalIgnoreCase);

    public MainWindow()
    {
        InitializeComponent();
        InitializeCategories();
        _ = LoadDesktopEntriesAsync();
    }

    private void InitializeCategories()
    {
        _categories["office"] = new CategoryDefinition(
            "Office & Documents",
            new[] { ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf", ".odt", ".ods", ".odp" },
            new[] { "word", "excel", "powerpoint", "office", "acrobat", "reader", "document", "spreadsheet", "presentation" });

        _categories["games"] = new CategoryDefinition(
            "Games",
            new[] { ".exe" },
            new[] { "steam", "epic", "game", "games", "minecraft", "blizzard", "origin", "uplay", "gog" });

        _categories["social"] = new CategoryDefinition(
            "Web & Communication",
            new[] { ".url", ".html", ".htm" },
            new[] { "discord", "telegram", "whatsapp", "skype", "zoom", "teams", "slack", "outlook", "thunderbird", "chrome", "firefox", "edge", "browser" });

        _categories["files"] = new CategoryDefinition(
            "Files & Folders",
            new[] { ".txt", ".rtf", ".md", ".zip", ".rar", ".7z", ".png", ".jpg", ".jpeg", ".gif", ".mp4", ".mov", ".avi" },
            new[] { "folder", "archive", "file", "notepad", "explorer", "winrar", "7zip" });
    }

    private async Task LoadDesktopEntriesAsync()
    {
        try
        {
            LoadingOverlay.IsVisible = true;
            StatusText.Text = "Scanning desktop...";

            var entries = await Task.Run(ScanDesktopEntries);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _desktopEntries.Clear();
                _desktopEntries.AddRange(entries);
                GroupEntries();
                RenderGroups();
                LoadingOverlay.IsVisible = false;
            });
        }
        catch (Exception ex)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                LoadingOverlay.IsVisible = false;
                StatusText.Text = $"Scan failed: {ex.Message}";
            });
        }
    }

    private static List<DesktopEntry> ScanDesktopEntries()
    {
        var results = new Dictionary<string, DesktopEntry>(StringComparer.OrdinalIgnoreCase);
        var folders = new[]
        {
            Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)
        };

        foreach (var folder in folders.Where(path => !string.IsNullOrWhiteSpace(path)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!Directory.Exists(folder))
            {
                continue;
            }

            foreach (var path in Directory.EnumerateFileSystemEntries(folder).Take(250))
            {
                TryAddEntry(results, path);
            }
        }

        return results.Values.OrderBy(entry => entry.Name, StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static void TryAddEntry(IDictionary<string, DesktopEntry> results, string path)
    {
        try
        {
            var attributes = File.GetAttributes(path);
            if (attributes.HasFlag(FileAttributes.Hidden) || attributes.HasFlag(FileAttributes.System))
            {
                return;
            }

            var isDirectory = attributes.HasFlag(FileAttributes.Directory);
            var extension = isDirectory ? string.Empty : Path.GetExtension(path).ToLowerInvariant();
            var name = isDirectory ? Path.GetFileName(path) : Path.GetFileNameWithoutExtension(path);

            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            results[path] = new DesktopEntry(
                name,
                path,
                extension,
                isDirectory,
                !isDirectory && (extension.Equals(".lnk", StringComparison.OrdinalIgnoreCase) || extension.Equals(".url", StringComparison.OrdinalIgnoreCase)));
        }
        catch
        {
            // Desktop scans should be best effort. A single inaccessible item must not break the UI.
        }
    }

    private void GroupEntries()
    {
        _groupedEntries.Clear();
        foreach (var key in _categories.Keys.Append("other"))
        {
            _groupedEntries[key] = new List<DesktopEntry>();
        }

        foreach (var entry in _desktopEntries)
        {
            var category = Categorize(entry);
            _groupedEntries[category].Add(entry);
        }
    }

    private string Categorize(DesktopEntry entry)
    {
        if (entry.IsDirectory)
        {
            return "files";
        }

        foreach (var category in _categories)
        {
            if (category.Value.Matches(entry))
            {
                return category.Key;
            }
        }

        return "other";
    }

    private void RenderGroups()
    {
        RenderGroup("office", OfficeItemsPanel, OfficeCountText);
        RenderGroup("games", GamesItemsPanel, GamesCountText);
        RenderGroup("social", SocialItemsPanel, SocialCountText);
        RenderGroup("files", FilesItemsPanel, FilesCountText);
        RenderGroup("other", OtherItemsPanel, OtherCountText);

        var organized = _groupedEntries.Where(pair => pair.Key != "other").Sum(pair => pair.Value.Count);
        StatusText.Text = $"{organized}/{_desktopEntries.Count} desktop items grouped. Drag/drop, custom boxes, and persistence are next MVP tasks.";
    }

    private void RenderGroup(string key, StackPanel panel, TextBlock countText)
    {
        panel.Children.Clear();
        var entries = _groupedEntries.TryGetValue(key, out var group) ? group : new List<DesktopEntry>();
        countText.Text = entries.Count.ToString();

        if (entries.Count == 0)
        {
            panel.Children.Add(CreateEmptyState());
            return;
        }

        foreach (var entry in entries.Take(40))
        {
            panel.Children.Add(CreateEntryRow(entry));
        }

        if (entries.Count > 40)
        {
            panel.Children.Add(new TextBlock
            {
                Text = $"+ {entries.Count - 40} more items",
                Foreground = new SolidColorBrush(Color.Parse("#FF9EAAB4")),
                FontSize = 12,
                Margin = new Avalonia.Thickness(4, 8, 4, 0)
            });
        }
    }

    private static Control CreateEntryRow(DesktopEntry entry)
    {
        var icon = entry.IsDirectory ? "📁" : entry.IsShortcut ? "↗️" : "📄";
        var text = new TextBlock
        {
            Text = $"{icon}  {entry.Name}",
            TextTrimming = TextTrimming.CharacterEllipsis,
            Foreground = Brushes.White,
            FontSize = 13,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };

        ToolTip.SetTip(text, entry.FullPath);

        return new Border
        {
            Background = new SolidColorBrush(Color.Parse("#1AFFFFFF")),
            CornerRadius = new Avalonia.CornerRadius(8),
            Padding = new Avalonia.Thickness(9, 7),
            Child = text
        };
    }

    private static Control CreateEmptyState()
    {
        return new TextBlock
        {
            Text = "No matching desktop items yet.",
            Foreground = new SolidColorBrush(Color.Parse("#FF9EAAB4")),
            FontSize = 12,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Avalonia.Thickness(4)
        };
    }

    private async void RefreshButton_Click(object? sender, RoutedEventArgs e)
    {
        await LoadDesktopEntriesAsync();
    }

    private void AddBoxButton_Click(object? sender, RoutedEventArgs e)
    {
        StatusText.Text = "Add SmartBox is planned for Milestone 2. The current build focuses on real desktop scan and display.";
    }

    private async void SettingsButton_Click(object? sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow();
        await settingsWindow.ShowDialog(this);
    }

    private sealed record DesktopEntry(string Name, string FullPath, string Extension, bool IsDirectory, bool IsShortcut);

    private sealed record CategoryDefinition(string Name, string[] Extensions, string[] Keywords)
    {
        public bool Matches(DesktopEntry entry)
        {
            if (Extensions.Contains(entry.Extension, StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }

            var haystack = $"{entry.Name} {entry.FullPath}".ToLowerInvariant();
            return Keywords.Any(keyword => haystack.Contains(keyword.ToLowerInvariant(), StringComparison.Ordinal));
        }
    }
}
