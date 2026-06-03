namespace TidyTop.Core.Models;

/// <summary>
/// A runtime projection of a saved layout reconciled with the current desktop scan.
/// </summary>
public sealed record DesktopWorkspace(
    DesktopLayout Layout,
    IReadOnlyList<DesktopItem> Items,
    IReadOnlyList<SmartBoxSnapshot> SmartBoxes)
{
    public int TotalItemCount => Items.Count;
    public int OrganizedItemCount => SmartBoxes.Sum(box => box.Items.Count);
    public int BoxCount => SmartBoxes.Count;
}

public sealed record SmartBoxSnapshot(SmartBox SmartBox, IReadOnlyList<DesktopItem> Items);
