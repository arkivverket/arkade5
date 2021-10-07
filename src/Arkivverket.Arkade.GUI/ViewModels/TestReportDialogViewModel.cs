using System.IO;
using System.Linq;
using System.Windows.Forms;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Report;
using Arkivverket.Arkade.GUI.Languages;
using Arkivverket.Arkade.GUI.Util;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;

namespace Arkivverket.Arkade.GUI.ViewModels
{
    public class TestReportDialogViewModel : BindableBase
    {
        private readonly ILogger _log = Log.ForContext<TestReportDialogViewModel>();
        private readonly ArkadeApi _arkadeApi;
        public DelegateCommand ShowTestReportCommand { get; }
        public DelegateCommand ExportTestReportFilesCommand { get; }
        public TestSession TestSession;
        private DirectoryInfo TestReportDirectory => TestSession.Archive.GetTestReportDirectory();
        private Uuid Uuid => TestSession.Archive.Uuid;

        public TestReportDialogViewModel(ArkadeApi arkadeApi)
        {
            _arkadeApi = arkadeApi;
            
            ShowTestReportCommand = new DelegateCommand(ShowTestReport);

            ExportTestReportFilesCommand = new DelegateCommand(ExportTestReport);
        }

        private void ShowTestReport()
        {
            _log.Information("User action: Show HTML test report");

            FileInfo testReportFile = TestReportDirectory.GetFiles()
                .FirstOrDefault(f => f.Extension.Contains(TestReportFormat.html.ToString()));

            if (testReportFile == default)
                testReportFile = TestReportDirectory.GetFiles().First(f => f.Extension.Equals(".txt"));
                
            testReportFile.FullName.LaunchUrl();
        }

        private void ExportTestReport()
        {
            const string action = "export test report";

            _log.Information($"User action: Open choose directory for {action} dialog");

            DirectoryPicker("Export test report",
                TestReportGUI.ChooseTestReportExportDestination,
                out string testReportExportDestination
            );

            if (testReportExportDestination == null)
            {
                _log.Information($"User action: Abort choose directory for {action}");
                return;
            }

            _log.Information($"User action: Chose directory for {action}: {testReportExportDestination}");

            var testReportExportDirectory = new DirectoryInfo(Path.Combine(testReportExportDestination,
                string.Format(Core.Resources.OutputFileNames.StandaloneTestReportDirectory, TestSession.Archive.Uuid)));

            if (!testReportExportDirectory.Exists)
                testReportExportDirectory.Create();

            //foreach (FileInfo testReportFile in TestReportDirectory.GetFiles())
            //{
            //    string destinationTestReportFileName = Path.Combine(
            //        testReportExportDirectory.FullName,
            //        testReportFile.Name.Equals(Core.Resources.OutputFileNames.DbptkValidationReportFile)
            //            ? string.Format(Core.Resources.OutputFileNames.StandaloneDbptkValidationReportFile, TestSession.Archive.Uuid)
            //            : string.Format(Core.Resources.OutputFileNames.StandaloneTestReportFile, TestSession.Archive.Uuid,
            //                testReportFile.Extension.Trim('.'))
            //    );

            //    testReportFile.CopyTo(destinationTestReportFileName, overwrite: true);
            //}

            _arkadeApi.SaveReport(TestSession, testReportExportDirectory, standalone: true);

            string argument = "/select, \"" + testReportExportDirectory + "\"";
            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        private void DirectoryPicker(string action, string title, out string directory)
        {
            _log.Information($"User action: Open choose directory for {action} dialog");

            var selectDirectoryDialog = new FolderBrowserDialog
            {
                Description = title,
                UseDescriptionForTitle = true,
            };

            if (selectDirectoryDialog.ShowDialog() == DialogResult.OK)
            {
                directory = selectDirectoryDialog.SelectedPath;

                _log.Information($"User action: Chose directory for {action}: {directory}");
            }
            else
            {
                directory = null;
                _log.Information($"User action: Abort choose directory for {action}");
            }

            // TODO: "Merge" with ToolsDialogViewModel.DirectoryPicker
        }
    }
}
