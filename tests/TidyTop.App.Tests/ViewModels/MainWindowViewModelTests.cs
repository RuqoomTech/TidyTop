using TidyTop.App.ViewModels;

namespace TidyTop.App.Tests.ViewModels;

public class MainWindowViewModelTests
{
    [Fact]
    public void Title_DefaultsToProductName()
    {
        var viewModel = new MainWindowViewModel();

        Assert.Contains("TidyTop", viewModel.Title);
    }
}
