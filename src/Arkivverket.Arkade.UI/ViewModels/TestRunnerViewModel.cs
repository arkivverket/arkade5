using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
using Arkivverket.Arkade.Report;
using Arkivverket.Arkade.UI.Util;
using Application = System.Windows.Application;

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class TestRunnerViewModel : BindableBase, INavigationAware
    {
        private readonly ILogger _log = Log.ForContext<TestRunnerViewModel>();

        private ObservableCollection<OperationMessage> _operationMessages = new ObservableCollection<OperationMessage>();

        private readonly TestSessionFactory _testSessionFactory;
        private readonly TestEngineFactory _testEngineFactory;
        private readonly IRegionManager _regionManager;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly IStatusEventHandler _statusEventHandler;

        public DelegateCommand NavigateToCreatePackageCommand { get; set; }
        private DelegateCommand RunTestEngineCommand { get; set; }
        public DelegateCommand ShowReportCommand { get; set; }
        public DelegateCommand NewProgramSessionCommand { get; set; }

        private string _archiveFileName;
        private ArchiveType _archiveType;
        private TestSession _testSession;
        private bool _isRunningTests;
        private ArchiveInformationStatus _archiveInformationStatus = new ArchiveInformationStatus();
        private Visibility _archiveCurrentProcessing = Visibility.Hidden;
        private Visibility _addmlDataObjectStatusVisibilty = Visibility.Collapsed;
        private Visibility _addmlFlatFileStatusVisibilty = Visibility.Collapsed;
        private int _numberOfProcessedRecords = 0;
        private int _numberOfProcessedFiles = 0;
        private string _currentlyProcessingFile;
        private string _currentActivityMessage;
        private int _numberOfTestsFinished = 0;
        private string _currentlyRunningTest;

        public Visibility AddmlDataObjectStatusVisibility
        {
            get { return _addmlDataObjectStatusVisibilty; }
            set { SetProperty(ref _addmlDataObjectStatusVisibilty, value); }
        }

        public Visibility AddmlFlatFileStatusVisibility
        {
            get { return _addmlFlatFileStatusVisibilty; }
            set { SetProperty(ref _addmlFlatFileStatusVisibilty, value); }
        }

        public string CurrentlyRunningTest
        {
            get { return _currentlyRunningTest; }
            set { SetProperty(ref _currentlyRunningTest, value); }
        }

        public int NumberOfTestsFinished
        {
            get { return _numberOfTestsFinished; }
            set { SetProperty(ref _numberOfTestsFinished, value); }
        }

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

        public int NumberOfProcessedRecords
        {
            get { return _numberOfProcessedRecords; }
            set { SetProperty(ref _numberOfProcessedRecords, value); }
        }

        public ObservableCollection<OperationMessage> OperationMessages
        {
            get { return _operationMessages; }
            set { SetProperty(ref _operationMessages, value); }
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
            _statusEventHandler.OperationMessageEvent += OnOperationMessageEvent;
            _statusEventHandler.TestStartedEvent += OnTestStartedEvent;
            _statusEventHandler.TestFinishedEvent += OnTestFinishedEvent;
            _statusEventHandler.FileProcessStartedEvent += OnFileProcessStartedEvent;
            _statusEventHandler.FileProcessFinishedEvent += OnFileProcessFinishedEvent;
            _statusEventHandler.RecordProcessingStartedEvent += OnRecordProcessingStartedEvent;
            _statusEventHandler.RecordProcessingFinishedEvent += OnRecordProcessingFinishedEvent;
            _statusEventHandler.NewArchiveProcessEvent += OnNewArchiveInformationEvent;
            
            RunTestEngineCommand = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => RunTests()));
            NavigateToCreatePackageCommand = new DelegateCommand(NavigateToCreatePackage, IsFinishedRunningTests);
            NewProgramSessionCommand = new DelegateCommand(ReturnToProgramStart, IsFinishedRunningTests);
            //ShowReportCommand = new DelegateCommand(SaveAndShowPdfReport, IsFinishedRunningTests);
            ShowReportCommand = new DelegateCommand(ShowHtmlReport, IsFinishedRunningTests);
        }
        private void OnTestStartedEvent(object sender, OperationMessageEventArgs eventArgs)
        {
            CurrentlyRunningTest = eventArgs.Id;
        }

        private void OnTestFinishedEvent(object sender, OperationMessageEventArgs eventArgs)
        {
            NumberOfTestsFinished = NumberOfTestsFinished + 1;
        }


        private void ReturnToProgramStart()
        {
            _regionManager.RequestNavigate("MainContentRegion", "LoadArchiveExtraction");
        }


        private void NavigateToCreatePackage()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("TestSession", _testSession);
            _regionManager.RequestNavigate("MainContentRegion", "CreatePackage", navigationParameters);
        }

        private bool IsFinishedRunningTests()
        {
            return !_isRunningTests;
        }
        public void OnNavigatedTo(NavigationContext context)
        {
            _archiveType = (ArchiveType)context.Parameters["archiveType"];
            _archiveFileName = (string)context.Parameters["archiveFileName"];

            RunTestEngineCommand.Execute();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }


        private void OnOperationMessageEvent(object sender, OperationMessageEventArgs eventArgs)
        {
            UpdateOperationMessageList(eventArgs);
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
            NumberOfProcessedRecords = NumberOfProcessedRecords + 1;
        }

        private void OnNewArchiveInformationEvent(object sender, ArchiveInformationEventArgs eventArgs)
        {
            _log.Debug("Got a OnNewArchiveProcessEvent");

            ArchiveInformationStatus.Update(eventArgs);
            ArchiveCurrentProcessing = Visibility.Visible;

            if (eventArgs.ArchiveType == ArchiveType.Noark5.ToString())
            {
                AddmlDataObjectStatusVisibility = Visibility.Visible;
            }
            else
            {
                AddmlFlatFileStatusVisibility = Visibility.Visible;
            }
        }

        private void RunTests()
        {
            try
            {
                _log.Debug("Issued the RunTests command");

                NotifyStartRunningTests();

                if (Directory.Exists(_archiveFileName))
                {
                    _testSession = _testSessionFactory.NewSessionFromArchiveDirectory(ArchiveDirectory.Read(_archiveFileName, _archiveType));
                } else
                {
                    _testSession = _testSessionFactory.NewSessionFromArchiveFile(ArchiveFile.Read(_archiveFileName, _archiveType));
                }

                _log.Debug(_testSession.Archive.Uuid.GetValue());
                _log.Debug(_testSession.Archive.ArchiveType.ToString());
                _log.Debug(_testSession.Archive.WorkingDirectory.Name);

                ITestEngine testEngine = _testEngineFactory.GetTestEngine(_testSession);
                _testSession.TestSuite = testEngine.RunTestsOnArchive(_testSession);
                _testSession.TestSummary = new TestSummary(_numberOfProcessedFiles, _numberOfProcessedRecords, _numberOfTestsFinished);

                TestSessionXmlGenerator.GenerateXmlAndSaveToFile(_testSession);
                SaveHtmlReport();

                NotifyFinishedRunningTests();
            }
            catch (Exception e)
            {
                ExceptionMessageBox.Show(e);
                throw;
            }
        }


        private void NotifyFinishedRunningTests()
        {
            _isRunningTests = false;
            ShowReportCommand.RaiseCanExecuteChanged();
            NavigateToCreatePackageCommand.RaiseCanExecuteChanged();
            NewProgramSessionCommand.RaiseCanExecuteChanged();
            _statusEventHandler.RaiseEventOperationMessage(Resources.UI.TestrunnerFinishedOperationMessage, null, OperationMessageStatus.Ok);
        }

        private void NotifyStartRunningTests()
        {
            _isRunningTests = true;
            ShowReportCommand.RaiseCanExecuteChanged();
            NavigateToCreatePackageCommand.RaiseCanExecuteChanged();
            NewProgramSessionCommand.RaiseCanExecuteChanged();
        }

        private void UpdateOperationMessageList(OperationMessageEventArgs operationMessageEventArgs)
        {
            // http://stackoverflow.com/questions/18331723/this-type-of-collectionview-does-not-support-changes-to-its-sourcecollection-fro
            Application.Current.Dispatcher.Invoke(delegate
            {
                var item = OperationMessages.FirstOrDefault(i => i.Id == operationMessageEventArgs.Id);
                if (item != null)
                {
                    item.Update(operationMessageEventArgs);
                }
                else
                {
                    OperationMessages.Add(new OperationMessage(operationMessageEventArgs));
                }
                OperationMessages.Sort();
            });
        }

        private void OpenFile(FileInfo file)
        {
            System.Diagnostics.Process.Start(file.FullName);
        }

        private void SaveHtmlReport()
        {
            FileInfo file = GetHtmlFile();
            SaveHtmlReport(file);
        }

        private FileInfo GetHtmlFile()
        {
            DirectoryInfo directoryName = _testSession.GetReportDirectory();
            return new FileInfo(Path.Combine(directoryName.FullName, "report.html"));
        }

        private void ShowHtmlReport()
        {
            OpenFile(GetHtmlFile());
        }

        private void SaveHtmlReport(FileInfo htmlFile)
        {
            string eventId = "Lager rapport";
            _statusEventHandler.RaiseEventOperationMessage(eventId, null, OperationMessageStatus.Started);

            Core.Arkade arkade = new Core.Arkade();
            arkade.SaveReport(_testSession, htmlFile);

            var message = "Rapport lagret " + htmlFile.FullName;
            _statusEventHandler.RaiseEventOperationMessage(eventId, message, OperationMessageStatus.Ok);
        }

    }
}
