using System.Security.Cryptography;
using System.Text;

namespace TidyTop.Core.Models;

/// <summary>
/// A file-system item discovered on the Windows desktop.
/// </summary>
public sealed class DesktopItem
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string FullPath { get; init; } = string.Empty;
    public string NormalizedPath { get; init; } = string.Empty;
    public string Extension { get; init; } = string.Empty;
    public DesktopItemType Type { get; init; } = DesktopItemType.File;
    public long SizeBytes { get; init; }
    public DateTimeOffset CreatedUtc { get; init; }
    public DateTimeOffset ModifiedUtc { get; init; }

    public bool IsShortcut => Type is DesktopItemType.Shortcut or DesktopItemType.UrlShortcut;
    public bool IsFolder => Type == DesktopItemType.Folder;

    public static DesktopItem FromFileSystemEntry(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Desktop item path cannot be empty.", nameof(path));
        }

        var attributes = File.GetAttributes(path);
        var isDirectory = attributes.HasFlag(FileAttributes.Directory);
        var extension = isDirectory ? string.Empty : Path.GetExtension(path).ToLowerInvariant();
        var normalizedPath = NormalizePath(path);

        var name = isDirectory ? Path.GetFileName(path) : Path.GetFileNameWithoutExtension(path);
        if (string.IsNullOrWhiteSpace(name))
        {
            name = path;
        }

        var info = isDirectory
            ? (FileSystemInfo)new DirectoryInfo(path)
            : new FileInfo(path);

        return new DesktopItem
        {
            Id = CreateStableId(normalizedPath),
            Name = name,
            FullPath = path,
            NormalizedPath = normalizedPath,
            Extension = extension,
            Type = GetItemType(isDirectory, extension),
            SizeBytes = isDirectory ? 0 : ((FileInfo)info).Length,
            CreatedUtc = info.CreationTimeUtc,
            ModifiedUtc = info.LastWriteTimeUtc
        };
    }

    public static string NormalizePath(string path)
    {
        return Path.GetFullPath(path.Trim()).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToUpperInvariant();
    }

    private static DesktopItemType GetItemType(bool isDirectory, string extension)
    {
        if (isDirectory)
        {
            return DesktopItemType.Folder;
        }

        return extension switch
        {
            ".lnk" => DesktopItemType.Shortcut,
            ".url" => DesktopItemType.UrlShortcut,
            _ => DesktopItemType.File
        };
    }

    private static string CreateStableId(string normalizedPath)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(normalizedPath));
        return Convert.ToHexString(bytes[..12]).ToLowerInvariant();
    }
}

public enum DesktopItemType
{
    File,
    Folder,
    Shortcut,
    UrlShortcut
}
