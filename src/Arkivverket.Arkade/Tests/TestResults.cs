using System.Collections.Generic;

namespace Arkivverket.Arkade.Tests
{
    public class TestResults
    {
        public List<TestResult> Results { get; set; }

        public TestResults()
        {
            Results = new List<TestResult>();
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
