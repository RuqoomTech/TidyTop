using System.Collections.ObjectModel;
using Avalonia.Media;
using ReactiveUI;
using TidyTop.Core.Models;

namespace TidyTop.App.ViewModels;

public sealed class SmartBoxViewModel : ReactiveObject
{
    public const int MinimumWidth = SmartBox.MinimumWidth;
    public const int MinimumHeight = SmartBox.MinimumHeight;

    private string _title;
    private string _headerText;
    private bool _isVisible;
    private int _x;
    private int _y;
    private int _width;
    private int _height;
    private bool _isBeingMoved;
    private bool _isBeingResized;

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
        _isVisible = snapshot.SmartBox.IsVisible;
        _x = snapshot.SmartBox.X;
        _y = snapshot.SmartBox.Y;
        _width = snapshot.SmartBox.Width;
        _height = snapshot.SmartBox.Height;
        AccentBrush = ToBrush(snapshot.SmartBox.AccentColor);
        Items = new ObservableCollection<DesktopItemViewModel>(snapshot.Items.Select(item => new DesktopItemViewModel(item, Id, Title)));
    }

    public Guid Id { get; }
    public string Emoji { get; }
    public string Behavior { get; }
    public bool IsSystemBox { get; }
    public bool IsLocked { get; }
    public bool IsCollapsed { get; }
    public IBrush AccentBrush { get; }
    public ObservableCollection<DesktopItemViewModel> Items { get; }
    public int ItemCount => Items.Count;
    public bool HasItems => ItemCount > 0;
    public bool IsEmpty => ItemCount == 0;
    public string BoxSubtitle => Behavior switch
    {
        nameof(SmartBoxBehavior.CatchAll) => "Unboxed desktop items",
        nameof(SmartBoxBehavior.Manual) => "Manual group",
        _ => "Auto-sorted group"
    };

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

    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }

    public int X
    {
        get => _x;
        private set => this.RaiseAndSetIfChanged(ref _x, value);
    }

    public int Y
    {
        get => _y;
        private set => this.RaiseAndSetIfChanged(ref _y, value);
    }

    public int Width
    {
        get => _width;
        private set => this.RaiseAndSetIfChanged(ref _width, value);
    }

    public int Height
    {
        get => _height;
        private set => this.RaiseAndSetIfChanged(ref _height, value);
    }

    public bool IsBeingMoved
    {
        get => _isBeingMoved;
        private set => this.RaiseAndSetIfChanged(ref _isBeingMoved, value);
    }

    public bool IsBeingResized
    {
        get => _isBeingResized;
        private set => this.RaiseAndSetIfChanged(ref _isBeingResized, value);
    }

    public bool IsInteracting => IsBeingMoved || IsBeingResized;

    public string EmptyText => Behavior == nameof(SmartBoxBehavior.Manual)
        ? "Drop desktop items here to organize them."
        : "No matching desktop items.";

    public void BeginMove()
    {
        IsBeingMoved = true;
        this.RaisePropertyChanged(nameof(IsInteracting));
    }

    public void BeginResize()
    {
        IsBeingResized = true;
        this.RaisePropertyChanged(nameof(IsInteracting));
    }

    public void EndInteraction()
    {
        IsBeingMoved = false;
        IsBeingResized = false;
        this.RaisePropertyChanged(nameof(IsInteracting));
    }

    public void SetPosition(int x, int y)
    {
        X = Math.Max(0, x);
        Y = Math.Max(0, y);
    }

    public void SetSize(int width, int height)
    {
        Width = Math.Max(MinimumWidth, width);
        Height = Math.Max(MinimumHeight, height);
    }

    public void SetGeometry(int x, int y, int width, int height)
    {
        SetPosition(x, y);
        SetSize(width, height);
    }

    private static IBrush ToBrush(string color)
    {
        return new SolidColorBrush(Color.Parse(string.IsNullOrWhiteSpace(color) ? "#64748B" : color));
    }
}
