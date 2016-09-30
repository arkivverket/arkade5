using System.Windows.Media;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class TestRunViewModel
    {
        private readonly TestRun _testRun;

        private readonly SolidColorBrush _colorSuccess = new SolidColorBrush(Color.FromRgb(76, 175, 80));
        private readonly SolidColorBrush _colorFailure = new SolidColorBrush(Color.FromRgb(244, 67, 54));

        public TestRunViewModel(TestRun testRun)
        {
            _testRun = testRun;
        }

        public string Name => _testRun.TestName;
        public double Duration => _testRun.TestDuration;
        public string ResultAsLabel => _testRun.IsSuccess() ? "OK" : "Feil";
        public SolidColorBrush ResultAsColor => _testRun.IsSuccess() ? _colorSuccess : _colorFailure;
        public string ResultIcon => _testRun.IsSuccess() ? "Check" : "Alert";

        public string Message
        {
            get
            {
                if (_testRun.Results != null && _testRun.Results.Count > 0)
                    return _testRun.Results[0].Message + " (Kjøretid: " + Duration + " ms)";
                else
                    return null;
            }
        } 

    }
}
