namespace TidyTop.Core.Services;

public sealed class FileAppLogger : IAppLogger
{
    private readonly AppDataPaths _paths;
    private readonly object _gate = new();

    public FileAppLogger(AppDataPaths paths)
    {
        _paths = paths;
    }

    public void Info(string message) => Write("INFO", message, null);

    public void Warning(string message) => Write("WARN", message, null);

    public void Error(string message, Exception? exception = null) => Write("ERROR", message, exception);

    private void Write(string level, string message, Exception? exception)
    {
        try
        {
            _paths.EnsureCreated();
            Directory.CreateDirectory(_paths.LogsDirectoryPath);

            var line = $"{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff zzz} [{level}] {message}";
            if (exception is not null)
            {
                line += Environment.NewLine + exception;
            }

            lock (_gate)
            {
                File.AppendAllText(_paths.LogFilePath, line + Environment.NewLine);
            }
        }
        catch
        {
            // Logging must never crash the app.
        }
    }
}
