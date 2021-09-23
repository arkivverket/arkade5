using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;

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

        public static TestReport CreateForSiard(TestSession testSession)
        {
            var testReport = new TestReport
            {
                Summary = CreateTestReportSummary(testSession),
                TestsResults = GetSiardTestReportResults(),
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
                NumberOfProcessedFiles = testSession.TestSummary.NumberOfProcessedFiles,
                NumberOfProcessedRecords = testSession.TestSummary.NumberOfProcessedRecords,
                NumberOfWarnings = testSession.TestSummary.NumberOfWarnings,
                NumberOfErrors = testSession.Archive.ArchiveType is ArchiveType.Siard
                    ? testSession.TestSummary.NumberOfErrors
                    : testSession.TestSuite.FindNumberOfErrors().ToString(),
        };

            return summary;
        }

        private static List<ExecutedTest> GetTestReportResults(TestSession testSession)
        {
            return testSession.TestSuite.TestRuns.Select(testRun => new ExecutedTest
                {
                    TestId = testRun.TestId.ToString(),
                    TestName = testRun.TestName,
                    TestType = testRun.TestType,
                    TestDescription = testRun.TestDescription,
                    ResultSet = GetResultSet(testRun.TestResults),
                    HasResults = testRun.TestResults.GetNumberOfResults() > 0,
                    NumberOfErrors = testRun.TestResults.GetErrorResults().Count.ToString(),
                })
                .ToList();
        }

        private static ResultSet GetResultSet(TestResultSet testResultSet)
        {
            return new ResultSet
            {
                Name = testResultSet.Name,
                Results = GetResults(testResultSet.TestsResults),
                ResultSets = GetResultSets(testResultSet.TestResultSets),
            };
        }

        private static List<ResultSet> GetResultSets(List<TestResultSet> testResultSets)
        {
            return testResultSets.Select(testResultSet => new ResultSet
            {
                Name = testResultSet.Name,
                Results = GetResults(testResultSet.TestsResults),
                ResultSets = GetResultSets(testResultSet.TestResultSets),
            }).ToList();
        }

        private static List<Result> GetResults(IEnumerable<TestResult> testResults)
        {
            return testResults.Select(testResult => new Result
            {
                ResultType = testResult.Result,
                Location = testResult.Location.ToString(),
                Message = testResult.Message,
            }).ToList();
        }

        private static List<ExecutedTest> GetSiardTestReportResults()
        {
            return new()
            {
                new ExecutedTest
                {
                    TestId = "externalReport",
                    TestName = string.Format(Resources.SiardMessages.ValidationResultTestName, Resources.SiardMessages.DbptkDeveloper),
                    ResultSet = GetSiardResultSet(),
                    HasResults = true,
                    TestType = null,
                }
            };
        }

        private static ResultSet GetSiardResultSet()
        {
            return new()
            {
                Results = GetSiardResult(),
                ResultSets = new List<ResultSet>(),
            };
        }

        private static List<Result> GetSiardResult()
        {
            return new()
            {
                new Result
                {
                    Location = Resources.OutputFileNames.SiardValidationReportFile,
                    Message = Resources.SiardMessages.ValidationResultMessage,
                }
            };
        }
    }
}
