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

    public JsonLayoutStore(AppDataPaths paths)
    {
        _paths = paths;
    }

    public async Task<DesktopLayout?> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_paths.LayoutFilePath))
        {
            return null;
        }

        try
        {
            await using var stream = File.OpenRead(_paths.LayoutFilePath);
            return await JsonSerializer.DeserializeAsync<DesktopLayout>(stream, JsonOptions, cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task SaveAsync(DesktopLayout layout, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(layout);

        _paths.EnsureCreated();
        layout.SchemaVersion = DesktopLayout.CurrentSchemaVersion;
        layout.UpdatedUtc = DateTimeOffset.UtcNow;

        var tempPath = $"{_paths.LayoutFilePath}.tmp";
        await using (var stream = File.Create(tempPath))
        {
            await JsonSerializer.SerializeAsync(stream, layout, JsonOptions, cancellationToken);
        }

        if (File.Exists(_paths.LayoutFilePath))
        {
            File.Delete(_paths.LayoutFilePath);
        }

        File.Move(tempPath, _paths.LayoutFilePath);
    }

    public Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        if (File.Exists(_paths.LayoutFilePath))
        {
            File.Delete(_paths.LayoutFilePath);
        }

        return Task.CompletedTask;
    }
}
