using CommunityToolkit.Mvvm.ComponentModel;
using Galapa.Launcher.ViewModels.AppFrame;

namespace Galapa.Launcher.ViewModels;

public partial class MainWindowViewModel(AppFrameViewModel appFrameViewModel) : ObservableObject
{
    public AppFrameViewModel AppFrameViewModel { get; set; } = appFrameViewModel;
}