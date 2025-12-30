using Galapa.Toolbox.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Galapa.Toolbox
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            ContentFrame.Navigate(typeof(HomePage));
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                ContentFrame.Navigate(typeof(SettingsPage));
            }
            else if (args.SelectedItem is NavigationViewItem selectedItem)
            {
                switch (selectedItem.Tag)
                {
                    case "HomePage":
                        ContentFrame.Navigate(typeof(HomePage));
                        break;
                    case "SaveExplorerPage":
                        ContentFrame.Navigate(typeof(SaveExplorerPage));
                        break;
                    case "ObfuscatorToolPage":
                        this.ContentFrame.Navigate(typeof(ObfuscatorToolPage));
                        break;
                }
            }
        }
    }
}