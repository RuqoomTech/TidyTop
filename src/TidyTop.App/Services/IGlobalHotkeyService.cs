namespace TidyTop.App.Services;

public interface IGlobalHotkeyService : IDisposable
{
    event EventHandler? ToggleRequested;

    bool IsSupported { get; }
    bool IsRunning { get; }

    void Start();
    void Stop();
}
