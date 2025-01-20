using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.GUI.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Regions;
using Serilog;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Testing.Noark5;
using Arkivverket.Arkade.GUI.Util;
using Arkivverket.Arkade.GUI.Views;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.GUI.Languages;
using Application = System.Windows.Application;
using Settings = Arkivverket.Arkade.GUI.Properties.Settings;

namespace Arkivverket.Arkade.GUI.ViewModels
{
    public class TestRunnerViewModel : BindableBase, INavigationAware
    {
        private readonly ILogger _log = Log.ForContext<TestRunnerViewModel>();

        private ObservableCollection<OperationMessage> _operationMessages = new ObservableCollection<OperationMessage>();
        private ObservableCollection<SelectableTest> _selectableTests = new ObservableCollection<SelectableTest>();

        private readonly ArkadeApi _arkadeApi;
        private readonly IRegionManager _regionManager;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly IStatusEventHandler _statusEventHandler;

        public DelegateCommand StartTestingCommand { get; set; }
        public DelegateCommand NavigateToCreatePackageCommand { get; set; }
        private DelegateCommand RunTestEngineCommand { get; set; }
        public DelegateCommand ShowReportCommand { get; set; }
        public DelegateCommand NewProgramSessionCommand { get; set; }

        private string _archiveFileName;
        private ArchiveType _archiveType;
        private TestSession _testSession;
        private bool _testRunHasBeenExecuted;
        private bool _isRunningTests;
        private bool _testRunCompletedSuccessfully;
        private bool _testRunHasFailed;
        private bool _canSelectTests;
        private bool _allTestsSelected;
        private bool _isProcessingRecord;
        private ArchiveInformationStatus _archiveInformationStatus = new ArchiveInformationStatus();
        private Visibility _archiveCurrentProcessing = Visibility.Hidden;
        private Visibility _numberOfProcessedRecordsVisibility = Visibility.Collapsed;
        private Visibility _processingFileVisibility = Visibility.Collapsed;
        private Visibility _addmlDataObjectStatusVisibilty = Visibility.Collapsed;
        private Visibility _addmlFlatFileStatusVisibilty = Visibility.Collapsed;
        private int _numberOfProcessedRecords = 0;
        private int _numberOfProcessedFiles = 0;
        private string _currentlyProcessingFile;
        private string _currentActivityMessage;
        private int _numberOfTestsFinished = 0;
        private string _currentlyRunningTest;
        private string _testProgressPercentage;

        public Visibility NumberOfProcessedRecordsVisibility
        {
            get => _numberOfProcessedRecordsVisibility;
            set => SetProperty(ref _numberOfProcessedRecordsVisibility, value);
        }

        public Visibility ProcessingFileVisibility
        {
            get => _processingFileVisibility;
            set => SetProperty(ref _processingFileVisibility, value);
        }

        public Visibility AddmlDataObjectStatusVisibility
        {
            get => _addmlDataObjectStatusVisibilty;
            set => SetProperty(ref _addmlDataObjectStatusVisibilty, value);
        }

        public Visibility AddmlFlatFileStatusVisibility
        {
            get => _addmlFlatFileStatusVisibilty;
            set => SetProperty(ref _addmlFlatFileStatusVisibilty, value);
        }

        public string CurrentlyRunningTest
        {
            get => _currentlyRunningTest;
            set => SetProperty(ref _currentlyRunningTest, value);
        }

        public int NumberOfTestsFinished
        {
            get => _numberOfTestsFinished;
            set => SetProperty(ref _numberOfTestsFinished, value);
        }

        public string TestProgressPercentage
        {
            get => _testProgressPercentage;
            set => SetProperty(ref _testProgressPercentage, value);
        }

        public string CurrentActivityMessage
        {
            get => _currentActivityMessage;
            set => SetProperty(ref _currentActivityMessage, value);
        }

        public string CurrentlyProcessingFile
        {
            get => _currentlyProcessingFile;
            set => SetProperty(ref _currentlyProcessingFile, value);
        }

        public int NumberOfProcessedFiles
        {
            get => _numberOfProcessedFiles;
            set => SetProperty(ref _numberOfProcessedFiles, value);
        }

        public int NumberOfProcessedRecords
        {
            get => _numberOfProcessedRecords;
            set => SetProperty(ref _numberOfProcessedRecords, value);
        }

        public bool AllTestsSelected
        {
            get => _allTestsSelected;
            set
            {
                SetProperty(ref _allTestsSelected, value);
                foreach (SelectableTest test in SelectableTests)
                    test.IsSelected = value;
            }
        }

        public bool CanSelectTests
        {
            get => _canSelectTests;
            set => SetProperty(ref _canSelectTests, value);
        }

        public ObservableCollection<OperationMessage> OperationMessages
        {
            get => _operationMessages;
            set => SetProperty(ref _operationMessages, value);
        }

        public ObservableCollection<SelectableTest> SelectableTests
        {
            get => _selectableTests;
            set => SetProperty(ref _selectableTests, value);
        }

        public ArchiveInformationStatus ArchiveInformationStatus
        {
            get => _archiveInformationStatus;
            set => SetProperty(ref _archiveInformationStatus, value);
        }

        public Visibility ArchiveCurrentProcessing
        {
            get => _archiveCurrentProcessing;
            set => SetProperty(ref _archiveCurrentProcessing, value);
        }

        public TestRunnerViewModel(ArkadeApi arkadeApi, IRegionManager regionManager,  IStatusEventHandler statusEventHandler)
        {
            _arkadeApi = arkadeApi;
            _regionManager = regionManager;
            _statusEventHandler = statusEventHandler;
            _statusEventHandler.OperationMessageEvent += OnOperationMessageEvent;
            _statusEventHandler.TestStartedEvent += OnTestStartedEvent;
            _statusEventHandler.TestFinishedEvent += OnTestFinishedEvent;
            _statusEventHandler.TestProgressUpdatedEvent += OnTestProgressUpdatedEvent;
            _statusEventHandler.FileProcessStartedEvent += OnFileProcessStartedEvent;
            _statusEventHandler.FileProcessFinishedEvent += OnFileProcessFinishedEvent;
            _statusEventHandler.RecordProcessingStartedEvent += OnRecordProcessingStartedEvent;
            _statusEventHandler.RecordProcessingFinishedEvent += OnRecordProcessingFinishedEvent;
            _statusEventHandler.NewArchiveProcessEvent += OnNewArchiveInformationEvent;
            _statusEventHandler.SiardValidationFinishedEvent += OnSiardValidationFinished;
            _statusEventHandler.ReadXmlEndElementEvent += OnReadXmlEndElementEvent;

            StartTestingCommand = new DelegateCommand(StartTesting, CanStartTestRun);
            RunTestEngineCommand = new DelegateCommand(async () => await Task.Run(() => RunTests()));
            NavigateToCreatePackageCommand = new DelegateCommand(NavigateToCreatePackage, CanCreatePackage);
            NewProgramSessionCommand = new DelegateCommand(ReturnToProgramStart, IsFinishedRunningTests);
            ShowReportCommand = new DelegateCommand(ShowTestReportDialog, CanContinueOperationOnTestRun);
            _allTestsSelected = true;
        }

        private void StartTesting()
        {
            _log.Information("User action: Start testing");

            RunTestEngineCommand.Execute();
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
            if (!UserDialogs.UserConfirmsNewProgramSession())
                return;

            _log.Information("User action: Leave test session and return to load archive window");

            _regionManager.RequestNavigate("MainContentRegion", "LoadArchiveExtraction");
        }


        private void NavigateToCreatePackage()
        {
            _log.Information("User action: Navigate to create package window");

            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("TestSession", _testSession);
            navigationParameters.Add("archiveFileName", _archiveFileName);
            _regionManager.RequestNavigate("MainContentRegion", "CreatePackage", navigationParameters);
        }

        private bool CanStartTestRun()
        {
            return _testSession != null && _testSession.IsTestableArchive(out _) && !_testRunHasBeenExecuted;
        }

        private bool CanCreatePackage()
        {
            return !_isRunningTests;
        }

        private bool IsFinishedRunningTests()
        {
            return !_isRunningTests;
        }

        private bool CanContinueOperationOnTestRun()
        {
            return IsFinishedRunningTests() && _testRunCompletedSuccessfully;
        }

        public void OnNavigatedTo(NavigationContext context)
        {
            try
            {
                _archiveType = (ArchiveType) context.Parameters["archiveType"];
                _archiveFileName = (string) context.Parameters["archiveFileName"];

                _testSession = Directory.Exists(_archiveFileName)
                    ? _arkadeApi.CreateTestSession(ArchiveDirectory.Read(_archiveFileName, _archiveType))
                    : _arkadeApi.CreateTestSession(ArchiveFile.Read(_archiveFileName, _archiveType));

                if (!_testSession.IsTestableArchive(out string disqualifyingCause))
                    LogNotTestableArchiveOperationMessage(disqualifyingCause);

                if (_testSession.Archive.ArchiveType == ArchiveType.Noark5)
                {
                    SupportedLanguage uiLanguage = LanguageSettingHelper.GetUILanguage();

                    foreach (TestId testId in Noark5TestProvider.GetAllTestIds())
                    {
                        _selectableTests.Add(new SelectableTest
                        {
                            TestId = testId,
                            DisplayName = ArkadeTestNameProvider.GetDisplayName(testId, uiLanguage),
                            IsSelected = true
                        });
                    }

                    CanSelectTests = true;
                }


                StartTestingCommand.RaiseCanExecuteChanged(); // testSession has been updated, reevaluate command
            }
            catch (SiardArchiveReaderException siardArchiveReaderException)
            {
                Log.Error(siardArchiveReaderException, siardArchiveReaderException.Message);
                LogNotTestableArchiveOperationMessage(siardArchiveReaderException.Message);
            }
            catch (Exception e)
            {
                string message = string.Format(TestRunnerGUI.ErrorReadingArchive, e.Message);
                Log.Error(e, message);
                _statusEventHandler.RaiseEventOperationMessage(null, message, OperationMessageStatus.Error);
                if (e is ArkadeException)
                    LogNotTestableArchiveOperationMessage(TestRunnerGUI.ValidSpecificationFileNotFound);
            }
        }

        private void LogNotTestableArchiveOperationMessage(string disqualifyingCause)
        {
            string notTestableArchiveMessage = string.Format(TestRunnerGUI.ArchiveNotTestable, disqualifyingCause, ArkadeProcessingArea.LogsDirectory);

            _statusEventHandler.RaiseEventOperationMessage(
                TestRunnerGUI.ArchiveTestability,
                notTestableArchiveMessage,
                OperationMessageStatus.Info
            );
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            MainWindow.ProgressBarWorker.ReportProgress(0, "reset");
        }

        private void OnSiardValidationFinished(object sender, SiardValidationEventArgs eventArgs)
        {
            var noErrors = true;

            foreach (string errorOrWarningMsg in eventArgs.Errors.Where(e => e != null))
            {
                if (!errorOrWarningMsg.StartsWith("WARN"))
                {
                    _statusEventHandler.RaiseEventOperationMessage(errorOrWarningMsg, string.Empty,
                        OperationMessageStatus.Error);
                    noErrors = false;
                }
                else if (!ArkadeConstants.SuppressedDbptkWarningMessages.Contains(errorOrWarningMsg))
                {
                    _statusEventHandler.RaiseEventOperationMessage(errorOrWarningMsg, string.Empty,
                        OperationMessageStatus.Warning);
                }
            }

            if (noErrors)
                _statusEventHandler.RaiseEventOperationMessage(TestRunnerGUI.SiardProgressMessage,
                    TestRunnerGUI.MessageCompleted, OperationMessageStatus.Ok);
        }

        private void OnOperationMessageEvent(object sender, OperationMessageEventArgs eventArgs)
        {
            UpdateOperationMessageList(eventArgs);
        }

        private void OnFileProcessStartedEvent(object sender, FileProcessingStatusEventArgs eventArgs)
        {
            CurrentlyProcessingFile = eventArgs.FileName;
        }

        private void OnFileProcessFinishedEvent(object sender, FileProcessingStatusEventArgs eventArgs)
        {
            NumberOfProcessedFiles = NumberOfProcessedFiles + 1;
        }

        private void OnTestProgressUpdatedEvent(object sender, TestProgressEventArgs eventArgs)
        {
            if (eventArgs.HasFailed)
            {
                _statusEventHandler.RaiseEventOperationMessage(
                    _archiveType == ArchiveType.Siard
                        ? TestRunnerGUI.SiardProgressMessage
                        : TestRunnerGUI.EventIdFinishedWithError,
                    eventArgs.FailMessage, OperationMessageStatus.Error);
                _testRunHasFailed = true;
            }
            
            else
                TestProgressPercentage = eventArgs.TestProgress;
        }
        
        private void OnRecordProcessingStartedEvent(object sender, EventArgs eventArgs)
        {
            _isProcessingRecord = true;
        }
        private void OnRecordProcessingFinishedEvent(object sender, EventArgs eventArgs)
        {
            if (_isProcessingRecord)
                NumberOfProcessedRecords = NumberOfProcessedRecords + 1;

            _isProcessingRecord = false;
        }

        private void OnReadXmlEndElementEvent(object sender, EventArgs eventArgs)
        {
            NumberOfProcessedRecords = NumberOfProcessedRecords + 1;
        }

        private void OnNewArchiveInformationEvent(object sender, ArchiveInformationEventArgs eventArgs)
        {
            ArchiveInformationStatus.Update(eventArgs);
            ArchiveCurrentProcessing = Visibility.Visible;

            Enum.TryParse(eventArgs.ArchiveType, out ArchiveType archiveType);

            switch (archiveType)
            {
                case ArchiveType.Noark5:
                    AddmlDataObjectStatusVisibility = Visibility.Visible;
                    ProcessingFileVisibility = Visibility.Visible;
                    NumberOfProcessedRecordsVisibility = Visibility.Visible;
                    break;
                case ArchiveType.Noark3 or ArchiveType.Fagsystem:
                    AddmlFlatFileStatusVisibility = Visibility.Visible;
                    ProcessingFileVisibility = Visibility.Visible;
                    NumberOfProcessedRecordsVisibility = Visibility.Visible;
                    break;
            }
        }

        private void RunTests()
        {
            try
            {
                NotifyStartRunningTests();

                _testSession.TestsToRun = GetSelectedTests();
                
                _testSession.OutputLanguage = LanguageSettingHelper.GetOutputLanguage();

                _arkadeApi.RunTests(_testSession);

                _testSession.AddLogEntry("Test run completed.");

                if (_testRunHasFailed)
                {
                    NotifyFinishedRunningTests();
                    return;
                }

                SaveTestReports(_testSession.Archive.GetTestReportDirectory());

                _testRunCompletedSuccessfully = true;
                _statusEventHandler.RaiseEventOperationMessage(TestRunnerGUI.EventIdFinishedOperation, null, OperationMessageStatus.Ok);
                NotifyFinishedRunningTests();
            }
            catch (ArkadeException e)
            {
                _testSession?.AddLogEntry("Test run failed: " + e.Message);
                _log.Error(e.Message, e);
                _statusEventHandler.RaiseEventTestProgressUpdated(string.Empty, true, e.Message);
                NotifyFinishedRunningTests();
            }
            catch (Exception e)
            {
                _testSession?.AddLogEntry("Test run failed: " + e.Message);
                _log.Error(e.Message, e);

                var operationMessageBuilder = new StringBuilder();

                if (e.GetType() == typeof(FileNotFoundException))
                {
                    string nameOfMissingFile = new FileInfo(((FileNotFoundException) e).FileName).Name;
                    operationMessageBuilder.Append(string.Format(TestRunnerGUI.FileNotFoundMessage, nameOfMissingFile));
                }
                else
                {
                    operationMessageBuilder.Append(e.Message);
                }

                string fileName = new DetailedExceptionMessage(e).WriteToFile();

                if (!string.IsNullOrEmpty(fileName))
                    operationMessageBuilder.AppendLine("\n" + string.Format(TestRunnerGUI.DetailedErrorMessageInfo, fileName));

                string operationMessage = operationMessageBuilder.ToString();

                _statusEventHandler.RaiseEventOperationMessage(
                    TestRunnerGUI.EventIdFinishedWithError, operationMessage, OperationMessageStatus.Error
                );

                NotifyFinishedRunningTests();
            }
        }

        private List<TestId> GetSelectedTests()
        {
            var selectedTests = new List<TestId>();

            foreach (SelectableTest selectableTest in SelectableTests)
            {
                if(selectableTest.IsSelected)
                    selectedTests.Add(selectableTest.TestId);
            }

            return selectedTests;
        }

        private void NotifyFinishedRunningTests()
        {
            _isRunningTests = false;
            ShowReportCommand.RaiseCanExecuteChanged();
            NavigateToCreatePackageCommand.RaiseCanExecuteChanged();
            NewProgramSessionCommand.RaiseCanExecuteChanged();
            
            MainWindow.ProgressBarWorker.ReportProgress(100);
        }

        private void NotifyStartRunningTests()
        {
            _testRunHasBeenExecuted = true;
            CanSelectTests = false;
            _isRunningTests = true;
            StartTestingCommand.RaiseCanExecuteChanged();
            ShowReportCommand.RaiseCanExecuteChanged();
            NavigateToCreatePackageCommand.RaiseCanExecuteChanged();
            NewProgramSessionCommand.RaiseCanExecuteChanged();

            ArkadeProcessingState.TestingIsStarted = true;
            MainWindowViewModel.ShowSettingsCommand.RaiseCanExecuteChanged();

            MainWindow.ProgressBarWorker.ReportProgress(0);
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

        private void ShowTestReportDialog()
        {
            new TestReportDialog(_testSession.Archive.GetTestReportDirectory(), _testSession.InputDiasPackageId).ShowDialog(); // NB! UUID-transfer
        }

        private void SaveTestReports(DirectoryInfo testReportDirectory)
        {
            string eventId = TestRunnerGUI.EventIdCreatingReport;
            _statusEventHandler.RaiseEventOperationMessage(eventId, null, OperationMessageStatus.Started);

            _arkadeApi.SaveReport(_testSession, testReportDirectory, false, Settings.Default.TestResultDisplayLimit);

            _statusEventHandler.RaiseEventOperationMessage(eventId, TestRunnerGUI.TestReportIsSavedMessage, OperationMessageStatus.Ok);
        }

    }
}
