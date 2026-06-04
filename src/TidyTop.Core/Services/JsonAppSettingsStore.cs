using System.Text.Json;
using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

public sealed class JsonAppSettingsStore : IAppSettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private readonly AppDataPaths _paths;
    private readonly IAppLogger _logger;

    public JsonAppSettingsStore(AppDataPaths paths, IAppLogger? logger = null)
    {
        _paths = paths;
        _logger = logger ?? NullAppLogger.Instance;
    }

    public async Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        var settings = await TryLoadFromPathAsync(_paths.SettingsFilePath, cancellationToken);
        if (settings is not null)
        {
            return settings;
        }

        if (File.Exists(_paths.SettingsBackupFilePath))
        {
            _logger.Warning("Primary settings file was missing or invalid. Trying backup settings file.");
            settings = await TryLoadFromPathAsync(_paths.SettingsBackupFilePath, cancellationToken);
            if (settings is not null)
            {
                _logger.Info("Recovered settings from backup file.");
                return settings;
            }
        }

        return new AppSettings();
    }

    public async Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);

        _paths.EnsureCreated();
        settings.UpdatedUtc = DateTimeOffset.UtcNow;

        await AtomicJsonFile.WriteAsync(
            _paths.SettingsFilePath,
            _paths.SettingsBackupFilePath,
            settings,
            JsonOptions,
            _logger,
            cancellationToken);
    }

    private async Task<AppSettings?> TryLoadFromPathAsync(string path, CancellationToken cancellationToken)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            await using var stream = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<AppSettings>(stream, JsonOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.Error($"Could not load settings file: {path}", ex);
            return null;
        }
    }
}
