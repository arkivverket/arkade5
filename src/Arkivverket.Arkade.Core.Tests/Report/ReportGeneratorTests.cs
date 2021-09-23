using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Report;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Tests.Base;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Report
{
    public class ReportGeneratorTests
    {
        private readonly string _workingDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\TestData\\Report\\FilesToBeListed";

        private TestSession CreateTestSessionWithTwoTestRuns()
        {
            TestRun testRun1 = new TestRunBuilder()
                .WithTestId(new TestId(TestId.TestKind.Unidentified, 1))
                .WithTestName("Test 1")
                .WithTestDescription("Test description 1")
                .WithTestResult(new TestResult(ResultType.Error, new Location("location"), "Test result 1"))
                .WithTestResult(new TestResult(ResultType.Error, new Location("location"), "Test result 2"))
                .WithDurationMillis(100L)
                .Build();

            TestRun testRun2 = new TestRunBuilder()
                .WithTestId(new TestId(TestId.TestKind.Unidentified, 2))
                .WithTestName("Test 2")
                .WithTestDescription("Test description 2")
                .WithTestResult(new TestResult(ResultType.Error, new Location("location"), "Test result 1"))
                .WithTestResult(new TestResult(ResultType.Error, new Location("location"), "Test result 2"))
                .WithTestResult(new TestResult(ResultType.Error, new Location("location"), "Test result 3"))
                .WithDurationMillis(100L)
                .Build();

            var testRuns = new List<TestRun> { testRun1, testRun2 };

            Archive archive = new ArchiveBuilder()
                .WithWorkingDirectoryRoot(_workingDirectory)
                .WithArchiveDetails("5.0")
                .WithArchiveType(ArchiveType.Noark5)
                .WithUuid(Uuid.Random())
                .Build();
            
            TestSession testSession = new TestSessionBuilder()
                .WithArchive(archive)
                .WithLogEntry("log entry")
                .WithTestRuns(testRuns)
                .WithTestSummary(new TestSummary(0, 0, 0, 0, 0))
                .Build();

            return testSession;
        }

        private static string GenerateReport(TestSession testSession, TestReportFormat reportType)
        {
            var memoryStream = new MemoryStream();
            TestReport testReport = TestReportFactory.Create(testSession);
            IReportGenerator reportGenerator = SelectReportGenerator(reportType);
            reportGenerator.Generate(testReport, memoryStream);
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        private static IReportGenerator SelectReportGenerator(TestReportFormat reportType)
        {
            return reportType switch
            {
                TestReportFormat.html => new HtmlReportGenerator(),
                TestReportFormat.xml => new XmlReportGenerator(),
                TestReportFormat.json => new JsonReportGenerator(),
                TestReportFormat.pdf => new PdfReportGenerator(),
                _ => null
            };
        }

        [Fact]
        public void ShouldGenerateHtmlStringWithExpectedInformation()
        {
            TestSession testSession = CreateTestSessionWithTwoTestRuns();

            string html = GenerateReport(testSession, TestReportFormat.html);

            html.Contains(Resources.Report.LabelArchiveCreators).Should().BeTrue();
            html.Contains(Resources.Report.LabelArchivePeriod).Should().BeTrue();
            html.Contains(Resources.Report.LabelSystemName).Should().BeTrue();
            html.Contains(Resources.Report.LabelSystemType).Should().BeTrue();
            html.Contains(Resources.Report.LabelArchiveType).Should().BeTrue();
            html.Contains(Resources.Report.LabelDateOfTesting).Should().BeTrue();
            html.Contains(Resources.Report.LabelNumberOfTestsExecuted).Should().BeTrue();
            html.Contains(Resources.Report.LabelNumberOfFilesProcessed).Should().BeTrue();
            html.Contains(Resources.Report.LabelNumberOfErrors).Should().BeTrue();
            html.Contains("id=\"U.01\"").Should().BeTrue();
            html.Contains(Resources.Report.TestTypeContentAnalysisDisplayName).Should().BeTrue();
            html.Contains("Test description 1").Should().BeTrue();
            html.Contains("location").Should().BeTrue();
        }

        [Fact]
        public void ShouldGenerateXmlStringWithExpectedInformation()
        {
            TestSession testSession = CreateTestSessionWithTwoTestRuns();

            string xml = GenerateReport(testSession, TestReportFormat.xml);

            xml.Contains("<Summary>").Should().BeTrue();
            xml.Contains("<ArchiveCreators>").Should().BeTrue();
            xml.Contains("<ArchivalPeriod>").Should().BeTrue();
            xml.Contains("<SystemName>").Should().BeTrue();
            xml.Contains("<SystemType>").Should().BeTrue();
            xml.Contains("<ArchiveType>").Should().BeTrue();
            xml.Contains("<DateOfTesting>").Should().BeTrue();
            xml.Contains("<NumberOfTestsRun>").Should().BeTrue();
            xml.Contains("<NumberOfProcessedFiles>").Should().BeTrue();
            xml.Contains("<NumberOfProcessedRecords>").Should().BeTrue();
            xml.Contains("<NumberOfErrors>").Should().BeTrue();
            xml.Contains("<TestsResults>").Should().BeTrue();
            xml.Contains("<ExecutedTest>").Should().BeTrue();
            xml.Contains("<TestId>U.01").Should().BeTrue();
            xml.Contains("<TestType>ContentAnalysis").Should().BeTrue();
            xml.Contains("<TestDescription>Test description 1").Should().BeTrue();
            xml.Contains("<ResultSet>").Should().BeTrue();
            xml.Contains("<Results>").Should().BeTrue();
            xml.Contains("<Result>").Should().BeTrue();
            xml.Contains("<ResultType>Error").Should().BeTrue();
            xml.Contains("<Location>location").Should().BeTrue();
            xml.Contains("<Message>Test result 1").Should().BeTrue();
        }

        [Fact]
        public void ShouldGenerateJsonStringWithExpectedInformation()
        {
            TestSession testSession = CreateTestSessionWithTwoTestRuns();

            string json = GenerateReport(testSession, TestReportFormat.json);

            json.Contains("\"Summary\"").Should().BeTrue();
            json.Contains("\"ArchiveCreators\"").Should().BeTrue();
            json.Contains("\"ArchivalPeriod\"").Should().BeTrue();
            json.Contains("\"SystemName\"").Should().BeTrue();
            json.Contains("\"SystemType\"").Should().BeTrue();
            json.Contains("\"ArchiveType\"").Should().BeTrue();
            json.Contains("\"DateOfTesting\"").Should().BeTrue();
            json.Contains("\"NumberOfTestsRun\"").Should().BeTrue(); 
            json.Contains("\"NumberOfProcessedFiles\"").Should().BeTrue();
            json.Contains("\"NumberOfProcessedRecords\"").Should().BeTrue();
            json.Contains("\"NumberOfErrors\"").Should().BeTrue();
            json.Contains("\"U.01\"").Should().BeTrue();
            json.Contains("\"ContentAnalysis\"").Should().BeTrue();
            json.Contains("\"Test description 1\"").Should().BeTrue();
            json.Contains("\"Error\"").Should().BeTrue();
            json.Contains("\"Test result 1\"").Should().BeTrue();
            json.Contains("\"Test result 2\"").Should().BeTrue();
            json.Contains("\"U.02\"").Should().BeTrue();
            json.Contains("\"Test result 3\"").Should().BeTrue();
        }

        [Fact]
        public void ShouldGeneratePdfReport()
        {
            TestSession testSession = CreateTestSessionWithTwoTestRuns();

            string pdf = GenerateReport(testSession, TestReportFormat.pdf);

            pdf.Contains("PDF").Should().BeTrue();
        }
    }
}