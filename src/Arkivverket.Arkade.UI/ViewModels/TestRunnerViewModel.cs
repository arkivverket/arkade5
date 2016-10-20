using System.Collections.ObjectModel;
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

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class TestRunnerViewModel : BindableBase, INavigationAware
    {
        private ILogger _log = Log.ForContext<TestRunnerViewModel>();

        private ObservableCollection<TestRunnerStatus> _testResults = new ObservableCollection<TestRunnerStatus>();

        private readonly TestSessionFactory _testSessionBuilder;
        private readonly TestEngineFactory _testEngineFactory;
        private readonly IRegionManager _regionManager;
        public DelegateCommand NavigateToSummaryCommand { get; set; }
        private DelegateCommand RunTestEngineCommand { get; set; }
        private string _metadataFileName;
        private string _archiveFileName;
        private TestSession _testSession;
        private bool _isRunningTests;
        private Visibility _finishedTestingMessageVisibility = Visibility.Collapsed;

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

        public TestRunnerViewModel(TestSessionFactory testSessionBuilder, TestEngineFactory testEngineFactory, IRegionManager regionManager)
        {
            _testSessionBuilder = testSessionBuilder;
            _testEngineFactory = testEngineFactory;
            _regionManager = regionManager;



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
            UpdateGuiCollection(TestRunnerStatus.TestExcecutionStatus.Executing, eventArgs.TestName);
        }

        private void TestEngineOnTestFinished(object sender, TestFinishedEventArgs eventArgs)
        {
            TestRunnerStatus.TestExcecutionStatus exeStatus = TestRunnerStatus.TestExcecutionStatus.Failed;
            if (eventArgs.IsSuccess)
            {
                exeStatus = TestRunnerStatus.TestExcecutionStatus.Passed;
            }
            UpdateGuiCollection(exeStatus, eventArgs.TestName, eventArgs.ResultMessage);
        }

        private void RunTests()
        {
            _log.Debug("Issued the RunTests command");

            _isRunningTests = true;
            NavigateToSummaryCommand.RaiseCanExecuteChanged();

            _testSession = _testSessionBuilder.NewSessionFromTarFile(_archiveFileName, _metadataFileName);

            _log.Debug(_testSession.Archive.Uuid.GetValue());
            _log.Debug(_testSession.Archive.ArchiveType.ToString());
            _log.Debug(_testSession.Archive.WorkingDirectory.Name);


            ITestEngine testEngine = _testEngineFactory.GetTestEngine(_testSession);
            testEngine.TestStarted += TestEngineOnTestStarted;
            testEngine.TestFinished += TestEngineOnTestFinished;

            _testSession.TestSuite = testEngine.RunTestsOnArchive(_testSession);

            TestSessionXmlGenerator.GenerateXmlAndSaveToFile(_testSession);

            _isRunningTests = false;
            FinishedTestingMessageVisibility = Visibility.Visible;
            NavigateToSummaryCommand.RaiseCanExecuteChanged();
        }


        private void UpdateGuiCollection(TestRunnerStatus.TestExcecutionStatus status, string testName, string resultMessage = null)
        {
            // http://stackoverflow.com/questions/18331723/this-type-of-collectionview-does-not-support-changes-to-its-sourcecollection-fro
            Application.Current.Dispatcher.Invoke(delegate
            {

                if (status == TestRunnerStatus.TestExcecutionStatus.Executing)
                {
                    var testRunnerStatus = new TestRunnerStatus(TestRunnerStatus.TestExcecutionStatus.Executing, testName);
                    TestResults.Add(testRunnerStatus);
                }
                else
                {
                    var item = TestResults.FirstOrDefault(i => i.TestName == testName);
                    if (item != null)
                    {
                        item.Update(status);

                        if (resultMessage != null)
                            item.ResultMessage = resultMessage;
                    }
                }
            });
        }

    }
}
