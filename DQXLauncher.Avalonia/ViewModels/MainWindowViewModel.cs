namespace DQXLauncher.Avalonia.ViewModels;

public partial class MainWindowViewModel(AppFrameViewModel appFrameViewModel) : ViewModelBase
{
    public AppFrameViewModel AppFrameViewModel { get; set; } = appFrameViewModel;
}