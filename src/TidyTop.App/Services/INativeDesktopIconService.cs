namespace TidyTop.App.Services;

public interface INativeDesktopIconService
{
    bool IsSupported { get; }

    void CaptureInitialState();
    bool AreIconsVisible();
    void SetIconsVisible(bool visible);
    void RestoreCapturedState();
}
