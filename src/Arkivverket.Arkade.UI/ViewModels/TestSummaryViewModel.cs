using System.Collections.ObjectModel;
using Arkivverket.Arkade.Core;
using Prism.Mvvm;
using Prism.Regions;

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class TestSummaryViewModel : BindableBase, INavigationAware
    {
        private ObservableCollection<TestRunViewModel> _testRuns;

        private TestSession _testSession;

        public TestSummaryViewModel()
        {
            _testRuns = new ObservableCollection<TestRunViewModel>();
        }

        public TestSession TestSession
        {
            get { return _testSession; }
            set { SetProperty(ref _testSession, value); }
        }

        public ObservableCollection<TestRunViewModel> TestRuns
        {
            get { return _testRuns; }
            set { SetProperty(ref _testRuns, value); }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            TestSession = (TestSession) navigationContext.Parameters["TestSession"];
            TestSession.TestSuite.TestRuns.ForEach(CreateViewModel);
        }

        private void CreateViewModel(TestRun testRun)
        {
            _testRuns.Add(new TestRunViewModel(testRun));
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}