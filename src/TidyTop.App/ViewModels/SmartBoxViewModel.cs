using System.Collections.ObjectModel;
using Avalonia.Media;
using ReactiveUI;
using TidyTop.Core.Models;

namespace TidyTop.App.ViewModels;

public sealed class SmartBoxViewModel : ReactiveObject
{
    private string _title;
    private string _headerText;

    public SmartBoxViewModel(SmartBoxSnapshot snapshot)
    {
        Id = snapshot.SmartBox.Id;
        _title = snapshot.SmartBox.Title;
        _headerText = snapshot.SmartBox.HeaderText;
        Emoji = snapshot.SmartBox.Emoji;
        Behavior = snapshot.SmartBox.Behavior.ToString();
        IsSystemBox = snapshot.SmartBox.IsSystemBox;
        IsLocked = snapshot.SmartBox.IsLocked;
        IsCollapsed = snapshot.SmartBox.IsCollapsed;
        IsVisible = snapshot.SmartBox.IsVisible;
        X = snapshot.SmartBox.X;
        Y = snapshot.SmartBox.Y;
        Width = snapshot.SmartBox.Width;
        Height = snapshot.SmartBox.Height;
        AccentBrush = ToBrush(snapshot.SmartBox.AccentColor);
        Items = new ObservableCollection<DesktopItemViewModel>(snapshot.Items.Select(item => new DesktopItemViewModel(item)));
    }

    public Guid Id { get; }
    public string Emoji { get; }
    public string Behavior { get; }
    public bool IsSystemBox { get; }
    public bool IsLocked { get; }
    public bool IsCollapsed { get; }
    public bool IsVisible { get; }
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }
    public IBrush AccentBrush { get; }
    public ObservableCollection<DesktopItemViewModel> Items { get; }
    public int ItemCount => Items.Count;

    public string Title
    {
        get => _title;
        private set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    public string HeaderText
    {
        get => _headerText;
        private set => this.RaiseAndSetIfChanged(ref _headerText, value);
    }

    public string EmptyText => Behavior == nameof(SmartBoxBehavior.Manual)
        ? "Empty manual box. Drag/drop assignment is planned next."
        : "No matching desktop items.";

    private static IBrush ToBrush(string color)
    {
        return new SolidColorBrush(Color.Parse(string.IsNullOrWhiteSpace(color) ? "#64748B" : color));
    }
}
