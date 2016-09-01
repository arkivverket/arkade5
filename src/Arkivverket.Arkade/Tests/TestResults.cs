using System.Collections.Generic;

namespace Arkivverket.Arkade.Tests
{
    public class TestResults
    {
        public List<TestResult> Results { get; set; }
        public double TestDuration { get; set; }
        public string TestName { get; set; }
        public TestType TestType { get; set; }

        public TestResults(string testName, TestType testType)
        {
            Results = new List<TestResult>();
            TestName = testName;
            TestType = testType;
        }

        public void Add(TestResult result)
        {
            Results.Add(result);
        }

        public bool IsSuccess()
        {
            bool success = true;
            foreach (TestResult result in Results)
            {
                if (result.IsError())
                {
                    success = false;
                    break;
                }
            }
            return success;
        }

    }
}
