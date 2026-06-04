using TidyTop.Core.Models;
using TidyTop.Core.Services;

namespace TidyTop.Core.Tests.Services;

public class JsonLayoutStoreTests
{
    [Fact]
    public async Task SaveAndLoadAsync_RoundTripsLayout()
    {
        var root = Path.Combine(Path.GetTempPath(), "TidyTopTests", Guid.NewGuid().ToString("N"));
        var store = new JsonLayoutStore(new AppDataPaths(root));
        var layout = new DesktopLayout
        {
            Name = "Saved",
            SmartBoxes = { new SmartBox { Title = "Manual" } }
        };

        await store.SaveAsync(layout);
        var loaded = await store.LoadAsync();

        Assert.NotNull(loaded);
        Assert.Equal("Saved", loaded!.Name);
        Assert.Single(loaded.SmartBoxes);
    }
    [Fact]
    public async Task LoadAsync_UsesBackupWhenPrimaryLayoutIsBroken()
    {
        var root = Path.Combine(Path.GetTempPath(), "TidyTopTests", Guid.NewGuid().ToString("N"));
        var paths = new AppDataPaths(root);
        var store = new JsonLayoutStore(paths);

        await store.SaveAsync(new DesktopLayout { Name = "Backup" });
        await store.SaveAsync(new DesktopLayout { Name = "Primary" });
        await File.WriteAllTextAsync(paths.LayoutFilePath, "{ broken json");

        var loaded = await store.LoadAsync();

        Assert.NotNull(loaded);
        Assert.Equal("Backup", loaded!.Name);
    }

}
