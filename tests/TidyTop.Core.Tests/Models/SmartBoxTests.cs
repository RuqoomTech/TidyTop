using TidyTop.Core.Models;

namespace TidyTop.Core.Tests.Models;

public class SmartBoxTests
{
    [Fact]
    public void AssignPath_DeduplicatesNormalizedPaths()
    {
        var box = new SmartBox { Title = "Work" };

        Assert.True(box.AssignPath(@"C:\Users\Test\Desktop\Report.pdf"));
        Assert.False(box.AssignPath(@"c:\users\test\desktop\report.pdf"));
        Assert.Single(box.ItemPaths);
    }

    [Fact]
    public void Matches_ReturnsFalseForManualBox()
    {
        var box = new SmartBox
        {
            Behavior = SmartBoxBehavior.Manual,
            Rules = { new SmartBoxRule { Kind = SmartBoxRuleKind.Extension, Value = ".pdf" } }
        };

        var item = new DesktopItem { Name = "Report", Extension = ".pdf" };

        Assert.False(box.Matches(item));
    }
}
