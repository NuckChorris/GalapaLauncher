using Avalonia.Controls;
using Galapa.Launcher.ViewModels.LoginFrame;

namespace Galapa.Launcher.Views.LoginFrame;

public partial class PlayerSelectPage : UserControl
{
    public PlayerSelectPage()
    {
        this.InitializeComponent();

        this.Loaded += (_, _) =>
        {
            if (this.DataContext is PlayerSelectPageViewModel viewModel) viewModel.LoadCommand.Execute(null);
        };
    }
}