using System.Windows;
using Arkivverket.Arkade.GUI.ViewModels;

namespace Arkivverket.Arkade.GUI.Views
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog
    {
        private readonly AboutDialogViewModel _aboutDialogViewModel;
        public AboutDialog()
        {
            InitializeComponent();

            _aboutDialogViewModel = new AboutDialogViewModel();

            DataContext = _aboutDialogViewModel;

            Owner = Application.Current.MainWindow;

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
    }
}
