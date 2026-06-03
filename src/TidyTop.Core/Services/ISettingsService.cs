using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

/// <summary>
/// Service for loading, saving, and resetting TidyTop settings.
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Gets current settings. If no settings file exists, defaults are returned.
    /// </summary>
    Task<DesktopSettings> GetSettingsAsync();

    /// <summary>
    /// Saves settings to the default settings file.
    /// </summary>
    Task<bool> SaveSettingsAsync(DesktopSettings settings);

    /// <summary>
    /// Resets settings to defaults.
    /// </summary>
    Task<bool> ResetSettingsAsync();

    /// <summary>
    /// Loads settings from a provided file path and saves them as current settings.
    /// </summary>
    Task<bool> LoadSettingsFromFileAsync(string filePath);

    /// <summary>
    /// Exports current settings to a provided file path.
    /// </summary>
    Task<bool> SaveSettingsToFileAsync(string filePath);

    /// <summary>
    /// Raised when settings are changed.
    /// </summary>
    event EventHandler<DesktopSettings>? SettingsChanged;
}
