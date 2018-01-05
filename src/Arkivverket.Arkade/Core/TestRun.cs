using Arkivverket.Arkade.Tests;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core
{
    public class TestRun
    {
        private readonly IArkadeTest _test;
        public TestId TestId => _test.GetId();
        public string TestName => _test.GetName();
        public TestType TestType => _test.GetTestType();
        public string TestDescription => _test.GetDescription();
        public List<TestResult> Results { get; set; }
        public long TestDuration { get; set; }

        public TestRun(IArkadeTest test)
        {
            _test = test;

            Results = new List<TestResult>();
        }

        public void Add(TestResult result)
        {
            Results.Add(result);
        }

        public bool IsSuccess()
        {
            return Results.TrueForAll(r => !r.IsError());
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("Test: ").AppendLine(TestName);
            builder.Append("Test type: ").AppendLine(TestType.ToString());
            builder.Append("IsSuccess: ").AppendLine(IsSuccess().ToString());
            builder.AppendLine("Results: ");
            foreach (TestResult result in Results)
                builder.AppendLine(result.ToString());

            return builder.ToString();
        }

        public int FindNumberOfErrors()
        {
            return Results.Count(r => r.IsError()) + Results.Where(r => r.IsErrorGroup()).Sum(r => r.GroupErrors);
        }
    }
}
