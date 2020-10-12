using System.Windows;
using Arkivverket.Arkade.GUI.ViewModels;

namespace Arkivverket.Arkade.GUI.Views
{
    /// <summary>
    /// Interaction logic for ToolDialog.xaml
    /// </summary>
    public partial class ToolsDialog
    {
        public ToolsDialog()
        {
            InitializeComponent();

            DataContext = new ToolsDialogViewModel();

            Owner = Application.Current.MainWindow;

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
    }
}
