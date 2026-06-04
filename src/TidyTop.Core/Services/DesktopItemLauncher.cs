using System.Diagnostics;
using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

/// <summary>
/// Opens desktop items through the OS shell so folders, files, .lnk shortcuts,
/// and .url shortcuts behave like they do on the native Windows desktop.
/// </summary>
public sealed class DesktopItemLauncher : IDesktopItemLauncher
{
    public Task LaunchAsync(DesktopItem item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        return LaunchAsync(item.FullPath, cancellationToken);
    }

    public Task LaunchAsync(string path, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Desktop item path cannot be empty.", nameof(path));
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (!File.Exists(path) && !Directory.Exists(path))
        {
            throw new FileNotFoundException("The desktop item no longer exists.", path);
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true,
            WorkingDirectory = GetWorkingDirectory(path)
        };

        Process.Start(startInfo);
        return Task.CompletedTask;
    }

    private static string GetWorkingDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            return path;
        }

        var directory = Path.GetDirectoryName(path);
        return string.IsNullOrWhiteSpace(directory)
            ? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
            : directory;
    }
}
