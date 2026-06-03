using TidyTop.Core.Models;

namespace TidyTop.Core.Tests.Models;

public class SmartBoxRuleTests
{
    [Fact]
    public void Matches_ExtensionRule_IsCaseInsensitive()
    {
        var item = new DesktopItem
        {
            Name = "Report",
            FullPath = @"C:\Users\Test\Desktop\Report.PDF",
            NormalizedPath = @"C:\USERS\TEST\DESKTOP\REPORT.PDF",
            Extension = ".pdf"
        };

        var rule = new SmartBoxRule { Kind = SmartBoxRuleKind.Extension, Value = "PDF" };

        Assert.True(rule.Matches(item));
    }

    [Fact]
    public void Matches_NameContainsRule_IsCaseInsensitive()
    {
        var item = new DesktopItem { Name = "Visual Studio Code", FullPath = @"C:\Desktop\VSCode.lnk" };
        var rule = new SmartBoxRule { Kind = SmartBoxRuleKind.NameContains, Value = "studio" };

        Assert.True(rule.Matches(item));
    }
}
