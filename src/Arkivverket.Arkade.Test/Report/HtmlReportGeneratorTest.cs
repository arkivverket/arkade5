using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Report;
using Arkivverket.Arkade.Test.Core;
using Arkivverket.Arkade.Tests;
using FluentAssertions;
using Xunit;
using System.IO;
using System.Text;

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

            List<TestRun> testRuns = new List<TestRun> { testRun1, testRun2 };

            TestSession testSession = new TestSessionBuilder()
                .WithTestRuns(testRuns)
                .WithTestSummary(new TestSummary(0, 0, 0))
                .Build();

            string html = GenerateReport(testSession);

            html.Should().Contain("<html");
            html.Should().Contain("Test 1");
            html.Should().Contain("Test 2");
            html.Should().Contain("</html>");
        }

        private static string GenerateReport(TestSession testSession)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            HtmlReportGenerator htmlReportGenerator = new HtmlReportGenerator(sw);
            htmlReportGenerator.Generate(testSession);
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        [Fact(Skip = "Feiler!")]
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

            string html = GenerateReport(testSession);

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

            string html = GenerateReport(testSession);

            // xunit was not very happy to report errors on a very huge string
            html.Contains("Antall filer").Should().BeTrue();
            html.Contains("Antall poster").Should().BeTrue();
            html.Contains("Antall tester kjørt").Should().BeFalse();
        }
    }
}