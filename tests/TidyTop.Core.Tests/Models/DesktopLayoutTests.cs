using TidyTop.Core.Models;

namespace TidyTop.Core.Tests.Models;

public class DesktopLayoutTests
{
    [Fact]
    public void Clone_CreatesNewBoxIdsButKeepsAssignments()
    {
        var box = new SmartBox { Title = "Work" };
        box.AssignPath(@"C:\Desktop\Report.pdf");

        var layout = new DesktopLayout { Name = "Main", SmartBoxes = { box } };
        var clone = layout.Clone();

        Assert.Equal("Main Copy", clone.Name);
        Assert.NotEqual(layout.SmartBoxes[0].Id, clone.SmartBoxes[0].Id);
        Assert.Equal(layout.SmartBoxes[0].ItemPaths, clone.SmartBoxes[0].ItemPaths);
    }
}
