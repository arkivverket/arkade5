using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Report;
using Arkivverket.Arkade.GUI.Languages;
using Arkivverket.Arkade.GUI.Util;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;
using Settings = Arkivverket.Arkade.GUI.Properties.Settings;

namespace Arkivverket.Arkade.GUI.ViewModels
{
    public class TestReportDialogViewModel : BindableBase
    {
        private readonly ArkadeCoreApi _arkadeCoreApi;
        private readonly IStatusEventHandler _statusEventHandler;
        private readonly ILogger _log = Log.ForContext<TestReportDialogViewModel>();
        public Archive Archive { get; set; }
        public DelegateCommand ShowTestReportCommand { get; }
        public DelegateCommand ExportTestReportFilesCommand { get; }
        private bool _isGeneratingTestReport = false;

        public TestReportDialogViewModel(ArkadeCoreApi arkadeCoreApi, IStatusEventHandler statusEventHandler)
        {
            _arkadeCoreApi = arkadeCoreApi;
            _statusEventHandler = statusEventHandler;
            
            ShowTestReportCommand = new DelegateCommand(ShowTestReport, () => !_isGeneratingTestReport);

            ExportTestReportFilesCommand = new DelegateCommand(ExportTestReport, () => !_isGeneratingTestReport);
        }

        private async void ShowTestReport()
        {
            _log.Information("User action: Show HTML test report");
            
            var tmpResultsDirectory = new DirectoryInfo(Archive.TestSession.TemporaryTestResultFilesDirectory.FullName);
            
            DirectoryInfo testReportDirectory = await GenerateTestReport(tmpResultsDirectory);

            FileInfo testReportFile = testReportDirectory.GetFiles()
                .FirstOrDefault(f => f.Extension.Contains(TestReportFormat.html.ToString()));

            if (testReportFile == default) // TODO: Consider to remove ...
                testReportFile = testReportDirectory.GetFiles().First(f => f.Extension.Equals(".txt"));
                
            testReportFile.FullName.LaunchUrl();
        }

        private async void ExportTestReport()
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

            DirectoryInfo testReportExportDirectory = await GenerateTestReport(new DirectoryInfo(testReportExportDestination));

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
        
        private async Task<DirectoryInfo> GenerateTestReport(DirectoryInfo targetDirectory)
        {
            _isGeneratingTestReport = true;
            ShowTestReportCommand.RaiseCanExecuteChanged();
            ExportTestReportFilesCommand.RaiseCanExecuteChanged();
            
            string eventId = TestRunnerGUI.EventIdCreatingReport;
            
            _statusEventHandler.RaiseEventOperationMessage(eventId, null, OperationMessageStatus.Started);
            
            DirectoryInfo testReportDirectory = await Task.Run(() => _arkadeCoreApi.GenerateTestReport(Archive, targetDirectory, Settings.Default.TestResultDisplayLimit, Archive.InputDiasPackage));
            
            _statusEventHandler.RaiseEventOperationMessage(eventId, TestRunnerGUI.TestReportIsSavedMessage, OperationMessageStatus.Ok);

            _isGeneratingTestReport = false;
            ShowTestReportCommand.RaiseCanExecuteChanged();
            ExportTestReportFilesCommand.RaiseCanExecuteChanged();
            
            return testReportDirectory;
        }
    }
}
