using System.Collections.Generic;
using System.Text;

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

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Test: ").AppendLine(TestName);
            builder.Append("Test type: ").AppendLine(TestType.ToString());
            builder.Append("IsSuccess: ").AppendLine(IsSuccess().ToString());
            builder.AppendLine("Results: ");

            foreach (TestResult result in Results)
            {
                builder.AppendLine(result.ToString());
            }

            return builder.ToString();
        }

    }
}
