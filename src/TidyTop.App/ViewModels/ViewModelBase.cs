using ReactiveUI;

namespace TidyTop.App.ViewModels;

public abstract class ViewModelBase : ReactiveObject
{
    private bool _isBusy;
    private string _statusMessage = "Ready";
    private string? _errorMessage;

    public bool IsBusy
    {
        get => _isBusy;
        protected set => this.RaiseAndSetIfChanged(ref _isBusy, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        protected set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        protected set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    protected void BeginBusy(string message)
    {
        ErrorMessage = null;
        this.RaisePropertyChanged(nameof(HasError));
        StatusMessage = message;
        IsBusy = true;
    }

    protected void EndBusy(string message)
    {
        StatusMessage = message;
        IsBusy = false;
    }

    protected void Fail(string message)
    {
        ErrorMessage = message;
        this.RaisePropertyChanged(nameof(HasError));
        StatusMessage = message;
        IsBusy = false;
    }
}
