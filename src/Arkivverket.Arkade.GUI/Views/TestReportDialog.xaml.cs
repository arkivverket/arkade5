using System.IO;
using System.Windows;
using Arkivverket.Arkade.GUI.ViewModels;
using Application = System.Windows.Application;

namespace Arkivverket.Arkade.GUI.Views
{
    /// <summary>
    /// Interaction logic for TestReportDialog.xaml
    /// </summary>
    public partial class TestReportDialog
    {
        public TestReportDialog(DirectoryInfo testReportDirectory)
        {
            InitializeComponent();

            Owner = Application.Current.MainWindow;

            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            ((TestReportDialogViewModel) DataContext).TestReportDirectory = testReportDirectory;
        }
    }
}
