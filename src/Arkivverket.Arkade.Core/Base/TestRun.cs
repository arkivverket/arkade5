using System;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base
{
    public class TestRun : IComparable
    {
        private readonly IArkadeTest _test;
        public TestId TestId => _test.GetId();
        public string TestName => ArkadeTestNameProvider.GetDisplayName(_test);
        public TestType TestType => _test.GetTestType();
        public string TestDescription => _test.GetDescription();
        public TestResultSet TestResults { get; set; }
        public long TestDuration { get; set; }

        public TestRun(IArkadeTest test)
        {
            _test = test;

            TestResults = new TestResultSet();
        }

        public void Add(TestResult testResult, string testResultSetName="")
        {
            if (testResultSetName == "")
                TestResults.TestsResults.Add(testResult);
            else
                TestResults.FindFirst(testResultSetName).TestsResults.Add(testResult);
            
        }

        public bool IsSuccess()
        {
            return TestResults.IsSuccess();
        }

        public int FindNumberOfErrors()
        {
            return TestResults.FindNumberOfErrors();
        }

        public int CompareTo(object obj)
        {
            var testRun = (TestRun) obj;

            return _test.CompareTo(testRun._test);
        }
    }
}
