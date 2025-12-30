using Galapa.Launcher.ViewModels.LoginFrame;

namespace Galapa.Launcher.ViewModels.AppFrame;

public class HomePageViewModel(LoginFrameViewModel loginFrameViewModel) : AppPageViewModel
{
    public LoginFrameViewModel LoginFrameViewModel { get; set; } = loginFrameViewModel;
}