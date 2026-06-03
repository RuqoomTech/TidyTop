using TidyTop.Core.Models;
using TidyTop.Core.Services;

namespace TidyTop.Core.Tests.Services;

public class LayoutReconcilerTests
{
    [Fact]
    public void Reconcile_AssignsMatchingItemsToRuleBasedBoxesAndRemainingToCatchAll()
    {
        var layout = DefaultSmartBoxFactory.CreateDefaultLayout();
        var items = new[]
        {
            CreateItem("Report", @"C:\Desktop\Report.pdf", ".pdf"),
            CreateItem("Unknown", @"C:\Desktop\Unknown.xyz", ".xyz")
        };

        var workspace = new LayoutReconciler().Reconcile(layout, items);

        var office = workspace.SmartBoxes.Single(box => box.SmartBox.Title == "Office & Documents");
        var other = workspace.SmartBoxes.Single(box => box.SmartBox.Behavior == SmartBoxBehavior.CatchAll);

        Assert.Contains(office.Items, item => item.Name == "Report");
        Assert.Contains(other.Items, item => item.Name == "Unknown");
    }

    [Fact]
    public void Reconcile_RemovesAssignmentsForDeletedDesktopItems()
    {
        var layout = DefaultSmartBoxFactory.CreateDefaultLayout();
        var office = layout.SmartBoxes.Single(box => box.Title == "Office & Documents");
        office.AssignPath(@"C:\Desktop\Deleted.pdf");

        new LayoutReconciler().Reconcile(layout, Array.Empty<DesktopItem>());

        Assert.DoesNotContain(office.ItemPaths, path => path.Contains("Deleted", StringComparison.OrdinalIgnoreCase));
    }

    private static DesktopItem CreateItem(string name, string path, string extension)
    {
        var normalizedPath = DesktopItem.NormalizePath(path);
        return new DesktopItem
        {
            Id = name,
            Name = name,
            FullPath = path,
            NormalizedPath = normalizedPath,
            Extension = extension,
            Type = DesktopItemType.File
        };
    }
}
