using System.Text.Json;

namespace TidyTop.Core.Services;

internal static class AtomicJsonFile
{
    public static async Task WriteAsync<T>(
        string targetPath,
        string backupPath,
        T value,
        JsonSerializerOptions jsonOptions,
        IAppLogger logger,
        CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(targetPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var tempPath = $"{targetPath}.tmp";

        try
        {
            await using (var stream = File.Create(tempPath))
            {
                await JsonSerializer.SerializeAsync(stream, value, jsonOptions, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }

            if (File.Exists(targetPath))
            {
                File.Copy(targetPath, backupPath, overwrite: true);
            }

            File.Move(tempPath, targetPath, overwrite: true);
        }
        catch (Exception ex)
        {
            logger.Error($"Could not atomically write JSON file: {targetPath}", ex);

            try
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
            catch (Exception cleanupException)
            {
                logger.Error($"Could not delete temporary JSON file: {tempPath}", cleanupException);
            }

            throw;
        }
    }
}
