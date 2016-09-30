using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Identify;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class TestRunnerViewModel : BindableBase, INavigationAware
    {

        private ObservableCollection<TestFinishedEventArgs> _testResults = new ObservableCollection<TestFinishedEventArgs>();

        private readonly TestSessionFactory _testSessionBuilder;
        private readonly TestEngine _testEngine;
        private bool _isRunningTests;

        private DelegateCommand RunTestEngineCommand { get; set; }
        private string _metadataFileName;
        private string _archiveFileName;

        private Operation _operation;

        public Operation Operation
        {
            get { return _operation; }
            set { SetProperty(ref _operation, value); }
        }

        public ObservableCollection<TestFinishedEventArgs> TestResults
        {
            get { return _testResults; }
            set { SetProperty(ref _testResults, value); }
        }

        public TestRunnerViewModel(TestSessionFactory testSessionBuilder, TestEngine testEngine)
        {
            _testSessionBuilder = testSessionBuilder;
            _testEngine = testEngine;
            _testEngine.TestStarted += TestEngineOnTestStarted;
            _testEngine.TestFinished += TestEngineOnTestFinished;
            RunTestEngineCommand = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => RunTests()));

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
                TestResults.Add(eventArgs);
            });

        }

        private void RunTests()
        {
            Log.Debug("Issued the RunTests command");

            TestSession testSession = _testSessionBuilder.NewSessionFromTarFile(_archiveFileName, _metadataFileName);

            Log.Debug(testSession.Archive.Uuid.GetValue());
            Log.Debug(testSession.Archive.ArchiveType.ToString());
            Log.Debug(testSession.Archive.WorkingDirectory.Name);

            testSession.TestSuite = _testEngine.RunTestsOnArchive(testSession);
        }

        
    }

    public class Operation
    {
        public string OperationName { get; set; }
        public DateTime StartTime { get; set; }
    }
}
