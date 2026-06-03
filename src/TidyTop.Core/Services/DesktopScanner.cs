using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

/// <summary>
/// Best-effort scanner for the user and public Windows desktop folders.
/// </summary>
public sealed class DesktopScanner : IDesktopScanner
{
    public Task<IReadOnlyList<DesktopItem>> ScanAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Scan(cancellationToken), cancellationToken);
    }

    private static IReadOnlyList<DesktopItem> Scan(CancellationToken cancellationToken)
    {
        var items = new Dictionary<string, DesktopItem>(StringComparer.OrdinalIgnoreCase);

        foreach (var folder in GetDesktopFolders())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!Directory.Exists(folder))
            {
                continue;
            }

            foreach (var path in Directory.EnumerateFileSystemEntries(folder))
            {
                cancellationToken.ThrowIfCancellationRequested();
                TryAddItem(path, items);
            }
        }

        return items.Values
            .OrderBy(item => item.Type == DesktopItemType.Folder ? 0 : 1)
            .ThenBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static IEnumerable<string> GetDesktopFolders()
    {
        return new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)
            }
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }

    private static void TryAddItem(string path, IDictionary<string, DesktopItem> items)
    {
        try
        {
            var attributes = File.GetAttributes(path);
            if (attributes.HasFlag(FileAttributes.Hidden) || attributes.HasFlag(FileAttributes.System))
            {
                return;
            }

            var item = DesktopItem.FromFileSystemEntry(path);
            items[item.NormalizedPath] = item;
        }
        catch
        {
            // Desktop scans must be resilient. One inaccessible shortcut must not crash the app.
        }
    }
}
