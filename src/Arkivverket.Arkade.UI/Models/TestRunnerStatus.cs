using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace Arkivverket.Arkade.UI.Models
{
    public class TestRunnerStatus : BindableBase
    {
        private string _testStatusIcon;
        public string TestStatusIcon
        {
            get { return _testStatusIcon; }
            set { SetProperty(ref _testStatusIcon, value); }
        }

        private string _testStatusDescription;

        public string TestStatusDescription
        {
            get { return _testStatusDescription; }
            set { SetProperty(ref _testStatusDescription, value); }
        }
        public string TestName { get; set; }

        private static string _iconDefinitionStringSuccess = "CheckboxMarkedOutline";
        private static string _iconDefinitionStringFail = "AlertBox";
        private static string _iconDefinitionStringExecuting = "Autorenew";
        private static string _iconDefinitionStringFinished = "ClockEnd";
        private static string _iconDefinitionStringËrror = "Bug";


        public TestRunnerStatus(TestExcecutionStatus testStatus, string testName)
        {
            string testStatusIcon;
            string testStatusDescription;
            SelectIconForTestStatus(testStatus, out testStatusIcon, out testStatusDescription);

            TestName = testName;
            TestStatusIcon = testStatusIcon;
            TestStatusDescription = testStatusDescription;
        }


        public static void SelectIconForTestStatus(TestExcecutionStatus status, out string icon, out string description)
        {
            icon = "Bug";
            description = "NA";
            if (status == TestExcecutionStatus.Executing)
            {
                icon = _iconDefinitionStringExecuting;
                description = "Kjører";
            }
            if (status == TestExcecutionStatus.Failed)
            {
                icon = _iconDefinitionStringFail;
                description = "Fullført med feil";
            }
            if (status == TestExcecutionStatus.Passed)
            {
                icon = _iconDefinitionStringSuccess;
                description = "Fullført";
            }
            if (status == TestExcecutionStatus.Ended)
            {
                icon = _iconDefinitionStringFinished;
                description = "Ferdig med kjøring";
            }
            if (status == TestExcecutionStatus.Error)
            {
                icon = _iconDefinitionStringËrror;
                description = "Feil i rapportering";
            }
        }


        public enum TestExcecutionStatus
        {
            Executing,
            Passed,
            Failed,
            Ended,
            Error
        }



    }
}
