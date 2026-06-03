using TidyTop.Core.Models;

namespace TidyTop.Core.Tests.Models;

public class DesktopLayoutTests
{
    [Fact]
    public void Clone_CreatesNewLayoutIdAndCopiesSmartBoxes()
    {
        var original = new DesktopLayout
        {
            Name = "Main",
            SmartBoxes =
            {
                new SmartBox { Id = Guid.NewGuid().ToString(), Title = "Work" }
            }
        };

        var clone = original.Clone();

        Assert.NotEqual(original.Id, clone.Id);
        Assert.Equal("Main (Copy)", clone.Name);
        Assert.Single(clone.SmartBoxes);
        Assert.NotEqual(original.SmartBoxes[0].Id, clone.SmartBoxes[0].Id);
    }
}
