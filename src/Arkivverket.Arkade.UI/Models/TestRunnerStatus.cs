using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.UI.Models
{
    public class TestRunnerStatus
    {
        public string TestStatusIcon { get; set; }
        public string TestNameReadable { get; set; }
        public string TestDurationSec { get; set; }

        private string _iconDefinitionStringSuccess = "CheckboxMarkedOutline";
        private string _iconDefinitionStringFail = "AlertBox";

        public TestRunnerStatus(bool testStatus, string testName, string testDurationSec)
        {
            TestStatusIcon = SelectIconForTestStatus(testStatus);
            TestNameReadable = GetUserFriendlyTestName(testName);
            TestDurationSec = testDurationSec;
        }


        private string SelectIconForTestStatus(bool status)
        {
            string icon;
            icon = status ? _iconDefinitionStringSuccess : _iconDefinitionStringFail;
            return icon;
        }

        private string GetUserFriendlyTestName(string testName)
        {
            int pos = testName.LastIndexOf(".", StringComparison.Ordinal) + 1;
            return Resources.UI.ResourceManager.GetString("TestName_" + testName.Substring(pos, testName.Length - pos));
        }


    }
}
