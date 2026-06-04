namespace TidyTop.Core.Services;

/// <summary>
/// Centralizes all user-writable TidyTop paths.
/// </summary>
public sealed class AppDataPaths
{
    public AppDataPaths(string rootDirectory)
    {
        if (string.IsNullOrWhiteSpace(rootDirectory))
        {
            throw new ArgumentException("Root directory cannot be empty.", nameof(rootDirectory));
        }

        RootDirectory = rootDirectory;
        LayoutFilePath = Path.Combine(rootDirectory, "layout.json");
        LayoutBackupFilePath = Path.Combine(rootDirectory, "layout.json.bak");
        SettingsFilePath = Path.Combine(rootDirectory, "settings.json");
        SettingsBackupFilePath = Path.Combine(rootDirectory, "settings.json.bak");
        LogsDirectoryPath = Path.Combine(rootDirectory, "logs");
        LogFilePath = Path.Combine(LogsDirectoryPath, "tidytop.log");
    }

    public string RootDirectory { get; }
    public string LayoutFilePath { get; }
    public string LayoutBackupFilePath { get; }
    public string SettingsFilePath { get; }
    public string SettingsBackupFilePath { get; }
    public string LogsDirectoryPath { get; }
    public string LogFilePath { get; }

    public static AppDataPaths CreateDefault()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return new AppDataPaths(Path.Combine(appData, "TidyTop"));
    }

    public void EnsureCreated()
    {
        Directory.CreateDirectory(RootDirectory);
    }
}
