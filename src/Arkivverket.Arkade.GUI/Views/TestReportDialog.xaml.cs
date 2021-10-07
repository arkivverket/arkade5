using System.IO;
using System.Windows;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.GUI.ViewModels;
using Application = System.Windows.Application;

namespace Arkivverket.Arkade.GUI.Views
{
    /// <summary>
    /// Interaction logic for TestReportDialog.xaml
    /// </summary>
    public partial class TestReportDialog
    {
        public TestReportDialog(TestSession testSession)
        {
            InitializeComponent();

            Owner = Application.Current.MainWindow;

            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            ((TestReportDialogViewModel) DataContext).TestSession = testSession;
        }
    }
}
