using System.Collections.ObjectModel;
using ReactiveUI;
using TidyTop.App.Commands;
using TidyTop.Core.Models;
using TidyTop.Core.Services;

namespace TidyTop.App.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly IDesktopWorkspaceService _workspaceService;
    private bool _hasLoaded;
    private int _totalItemCount;
    private int _organizedItemCount;
    private int _boxCount;

    public MainWindowViewModel(IDesktopWorkspaceService workspaceService)
    {
        _workspaceService = workspaceService;
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        AddSmartBoxCommand = new AsyncRelayCommand(AddSmartBoxAsync);
        ResetLayoutCommand = new AsyncRelayCommand(ResetLayoutAsync);
        SaveLayoutCommand = new AsyncRelayCommand(SaveLayoutAsync);
    }

    public string Title => "TidyTop";
    public string Subtitle => "Windows-first desktop organizer MVP";
    public ObservableCollection<SmartBoxViewModel> SmartBoxes { get; } = new();

    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand AddSmartBoxCommand { get; }
    public AsyncRelayCommand ResetLayoutCommand { get; }
    public AsyncRelayCommand SaveLayoutCommand { get; }

    public int TotalItemCount
    {
        get => _totalItemCount;
        private set => this.RaiseAndSetIfChanged(ref _totalItemCount, value);
    }

    public int OrganizedItemCount
    {
        get => _organizedItemCount;
        private set => this.RaiseAndSetIfChanged(ref _organizedItemCount, value);
    }

    public int BoxCount
    {
        get => _boxCount;
        private set => this.RaiseAndSetIfChanged(ref _boxCount, value);
    }

    public string SummaryText => $"{OrganizedItemCount}/{TotalItemCount} items organized across {BoxCount} boxes";

    public async Task InitializeAsync()
    {
        if (_hasLoaded)
        {
            return;
        }

        _hasLoaded = true;
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        await RunWorkspaceOperationAsync("Loading desktop layout...", () => _workspaceService.LoadAsync());
    }

    private async Task RefreshAsync()
    {
        await RunWorkspaceOperationAsync("Refreshing desktop...", () => _workspaceService.RefreshAsync());
    }

    private async Task AddSmartBoxAsync()
    {
        await RunWorkspaceOperationAsync("Adding SmartBox...", () => _workspaceService.AddSmartBoxAsync(""));
    }

    private async Task ResetLayoutAsync()
    {
        await RunWorkspaceOperationAsync("Resetting layout...", () => _workspaceService.ResetLayoutAsync());
    }

    private async Task SaveLayoutAsync()
    {
        try
        {
            BeginBusy("Saving layout...");
            await _workspaceService.SaveAsync();
            EndBusy("Layout saved.");
        }
        catch (Exception ex)
        {
            Fail($"Could not save layout: {ex.Message}");
        }
    }


    public async Task CommitSmartBoxGeometryAsync(SmartBoxViewModel smartBox)
    {
        ArgumentNullException.ThrowIfNull(smartBox);

        try
        {
            await _workspaceService.UpdateSmartBoxGeometryAsync(
                smartBox.Id,
                smartBox.X,
                smartBox.Y,
                smartBox.Width,
                smartBox.Height);

            StatusMessage = $"Saved position for {smartBox.Title}.";
        }
        catch (Exception ex)
        {
            Fail($"Could not save SmartBox position: {ex.Message}");
        }
    }

    private async Task RunWorkspaceOperationAsync(string busyMessage, Func<Task<DesktopWorkspace>> operation)
    {
        try
        {
            BeginBusy(busyMessage);
            var workspace = await operation();
            ApplyWorkspace(workspace);
            EndBusy($"Loaded {workspace.TotalItemCount} desktop items into {workspace.BoxCount} SmartBoxes.");
        }
        catch (Exception ex)
        {
            Fail($"TidyTop could not update the workspace: {ex.Message}");
        }
    }

    private void ApplyWorkspace(DesktopWorkspace workspace)
    {
        SmartBoxes.Clear();
        foreach (var smartBox in workspace.SmartBoxes)
        {
            SmartBoxes.Add(new SmartBoxViewModel(smartBox));
        }

        TotalItemCount = workspace.TotalItemCount;
        OrganizedItemCount = workspace.OrganizedItemCount;
        BoxCount = workspace.BoxCount;
        this.RaisePropertyChanged(nameof(SummaryText));
    }
}
