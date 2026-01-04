using Avalonia.Controls;
using Galapa.Toolbox.ViewModels;

namespace Galapa.Toolbox.Views;

public partial class SaveExplorerPage : UserControl
{
    public SaveExplorerPage()
    {
        this.InitializeComponent();
        this.DataContext = new SaveExplorerPageViewModel();
    }
}