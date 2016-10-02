using System.Windows;
using System.Windows.Media;
using Prism.Mvvm;

namespace Arkivverket.Arkade.UI.Models
{
    public class TestRunnerStatus : BindableBase
    {
        public enum TestExcecutionStatus
        {
            Executing,
            Passed,
            Failed,
            Ended,
            Error
        }

        private readonly SolidColorBrush _colorFailed = new SolidColorBrush(Color.FromRgb(244, 67, 54));

        private readonly SolidColorBrush _colorSuccess = new SolidColorBrush(Color.FromRgb(76, 175, 80));

        private string _resultMessage;

        private Visibility _progressBarVisibility = Visibility.Visible;

        private SolidColorBrush _resultAsColor;

        private string _resultAsIcon;

        private string _resultAsLabel;

        private Visibility _testResultsVisibility = Visibility.Collapsed;

        public string ResultMessage
        {
            get { return _resultMessage; }
            set { SetProperty(ref _resultMessage, value); }
        }

        public Visibility ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set { SetProperty(ref _progressBarVisibility, value); }
        }

        public Visibility TestResultsVisibility
        {
            get { return _testResultsVisibility; }
            set { SetProperty(ref _testResultsVisibility, value); }
        }

        public string TestName { get; set; }

        public string ResultAsIcon
        {
            get { return _resultAsIcon; }
            set { SetProperty(ref _resultAsIcon, value); }
        }

        public string ResultAsLabel
        {
            get { return _resultAsLabel; }
            set { SetProperty(ref _resultAsLabel, value); }
        }

        public SolidColorBrush ResultAsColor
        {
            get { return _resultAsColor; }
            set { SetProperty(ref _resultAsColor, value); }
        }

        public TestRunnerStatus(TestExcecutionStatus testStatus, string testName)
        {
            TestName = testName;
            Update(testStatus);
        }


        public void Update(TestExcecutionStatus executionStatus)
        {
            if (executionStatus == TestExcecutionStatus.Passed || executionStatus == TestExcecutionStatus.Failed)
            {
                ShowTestResults();
            }
            else
            {
                ShowProgressBar();
            }

            if (executionStatus == TestExcecutionStatus.Passed)
            {
                ResultAsColor = _colorSuccess;
                ResultAsLabel = "OK";
                ResultAsIcon = "Check";
            }
            else if (executionStatus == TestExcecutionStatus.Failed)
            {
                ResultAsColor = _colorFailed;
                ResultAsLabel = "Feil";
                ResultAsIcon = "Alert";
            }
        }

        private void ShowProgressBar()
        {
            ProgressBarVisibility = Visibility.Visible;
            TestResultsVisibility = Visibility.Collapsed;
        }

        private void ShowTestResults()
        {
            TestResultsVisibility = Visibility.Visible;
            ProgressBarVisibility = Visibility.Collapsed;
        }
    }
}