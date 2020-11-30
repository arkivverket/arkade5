using System.Windows;
using Arkivverket.Arkade.GUI.ViewModels;

namespace Arkivverket.Arkade.GUI.Views
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog
    {
        public AboutDialog()
        {
            InitializeComponent();

            DataContext = new AboutDialogViewModel();

            Owner = Application.Current.MainWindow;

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
    }
}
