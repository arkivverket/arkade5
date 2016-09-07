using System.Diagnostics;
using Arkivverket.Arkade.Core;
using Prism.Mvvm;

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class View100StatusViewModel : BindableBase
    {
        private readonly TestEngine _testEngine;

        // for the two way binding to work, this *must* be a regular field, not an auto property (get;set;)
        private string _logString;
        public string LogString
        {
            get { return _logString; }
            set { SetProperty(ref _logString, value); }
        }

        public View100StatusViewModel(TestEngine testEngine)
        {
            _testEngine = testEngine;
            _testEngine.TestResultsArrived += TestEngineOnTestResultsArrived;
        }

        private void TestEngineOnTestResultsArrived(object sender, TestResultsArrivedEventArgs eventArgs)
        {
            string msg = $"{eventArgs.TestName} isSuccess={eventArgs.IsSuccess}";
            LogString = $"{LogString}\n{msg}";
            Debug.Print(msg);
        }
    }
}