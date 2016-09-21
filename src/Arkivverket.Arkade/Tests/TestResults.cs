using System.Collections.Generic;
using System.Text;

namespace Arkivverket.Arkade.Tests
{
    public class TestResults
    {
        public Dictionary<string, string> AnalysisResults = new Dictionary<string, string>();

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
            var success = true;
            foreach (var result in Results)
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
            var builder = new StringBuilder();
            builder.Append("Test: ").AppendLine(TestName);
            builder.Append("Test type: ").AppendLine(TestType.ToString());
            builder.Append("IsSuccess: ").AppendLine(IsSuccess().ToString());
            builder.AppendLine("Results: ");

            foreach (var result in Results)
            {
                builder.AppendLine(result.ToString());
            }

            return builder.ToString();
        }

        public void AddAnalysisResult(string key, string value)
        {
            AnalysisResults.Add(key, value);
        }
    }
}