using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Report;
using Arkivverket.Arkade.Test.Core;
using Arkivverket.Arkade.Tests;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Report
{
    public class HtmlReportGeneratorTest
    {
        [Fact]
        public void ShouldGenerateReport()
        {
            TestRun testRun1 = new TestRunBuilder()
                .WithDurationMillis(100L)
                .WithTestName("Test 1")
                .WithTestDescription("Test description 1")
                .WithTestCategory("Testkategori 1")
                .WithTestResult(new TestResult(ResultType.Error, new Location("location"), "Test result 1"))
                .WithTestResult(new TestResult(ResultType.Error, new Location("location"), "Test result 2"))
                .Build();

            TestRun testRun2 = new TestRunBuilder()
                .WithDurationMillis(100L)
                .WithTestName("Test 2")
                .WithTestDescription("Test description 2")
                .WithTestCategory("Testkategori 2")
                .WithTestResult(new TestResult(ResultType.Error, new Location("location"), "Test result 1"))
                .WithTestResult(new TestResult(ResultType.Error, new Location("location"), "Test result 2"))
                .WithTestResult(new TestResult(ResultType.Error, new Location("location"), "Test result 3"))
                .Build();

            List<TestRun> testRuns = new List<TestRun> {testRun1, testRun2};

            TestSession testSession = new TestSessionBuilder()
                .WithTestRuns(testRuns)
                .WithTestSummary(new TestSummary(0,0,0))
                .Build();

            HtmlReportGenerator htmlReportGenerator = new HtmlReportGenerator();
            HtmlReport htmlReport = htmlReportGenerator.Generate(testSession);

            //htmlReport.Save(new FileInfo("c://tmp/report.html"));

            string html = htmlReport.GetHtml();
            html.Should().Contain("<html");
            html.Should().Contain("Test 1");
            html.Should().Contain("Test 2");
            html.Should().Contain("</html>");
        }

        [Fact]
        public void ShouldGenerateReportWithSummaryForNoark5()
        {
            TestRun testRun1 = new TestRunBuilder()
                .WithDurationMillis(100L)
                .WithTestName("Test 1")
                .WithTestDescription("Test description 1")
                .WithTestCategory("Testkategori 1")
                .WithTestResult(new TestResult(ResultType.Error, new Location("location"), "Test result 1"))
                .WithTestResult(new TestResult(ResultType.Error, new Location("location"), "Test result 2"))
                .Build();

            List<TestRun> testRuns = new List<TestRun> { testRun1 };

            TestSession testSession = new TestSessionBuilder()
                .WithArchive(new Archive(ArchiveType.Noark5, null, null))
                .WithTestSummary(new TestSummary(0, 0, 44))
                .WithTestRuns(testRuns)
                .Build();

            HtmlReportGenerator htmlReportGenerator = new HtmlReportGenerator();
            HtmlReport htmlReport = htmlReportGenerator.Generate(testSession);

            htmlReport.Save(new System.IO.FileInfo("c://tmp/report.html"));

            string html = htmlReport.GetHtml();
            // xunit was not very happy to report errors on a very huge string
            html.Contains("Antall tester").Should().BeTrue();
            html.Contains("Antall filer").Should().BeFalse();
            html.Contains("Antall poster").Should().BeFalse();
        }

        [Fact]
        public void ShouldGenerateReportWithSummaryForAddmlFlatFile()
        {
            TestRun testRun1 = new TestRunBuilder()
                .WithDurationMillis(100L)
                .WithTestName("Test 1")
                .WithTestDescription("Test description 1")
                .WithTestCategory("Testkategori 1")
                .WithTestResult(new TestResult(ResultType.Error, new Location("location"), "Test result 1"))
                .WithTestResult(new TestResult(ResultType.Error, new Location("location"), "Test result 2"))
                .Build();

            List<TestRun> testRuns = new List<TestRun> { testRun1 };

            TestSession testSession = new TestSessionBuilder()
                .WithArchive(new Archive(ArchiveType.Noark3, null, null))
                .WithTestSummary(new TestSummary(41, 42, 0))
                .WithTestRuns(testRuns)
                .Build();

            testSession.TestSummary = new TestSummary(42, 43, 44);

            HtmlReportGenerator htmlReportGenerator = new HtmlReportGenerator();
            HtmlReport htmlReport = htmlReportGenerator.Generate(testSession);

            htmlReport.Save(new System.IO.FileInfo("c://tmp/report.html"));

            string html = htmlReport.GetHtml();
            // xunit was not very happy to report errors on a very huge string
            html.Contains("Antall filer").Should().BeTrue();
            html.Contains("Antall poster").Should().BeTrue();
            html.Contains("Antall tester kjørt").Should().BeFalse();
        }
    }
}