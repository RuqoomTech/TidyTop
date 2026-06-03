using TidyTop.Core.Models;

namespace TidyTop.Core.Tests.Models;

public class SmartBoxTests
{
    [Fact]
    public void AddIcon_AssignsSmartBoxIdAndIncrementsCount()
    {
        var smartBox = new SmartBox { Id = Guid.NewGuid().ToString(), Title = "Work" };
        var icon = new DesktopIcon { Name = "Report", FullPath = @"C:\Users\Test\Desktop\Report.pdf", Extension = ".pdf" };

        smartBox.AddIcon(icon);

        Assert.Single(smartBox.Icons);
        Assert.Equal(smartBox.Id, icon.SmartBoxId);
        Assert.Contains("(1)", smartBox.FormattedTitle);
    }

    [Fact]
    public void RemoveIcon_ClearsSmartBoxId()
    {
        var smartBox = new SmartBox { Id = Guid.NewGuid().ToString(), Title = "Work" };
        var icon = new DesktopIcon { Name = "Report", FullPath = @"C:\Users\Test\Desktop\Report.pdf", Extension = ".pdf" };
        smartBox.AddIcon(icon);

        smartBox.RemoveIcon(icon);

        Assert.Empty(smartBox.Icons);
        Assert.Null(icon.SmartBoxId);
    }

    [Fact]
    public void MatchesIcon_UsesCategoryExtensions()
    {
        var category = new ApplicationCategory
        {
            Name = "Documents",
            FileExtensions = { ".pdf" }
        };
        var smartBox = new SmartBox { Category = category, AutoOrganize = true };
        var icon = new DesktopIcon { Name = "Manual", Extension = ".pdf", FullPath = @"C:\Desktop\Manual.pdf" };

        Assert.True(smartBox.MatchesIcon(icon));
    }
}
