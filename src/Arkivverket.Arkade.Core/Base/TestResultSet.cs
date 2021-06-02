using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Testing;

namespace Arkivverket.Arkade.Core.Base
{
    public class TestResultSet
    {
        public string Name { get; set; }
        public List<TestResultSet> TestResultSets = new();
        public List<TestResult> TestsResults = new();

        public int GetNumberOfResults()
        {
            return TestResultSets.Sum(testResultSet => testResultSet.GetNumberOfResults()) + TestsResults.Count;
        }

        public TestResultSet FindFirst(string name)
        {
            return name == Name
                ? this
                : TestResultSets.Select(testResultSet => testResultSet.FindFirst(name)).FirstOrDefault();
        }

        public bool IsSuccess()
        {
            foreach (TestResultSet testResultSet in TestResultSets)
                return testResultSet.IsSuccess();

            return TestsResults.TrueForAll(r => !r.IsError());
        }

        public int FindNumberOfErrors()
        {
            return TestsResults.Count(r => r.IsError()) +
                   TestsResults.Where(r => r.IsErrorGroup()).Sum(r => r.GroupErrors) +
                   TestResultSets.Sum(testResultSet => testResultSet.FindNumberOfErrors());
        }

        public List<TestResult> GetErrorResults()
        {
            List<TestResult> errorResults = TestsResults.Where(r => r.IsError()).ToList();

            foreach (TestResultSet testResultSet in TestResultSets)
                errorResults.AddRange(testResultSet.GetErrorResults());

            return errorResults;
        }

        public List<TestResult> GetAllResults()
        {
            var results = new List<TestResult>();

            results.AddRange(TestsResults);

            foreach (TestResultSet testResultSet in TestResultSets)
                results.AddRange(testResultSet.GetAllResults());

            return results;
        }
    }
}
