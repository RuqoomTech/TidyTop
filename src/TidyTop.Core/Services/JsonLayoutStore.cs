using System.Text.Json;
using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

public sealed class JsonLayoutStore : ILayoutStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    private readonly AppDataPaths _paths;
    private readonly IAppLogger _logger;

    public JsonLayoutStore(AppDataPaths paths, IAppLogger? logger = null)
    {
        _paths = paths;
        _logger = logger ?? NullAppLogger.Instance;
    }

    public async Task<DesktopLayout?> LoadAsync(CancellationToken cancellationToken = default)
    {
        var layout = await TryLoadFromPathAsync(_paths.LayoutFilePath, cancellationToken);
        if (layout is not null)
        {
            return layout;
        }

        if (File.Exists(_paths.LayoutBackupFilePath))
        {
            _logger.Warning("Primary layout file was missing or invalid. Trying backup layout file.");
            layout = await TryLoadFromPathAsync(_paths.LayoutBackupFilePath, cancellationToken);
            if (layout is not null)
            {
                _logger.Info("Recovered layout from backup file.");
                return layout;
            }
        }

        return null;
    }

    public async Task SaveAsync(DesktopLayout layout, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(layout);

        _paths.EnsureCreated();
        layout.SchemaVersion = DesktopLayout.CurrentSchemaVersion;
        layout.UpdatedUtc = DateTimeOffset.UtcNow;

        await AtomicJsonFile.WriteAsync(
            _paths.LayoutFilePath,
            _paths.LayoutBackupFilePath,
            layout,
            JsonOptions,
            _logger,
            cancellationToken);
    }

    public Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (File.Exists(_paths.LayoutFilePath))
            {
                if (File.Exists(_paths.LayoutBackupFilePath))
                {
                    File.Delete(_paths.LayoutBackupFilePath);
                }

                File.Move(_paths.LayoutFilePath, _paths.LayoutBackupFilePath);
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Could not delete layout file safely.", ex);
            throw;
        }

        return Task.CompletedTask;
    }

    private async Task<DesktopLayout?> TryLoadFromPathAsync(string path, CancellationToken cancellationToken)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            await using var stream = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<DesktopLayout>(stream, JsonOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.Error($"Could not load layout file: {path}", ex);
            return null;
        }
    }
}
