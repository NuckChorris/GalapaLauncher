using Galapa.Launcher.ViewModels.AppFrame;

namespace Galapa.Launcher.ViewModels;

public partial class MainWindowViewModel(AppFrameViewModel appFrameViewModel) : ViewModelBase
{
    public AppFrameViewModel AppFrameViewModel { get; set; } = appFrameViewModel;
}