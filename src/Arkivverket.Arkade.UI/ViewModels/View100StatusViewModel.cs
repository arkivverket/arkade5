using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using Arkivverket.Arkade.Core;
using Prism.Mvvm;

namespace Arkivverket.Arkade.UI.ViewModels
{
    public class View100StatusViewModel : BindableBase
    {
        private readonly TestEngine _testEngine;

        private string _logString;
        public string LogString
        {
            get { return _logString; }
            set { SetProperty(ref _logString, value); }
        }

        private ObservableCollection<TestResultsArrivedEventArgs> _testResults = new ObservableCollection<TestResultsArrivedEventArgs>();

        public ObservableCollection<TestResultsArrivedEventArgs> TestResults
        {
            get { return _testResults; }
            set { SetProperty(ref _testResults, value); }
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

            // http://stackoverflow.com/questions/18331723/this-type-of-collectionview-does-not-support-changes-to-its-sourcecollection-fro
            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                TestResults.Add(eventArgs);
            });

        }
    }
}