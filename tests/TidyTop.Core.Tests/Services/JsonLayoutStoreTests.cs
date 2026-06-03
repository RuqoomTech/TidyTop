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
}
