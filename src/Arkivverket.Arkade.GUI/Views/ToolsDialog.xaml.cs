using System.Windows;
using Arkivverket.Arkade.GUI.ViewModels;

namespace Arkivverket.Arkade.GUI.Views
{
    /// <summary>
    /// Interaction logic for ToolDialog.xaml
    /// </summary>
    public partial class ToolsDialog
    {
        private readonly ToolsDialogViewModel _toolsDialog;
        public ToolsDialog()
        {
            InitializeComponent();

            _toolsDialog = new ToolsDialogViewModel();

            DataContext = _toolsDialog;

            Owner = Application.Current.MainWindow;

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
    }
}
