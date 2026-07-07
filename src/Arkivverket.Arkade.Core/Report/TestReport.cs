using System.Collections.Generic;
using System.Text.Json.Serialization;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
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
        // Omitted from the report entirely when there is no relevant DIAS package (directory / .siard
        // input). JSON omits it via the attribute below; XmlSerializer omits a null string element;
        // the HTML/PDF generator skips the row. All four formats stay content-identical.
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string InformationPackageUuid { get; set; }

        // A DIAS package whose METS lacks a valid identity (UUID) still IS a package: the report's
        // package row is then rendered with an explicit unknown-identity marker, never dropped —
        // dropping it would disguise the package as a loose archive-extraction input. Omitted from
        // serialized reports when false, keeping identified-package and no-package reports unchanged.
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool PackageIdentityIsUnknown { get; set; }
        public bool ShouldSerializePackageIdentityIsUnknown() => PackageIdentityIsUnknown;
        public ArchiveType ArchiveType { get; set; }
        public string ArchiveCreators { get; set; }
        public string ArchivalPeriod { get; set; }
        public string SystemName { get; set; }
        public string SystemType { get; set; }
        public string TimeOfTesting { get; set; }
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
        public Location Location { get; set; }
        public string Message { get; set; }
    }

    public class Location
    {
        public string String { get; set; }
        public string FileName { get; set; }
        public List<long> LineNumbers { get; set; }
    }

}
