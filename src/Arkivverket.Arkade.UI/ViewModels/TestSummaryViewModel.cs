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

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class TestSummaryViewModel : BindableBase, INavigationAware
    {

        private ObservableCollection<TestResultsArrivedEventArgs> _testResults = new ObservableCollection<TestResultsArrivedEventArgs>();
        private readonly ArchiveExtractionReader _archiveExtractionReader;
        private readonly TestEngine _testEngine;
        private bool _isRunningTests;
        private DelegateCommand RunTestEngineCommand { get; set; }
            string metadataFileName;
            string archiveFileName;

        public ObservableCollection<TestResultsArrivedEventArgs> TestResults
        {
            get { return _testResults; }
            set { SetProperty(ref _testResults, value); }
        }
        public TestSummaryViewModel(ArchiveExtractionReader archiveExtractionReader, TestEngine testEngine)
        {
            _archiveExtractionReader = archiveExtractionReader;
            _testEngine = testEngine;
            _testEngine.TestResultsArrived += TestEngineOnTestResultsArrived;

            Debug.Print("TEST SUMMARY!!!!");
            RunTestEngineCommand = DelegateCommand.FromAsyncHandler(async () => await Task.Run(() => RunTests()));

        }
        public void OnNavigatedTo(NavigationContext context)
        {
            Debug.Print("NavigatedTo");
            metadataFileName = (string)context.Parameters["metadataFileName"];
            archiveFileName = (string)context.Parameters["archiveFileName"];

            Debug.Print("metadatafilename: " + metadataFileName);

            RunTestEngineCommand.Execute();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private void TestEngineOnTestResultsArrived(object sender, TestResultsArrivedEventArgs eventArgs)
        {
            string msg = $"{eventArgs.TestName} isSuccess={eventArgs.IsSuccess}";

            Debug.Print(msg);

            // http://stackoverflow.com/questions/18331723/this-type-of-collectionview-does-not-support-changes-to-its-sourcecollection-fro
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                TestResults.Add(eventArgs);
            });

        }

        private void RunTests()
        {
            Debug.Print("Issued the RunTests command");

            Archive archive = _archiveExtractionReader.ReadFromFile(archiveFileName, metadataFileName);

            Debug.Print(archive.Uuid);
            Debug.Print(archive.ArchiveType.ToString());
            Debug.Print(archive.WorkingDirectory);

            _testEngine.RunTestsOnArchive(archive);
        }

        
    }
}
