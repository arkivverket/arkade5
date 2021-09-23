using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;

namespace Arkivverket.Arkade.Core.Report
{
    public class TestReport
    {
        public TestReportSummary Summary { get; init; }
        public List<ExecutedTest> TestsResults { get; init; }
    }

    public class TestReportSummary
    {
        public string Uuid { get; set; }
        public ArchiveType ArchiveType { get; set; }
        public string ArchiveCreators { get; set; }
        public string ArchivalPeriod { get; set; }
        public string SystemName { get; set; }
        public string SystemType { get; set; }
        public string DateOfTesting { get; set; }
        public string NumberOfProcessedFiles { get; set; }
        public string NumberOfProcessedRecords { get; set; }
        public string NumberOfTestsRun { get; set; }
        public string NumberOfErrors { get; set; }
        public string NumberOfWarnings { get; set; }
    }

    public class ExecutedTest
    {
        public string TestId { get; set; }
        public string TestName { get; set; }
        public TestType? TestType { get; set; }
        public string TestDescription { get; set; }
        public ResultSet ResultSet { get; set; }
        public bool HasResults { get; set; }
        public string NumberOfErrors { get; set; }
    }

    public class ResultSet
    {
        public string Name { get; set; }
        public List<ResultSet> ResultSets { get; set; }
        public List<Result> Results { get; set; }
    }

    public class Result
    {
        public ResultType? ResultType { get; set; }
        public string Location { get; set; }
        public string Message { get; set; }
    }


}
