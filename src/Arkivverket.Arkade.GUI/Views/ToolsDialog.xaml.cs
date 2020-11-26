using System.Windows;

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

            Owner = Application.Current.MainWindow;

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
    }
}
