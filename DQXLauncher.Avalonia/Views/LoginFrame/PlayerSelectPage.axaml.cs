using Avalonia.Controls;
using DQXLauncher.Avalonia.ViewModels.LoginFrame;

namespace DQXLauncher.Avalonia.Views.LoginFrame;

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