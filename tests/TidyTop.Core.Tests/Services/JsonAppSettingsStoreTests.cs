using TidyTop.Core.Models;
using TidyTop.Core.Services;

namespace TidyTop.Core.Tests.Services;

public class JsonAppSettingsStoreTests
{
    [Fact]
    public async Task SaveAndLoadAsync_RoundTripsSettings()
    {
        var root = Path.Combine(Path.GetTempPath(), "TidyTopTests", Guid.NewGuid().ToString("N"));
        var store = new JsonAppSettingsStore(new AppDataPaths(root));
        var settings = new AppSettings
        {
            StartHidden = true,
            EnableDesktopOverlayHost = false,
            EnableNativeDesktopIconControl = false,
            HideNativeDesktopIcons = false,
            EnableGlobalHotkey = false,
            GlobalHotkey = "Ctrl+Alt+T"
        };

        await store.SaveAsync(settings);
        var loaded = await store.LoadAsync();

        Assert.True(loaded.StartHidden);
        Assert.False(loaded.EnableDesktopOverlayHost);
        Assert.False(loaded.EnableNativeDesktopIconControl);
        Assert.False(loaded.EnableGlobalHotkey);
    }

    [Fact]
    public async Task LoadAsync_UsesBackupWhenPrimarySettingsAreBroken()
    {
        var root = Path.Combine(Path.GetTempPath(), "TidyTopTests", Guid.NewGuid().ToString("N"));
        var paths = new AppDataPaths(root);
        var store = new JsonAppSettingsStore(paths);

        await store.SaveAsync(new AppSettings { StartHidden = true });
        await store.SaveAsync(new AppSettings { StartHidden = false });
        await File.WriteAllTextAsync(paths.SettingsFilePath, "{ broken json");

        var loaded = await store.LoadAsync();

        Assert.True(loaded.StartHidden);
    }
}
