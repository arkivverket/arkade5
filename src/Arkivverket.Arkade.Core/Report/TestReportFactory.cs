using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Report
{
    public static class TestReportFactory
    {
        public static TestReport Create(TestSession testSession)
        {
            var testReport = new TestReport
            {
                Summary = CreateTestReportSummary(testSession),
                TestsResults = GetTestReportResults(testSession),
            };

            return testReport;
        }

        private static TestReportSummary CreateTestReportSummary(TestSession testSession)
        {
            var norwegianCulture = new CultureInfo("nb-NO");
            int numberOfExecutedTests = testSession.TestSuite.TestRuns.Count();
            int numberOfAvailableTests = testSession.AvailableTests.Count;

            var summary = new TestReportSummary
            {
                Uuid = testSession.Archive.Uuid.ToString(),
                ArchiveCreators = testSession.Archive.Details.ArchiveCreators,
                ArchivalPeriod = testSession.Archive.Details.ArchivalPeriod,
                SystemName = testSession.Archive.Details.SystemName,
                SystemType = testSession.Archive.Details.SystemType,
                ArchiveType = testSession.Archive.ArchiveType,
                DateOfTesting = testSession.DateOfTesting.ToString(Resources.Report.DateFormat, norwegianCulture),
                NumberOfTestsRun = string.Format(Resources.Report.ValueNumberOfTestsExecuted, numberOfExecutedTests, numberOfAvailableTests),
                NumberOfErrors = testSession.TestSuite.FindNumberOfErrors().ToString(),
            };

            if (testSession.TestSummary != null)
            {
                summary.NumberOfProcessedFiles = testSession.TestSummary.NumberOfProcessedFiles;
                summary.NumberOfProcessedRecords = testSession.TestSummary.NumberOfProcessedRecords;
            }

            return summary;
        }

        private static List<ExecutedTest> GetTestReportResults(TestSession testSession)
        {
            var results = new List<ExecutedTest>();

            foreach (TestRun testRun in testSession.TestSuite.TestRuns)
            {
                var test = new ExecutedTest
                {
                    TestId = testRun.TestId.ToString(),
                    TestName = testRun.TestName,
                    TestType = testRun.TestType,
                    TestDescription = testRun.TestDescription,
                    TestResults = new List<Result>(),
                };

                foreach (Result result in testRun.Results.Select(testRunResult =>
                    new Result
                    {
                        ResultType = testRunResult.Result,
                        Location = testRunResult.Location.ToString(),
                        Message = testRunResult.Message,
                    }))
                {
                    test.TestResults.Add(result);
                }
                results.Add(test);
            }

            return results;
        }
    }
}
