using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.UI.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.UI.Util;
using Arkivverket.Arkade.Util;
using Application = System.Windows.Application;

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class TestRunnerViewModel : BindableBase, INavigationAware
    {
        private readonly ILogger _log = Log.ForContext<TestRunnerViewModel>();

        private ObservableCollection<TestRunnerStatus> _testResults = new ObservableCollection<TestRunnerStatus>();

        private readonly TestSessionFactory _testSessionFactory;
        private readonly TestEngineFactory _testEngineFactory;
        private readonly IRegionManager _regionManager;
        private readonly StatusEventHandler _statusEventHandler;
        public DelegateCommand NavigateToSummaryCommand { get; set; }
        private DelegateCommand RunTestEngineCommand { get; set; }
        public DelegateCommand SaveIpFileCommand { get; set; }

        private string _metadataFileName;
        private string _archiveFileName;
        private TestSession _testSession;
        private bool _isRunningTests;
        private Visibility _finishedTestingMessageVisibility = Visibility.Collapsed;
        private ArchiveInformationStatus _archiveInformationStatus = new ArchiveInformationStatus();
        private Visibility _archiveCurrentProcessing = Visibility.Hidden;
        private string _saveIpStatus;

        public Visibility FinishedTestingMessageVisibility
        {
            get { return _finishedTestingMessageVisibility; }
            set { SetProperty(ref _finishedTestingMessageVisibility, value); }
        }

        public ObservableCollection<TestRunnerStatus> TestResults
        {
            get { return _testResults; }
            set { SetProperty(ref _testResults, value); }
        }

        public ArchiveInformationStatus ArchiveInformationStatus
        {
            get { return _archiveInformationStatus; }
            set { SetProperty(ref _archiveInformationStatus, value); }
        }

        public Visibility ArchiveCurrentProcessing
        {
            get { return _archiveCurrentProcessing; }
            set { SetProperty(ref _archiveCurrentProcessing, value); }
        }
        public string SaveIpStatus
        {
            get { return _saveIpStatus; }
            set { SetProperty(ref _saveIpStatus, value); }
        }


        public TestRunnerViewModel(TestSessionFactory testSessionFactory, TestEngineFactory testEngineFactory, IRegionManager regionManager,  StatusEventHandler statusEventHandler)
        {
            _testSessionFactory = testSessionFactory;
            _testEngineFactory = testEngineFactory;
            _regionManager = regionManager;
            _statusEventHandler = statusEventHandler;
            _statusEventHandler.StatusEvent += OnStatusEvent;
            _statusEventHandler.FileProcessStartEvent += OnFileProcessStartEvent;
            _statusEventHandler.FileProcessStopEvent += OnFileProcessStopEvent;
            _statusEventHandler.RecordProcessStartEvent += OnRecordProcessStartEvent;
            _statusEventHandler.NewTestRecordEvent += OnNewTestRecordEvent;
            _statusEventHandler.NewArchiveProcessEvent += OnNewArchiveProcessEvent;
            
            RunTestEngineCommand = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => RunTests()));
            NavigateToSummaryCommand = new DelegateCommand(NavigateToSummary, CanNavigateToSummary);
            SaveIpFileCommand = new DelegateCommand(SaveIpFile);
        }

        private void NavigateToSummary()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("TestSession", _testSession);
            _regionManager.RequestNavigate("MainContentRegion", "TestSummary", navigationParameters);
        }

        private bool CanNavigateToSummary()
        {
            return !_isRunningTests;
        }
        public void OnNavigatedTo(NavigationContext context)
        {
            _metadataFileName = (string)context.Parameters["metadataFileName"];
            _archiveFileName = (string)context.Parameters["archiveFileName"];

            RunTestEngineCommand.Execute();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }


        private void OnStatusEvent(object sender, StatusEventArgument statusEventArgument)
        {
            UpdateGuiCollection(statusEventArgument);
        }

        private void OnFileProcessStartEvent(object sender, StatusEventArgFileProcessing statusEventArgFileProcessing)
        {
            _log.Debug("Got a onFileProcessStartEvent");
        }

        private void OnFileProcessStopEvent(object sender, StatusEventArgFileProcessing statusEventArgFileProcessing)
        {
            _log.Debug("Got a onFileProcessStopEvent");
        }

        private void OnRecordProcessStartEvent(object sender, StatusEventArgRecord statusEventArgRecord)
        {
            _log.Debug("Got a onRecordProcessStartEvent");
        }

        private void OnNewTestRecordEvent(object sender, StatusEventArgRecord statusEventArgRecord)
        {
            _log.Debug("Got a onNewTestRecordEvent");
        }

        private void OnNewArchiveProcessEvent(object sender, StatusEventNewArchiveInformation statusEventNewArchiveInformation)
        {
            _log.Debug("Got a OnNewArchiveProcessEvent");

            ArchiveInformationStatus.Update(statusEventNewArchiveInformation);
            ArchiveCurrentProcessing = Visibility.Visible;
        }


        private void RunTests()
        {
            try
            {
                _log.Debug("Issued the RunTests command");

                _isRunningTests = true;
                NavigateToSummaryCommand.RaiseCanExecuteChanged();

                _testSession = _testSessionFactory.NewSessionFromTarFile(_archiveFileName, _metadataFileName);

                _log.Debug(_testSession.Archive.Uuid.GetValue());
                _log.Debug(_testSession.Archive.ArchiveType.ToString());
                _log.Debug(_testSession.Archive.WorkingDirectory.Name);

                ITestEngine testEngine = _testEngineFactory.GetTestEngine(_testSession);
                _testSession.TestSuite = testEngine.RunTestsOnArchive(_testSession);
                TestSessionXmlGenerator.GenerateXmlAndSaveToFile(_testSession);

                _isRunningTests = false;
                FinishedTestingMessageVisibility = Visibility.Visible;
                NavigateToSummaryCommand.RaiseCanExecuteChanged();
            }
            catch (Exception e)
            {
                ExceptionMessageBox.Show(e);
                throw;
            }
        }



        private void UpdateGuiCollection(StatusEventArgument statusEventArgument)
        {
            // http://stackoverflow.com/questions/18331723/this-type-of-collectionview-does-not-support-changes-to-its-sourcecollection-fro
            Application.Current.Dispatcher.Invoke(delegate
            {

                if (statusEventArgument.TestStatus == StatusTestExecution.TestStarted)
                {
                    var testRunnerStatus = new TestRunnerStatus(statusEventArgument);
                    TestResults.Add(testRunnerStatus);
                }
                else
                {
                    var item = TestResults.FirstOrDefault(i => i.TestName == statusEventArgument.TestName);
                    if (item != null)
                    {
                        item.Update(statusEventArgument.TestStatus, statusEventArgument.IsSuccess);

                        if (statusEventArgument.ResultMessage != null)
                            item.ResultMessage = statusEventArgument.ResultMessage;
                    }
                }
            });
        }


        private void SaveIpFile()
        {
            DirectoryInfo directoryName = GetDirectoryName();

            Core.Arkade arkade = new Core.Arkade();
            bool saved = arkade.SaveIp(_testSession, directoryName);
            if (saved)
            {
                SaveIpStatus = "IP og metadata lagret i " + directoryName;
            }
        }

        private DirectoryInfo GetDirectoryName()
        {
            string directoryName = Path.Combine(ArkadeConstants.GetArkadeIpDirectory().FullName, _testSession.Archive.Uuid.GetValue());
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            return directoryInfo;

            /* If we want to use a FolderBrowserDialog
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = directoryName;
            dialog.ShowNewFolderButton = true;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                directoryName = dialog.SelectedPath;
                return new DirectoryInfo(directoryName);
            }
            else
            {
                return null;
            }
            */
        }
    }
}
