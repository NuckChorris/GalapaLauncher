using DQXLauncher.Avalonia.ViewModels.LoginFrame;

namespace DQXLauncher.Avalonia.ViewModels.AppFrame;

public class HomePageViewModel(LoginFrameViewModel loginFrameViewModel) : AppPageViewModel
{
    public LoginFrameViewModel LoginFrameViewModel { get; set; } = loginFrameViewModel;
}