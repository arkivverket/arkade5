using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.UI.Models;
using Arkivverket.Arkade.UI.Views;
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
        public DelegateCommand NavigateToSummaryCommand { get; set; }
        private DelegateCommand RunTestEngineCommand { get; set; }
        private string _metadataFileName;
        private string _archiveFileName;
        private TestSession _testSession;
        private bool _isRunningTests;


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
            UpdateGuiCollection(TestRunnerStatus.TestExcecutionStatus.Executing, eventArgs.TestName);
        }

        private static string GetUserFriendlyTestName(TestStartedEventArgs eventArgs)
        {
            return Resources.UI.ResourceManager.GetString("TestName_" + eventArgs.TestName);
        }

        private void TestEngineOnTestFinished(object sender, TestFinishedEventArgs eventArgs)
        {
            if (eventArgs.IsSuccess)
            {
                UpdateGuiCollection(TestRunnerStatus.TestExcecutionStatus.Passed, eventArgs.TestName);
            }
            else
            {
                UpdateGuiCollection(TestRunnerStatus.TestExcecutionStatus.Failed, eventArgs.TestName);
            }
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
            UpdateGuiCollection(TestRunnerStatus.TestExcecutionStatus.Ended, "");
            _isRunningTests = false;
            NavigateToSummaryCommand.RaiseCanExecuteChanged();
        }


        private void UpdateGuiCollection(TestRunnerStatus.TestExcecutionStatus status, string testName)
        {
            // http://stackoverflow.com/questions/18331723/this-type-of-collectionview-does-not-support-changes-to-its-sourcecollection-fro
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                string icon;
                string descript;
                var item = TestResults.FirstOrDefault(i => i.TestName == testName);

                if (status == TestRunnerStatus.TestExcecutionStatus.Executing)
                {
                    TestResults.Add(new TestRunnerStatus(TestRunnerStatus.TestExcecutionStatus.Executing, testName));
                } else if (status == TestRunnerStatus.TestExcecutionStatus.Ended)
                {
                    TestResults.Add(new TestRunnerStatus(TestRunnerStatus.TestExcecutionStatus.Ended, testName));
                }
                else if (item != null && status == TestRunnerStatus.TestExcecutionStatus.Passed)
                {
                    TestRunnerStatus.SelectIconForTestStatus(TestRunnerStatus.TestExcecutionStatus.Passed, out icon, out descript);
                    item.TestStatusIcon = icon;
                    item.TestStatusDescription = descript;
                }
                else if (item != null && status == TestRunnerStatus.TestExcecutionStatus.Failed)
                {
                    TestRunnerStatus.SelectIconForTestStatus(TestRunnerStatus.TestExcecutionStatus.Failed, out icon, out descript);
                    item.TestStatusIcon = icon;
                    item.TestStatusDescription = descript;
                }
                else
                {
                    TestResults.Add(new TestRunnerStatus(TestRunnerStatus.TestExcecutionStatus.Error, testName));
                }

            });

        }



    }
}
