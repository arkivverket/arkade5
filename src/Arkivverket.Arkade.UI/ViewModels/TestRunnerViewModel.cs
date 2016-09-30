using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.UI.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class TestRunnerViewModel : BindableBase, INavigationAware
    {

        private ObservableCollection<TestRunnerStatus> _testResults = new ObservableCollection<TestRunnerStatus>();

        private readonly TestSessionFactory _testSessionBuilder;
        private readonly TestEngine _testEngine;
        private readonly IRegionManager _regionManager;
        private bool _isRunningTests;
        public DelegateCommand NavigateToSummaryCommand { get; set; }
        private DelegateCommand RunTestEngineCommand { get; set; }
        private string _metadataFileName;
        private string _archiveFileName;
        private TestSession _testSession;
        private Operation _operation;

        public Operation Operation
        {
            get { return _operation; }
            set { SetProperty(ref _operation, value); }
        }

        public ObservableCollection<TestRunnerStatus> TestResults
        {
            get { return _testResults; }
            set { SetProperty(ref _testResults, value); }
        }

        public TestRunnerViewModel(TestSessionFactory testSessionBuilder, TestEngine testEngine, IRegionManager regionManager)
        {
            _testSessionBuilder = testSessionBuilder;
            _testEngine = testEngine;
            _regionManager = regionManager;

            _testEngine.TestStarted += TestEngineOnTestStarted;
            _testEngine.TestFinished += TestEngineOnTestFinished;

            RunTestEngineCommand = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => RunTests()));
            NavigateToSummaryCommand = new DelegateCommand(NavigateToSummary, CanNavigateToSummary);
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

        private void TestEngineOnTestStarted(object sender, TestStartedEventArgs eventArgs)
        {
            string userFriendlyName = GetUserFriendlyTestName(eventArgs);

            Operation = new Operation()
            {
                OperationName = userFriendlyName,
                StartTime = eventArgs.StartTime
            };
        }

        private static string GetUserFriendlyTestName(TestStartedEventArgs eventArgs)
        {
            return Resources.UI.ResourceManager.GetString("TestName_" + eventArgs.TestName);
        }

        private void TestEngineOnTestFinished(object sender, TestFinishedEventArgs eventArgs)
        {
            // http://stackoverflow.com/questions/18331723/this-type-of-collectionview-does-not-support-changes-to-its-sourcecollection-fro
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                TimeSpan timeSpan = DateTime.Now - Operation.StartTime;
                TestResults.Add(new TestRunnerStatus(eventArgs.IsSuccess, eventArgs.TestName,timeSpan.Seconds.ToString()));
            });

        }

        private void RunTests()
        {
            Log.Debug("Issued the RunTests command");
            _isRunningTests = true;
            NavigateToSummaryCommand.RaiseCanExecuteChanged();

            _testSession = _testSessionBuilder.NewSessionFromTarFile(_archiveFileName, _metadataFileName);

            Log.Debug(_testSession.Archive.Uuid.GetValue());
            Log.Debug(_testSession.Archive.ArchiveType.ToString());
            Log.Debug(_testSession.Archive.WorkingDirectory.Name);

            _testSession.TestSuite = _testEngine.RunTestsOnArchive(_testSession);

            _isRunningTests = false;
            NavigateToSummaryCommand.RaiseCanExecuteChanged();
        }
    }

    public class Operation
    {
        public string OperationName { get; set; }
        public DateTime StartTime { get; set; }
    }
}
