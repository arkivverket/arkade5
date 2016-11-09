using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.UI.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.UI.Util;
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

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly IStatusEventHandler _statusEventHandler;

        public DelegateCommand NavigateToCreatePackageCommand { get; set; }
        private DelegateCommand RunTestEngineCommand { get; set; }
        public DelegateCommand SaveIpFileCommand { get; set; }
        public DelegateCommand ShowReportCommand { get; set; }

        private string _metadataFileName;
        private string _archiveFileName;
        private TestSession _testSession;
        private bool _isRunningTests;
        private Visibility _finishedTestingMessageVisibility = Visibility.Collapsed;
        private ArchiveInformationStatus _archiveInformationStatus = new ArchiveInformationStatus();
        private Visibility _archiveCurrentProcessing = Visibility.Hidden;

        private BigInteger _numberOfProcessedRecords = BigInteger.Zero;
        private int _numberOfProcessedFiles = 0;
        private string _currentlyProcessingFile;
        private string _currentActivityMessage;

        public string CurrentActivityMessage
        {
            get { return _currentActivityMessage; }
            set { SetProperty(ref _currentActivityMessage, value); }
        }

        public string CurrentlyProcessingFile
        {
            get { return _currentlyProcessingFile; }
            set { SetProperty(ref _currentlyProcessingFile, value); }
        }

        public int NumberOfProcessedFiles
        {
            get { return _numberOfProcessedFiles; }
            set { SetProperty(ref _numberOfProcessedFiles, value); }
        }

        public string NumberOfProcessedRecords
        {
            get { return _numberOfProcessedRecords.ToString(); }
            set { SetProperty(ref _numberOfProcessedRecords, BigInteger.Parse(value)); }
        }

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

        public TestRunnerViewModel(TestSessionFactory testSessionFactory, TestEngineFactory testEngineFactory, IRegionManager regionManager,  IStatusEventHandler statusEventHandler)
        {
            _testSessionFactory = testSessionFactory;
            _testEngineFactory = testEngineFactory;
            _regionManager = regionManager;
            _statusEventHandler = statusEventHandler;
            _statusEventHandler.StatusEvent += OnStatusEvent;
            _statusEventHandler.FileProcessStartedEvent += OnFileProcessStartedEvent;
            _statusEventHandler.FileProcessFinishedEvent += OnFileProcessFinishedEvent;
            _statusEventHandler.RecordProcessingStartedEvent += OnRecordProcessingStartedEvent;
            _statusEventHandler.RecordProcessingFinishedEvent += OnRecordProcessingFinishedEvent;
            _statusEventHandler.NewArchiveProcessEvent += OnNewArchiveInformationEvent;
            
            RunTestEngineCommand = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => RunTests()));
            NavigateToCreatePackageCommand = new DelegateCommand(NavigateToCreatePackage, CanNavigateToCreatePackage);
            ShowReportCommand = new DelegateCommand(SaveAndShowReport);
        }

        private void NavigateToCreatePackage()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("TestSession", _testSession);
            _regionManager.RequestNavigate("MainContentRegion", "CreatePackage", navigationParameters);
        }

        private bool CanNavigateToCreatePackage()
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


        private void OnStatusEvent(object sender, TestInformationEventArgs eventArgs)
        {
            CurrentActivityMessage = eventArgs.ResultMessage;
            //UpdateGuiCollection(eventArgs);
        }

        private void OnFileProcessStartedEvent(object sender, FileProcessingStatusEventArgs eventArgs)
        {
            _log.Debug("Got a onFileProcessStartEvent");
            CurrentlyProcessingFile = eventArgs.FileName;
        }

        private void OnFileProcessFinishedEvent(object sender, FileProcessingStatusEventArgs eventArgs)
        {
            _log.Debug("Got a onFileProcessStopEvent");
            NumberOfProcessedFiles = NumberOfProcessedFiles + 1;
        }

        private void OnRecordProcessingStartedEvent(object sender, EventArgs eventArgs)
        {
            _log.Verbose("Got a onRecordProcessStartEvent");
        }
        private void OnRecordProcessingFinishedEvent(object sender, EventArgs eventArgs)
        {
            _log.Verbose("Got a onRecordProcessFinishedEvent");

            BigInteger counter = BigInteger.Parse(NumberOfProcessedRecords);
            counter++;
            NumberOfProcessedRecords = counter.ToString();
        }

        private void OnNewArchiveInformationEvent(object sender, ArchiveInformationEventArgs eventArgs)
        {
            _log.Debug("Got a OnNewArchiveProcessEvent");

            ArchiveInformationStatus.Update(eventArgs);
            ArchiveCurrentProcessing = Visibility.Visible;
        }

        private void RunTests()
        {
            try
            {
                _log.Debug("Issued the RunTests command");

                _isRunningTests = true;
                NavigateToCreatePackageCommand.RaiseCanExecuteChanged();

                _testSession = _testSessionFactory.NewSessionFromTarFile(_archiveFileName, _metadataFileName);

                _log.Debug(_testSession.Archive.Uuid.GetValue());
                _log.Debug(_testSession.Archive.ArchiveType.ToString());
                _log.Debug(_testSession.Archive.WorkingDirectory.Name);

                ITestEngine testEngine = _testEngineFactory.GetTestEngine(_testSession);
                _testSession.TestSuite = testEngine.RunTestsOnArchive(_testSession);
                TestSessionXmlGenerator.GenerateXmlAndSaveToFile(_testSession);

                _isRunningTests = false;
                FinishedTestingMessageVisibility = Visibility.Visible;
                NavigateToCreatePackageCommand.RaiseCanExecuteChanged();
            }
            catch (Exception e)
            {
                ExceptionMessageBox.Show(e);
                throw;
            }
        }

        private void UpdateGuiCollection(TestInformationEventArgs testInformationEventArgs)
        {
            // http://stackoverflow.com/questions/18331723/this-type-of-collectionview-does-not-support-changes-to-its-sourcecollection-fro
            Application.Current.Dispatcher.Invoke(delegate
            {

                if (testInformationEventArgs.TestStatus == StatusTestExecution.TestStarted)
                {
                    var testRunnerStatus = new TestRunnerStatus(testInformationEventArgs);
                    TestResults.Add(testRunnerStatus);
                }
                else
                {
                    var item = TestResults.FirstOrDefault(i => i.TestName == testInformationEventArgs.Identifier);
                    if (item != null)
                    {
                        item.Update(testInformationEventArgs.TestStatus, testInformationEventArgs.IsSuccess);

                        if (testInformationEventArgs.ResultMessage != null)
                            item.ResultMessage = testInformationEventArgs.ResultMessage;
                    }
                }
            });
        }

        private void SaveAndShowReport()
        {
            DirectoryInfo directoryName = _testSession.GetReportDirectory();
            FileInfo pdfFile = new FileInfo(Path.Combine(directoryName.FullName, "report.pdf"));
            SaveReport(pdfFile);
            OpenReport(pdfFile);
        }

        private void OpenReport(FileInfo pdfFile)
        {
            System.Diagnostics.Process.Start(pdfFile.FullName);
        }

        private void SaveReport(FileInfo pdfFile)
        {
            _statusEventHandler.RaiseEventTestInformation("SaveIp", "Lager testrapport", StatusTestExecution.TestStarted, false);

            Core.Arkade arkade = new Core.Arkade();
            arkade.SaveReport(_testSession, pdfFile);

            var message = "Rapport lagret " + pdfFile.FullName;
            _statusEventHandler.RaiseEventTestInformation("SaveIp", message, StatusTestExecution.TestCompleted, true);
        }

    }
}
