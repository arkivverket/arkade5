using System.Collections.Generic;
using System.IO;
using System.Text;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Report;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.Core.Tests.Base;
using FluentAssertions;
using Xunit;
using Location = Arkivverket.Arkade.Core.Testing.Location;

namespace Arkivverket.Arkade.Core.Tests.Report
{
    public class HtmlReportGeneratorTest
    {
        private const int TestResultDisplayLimit = 100;

        private static TestSession CreateTestSessionWithTwoTestRuns()
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

            var testRuns = new List<TestRun> {testRun1, testRun2};

            TestSession testSession = new TestSessionBuilder()
                .WithTestRuns(testRuns)
                .WithTestSummary(new TestSummary(0, 0, 0, 0, 0))
                .Build();
            return testSession;
        }

        private static string GenerateReport(TestSession testSession)
        {
            var ms = new MemoryStream();
            TestReport testReport = TestReportFactory.Create(testSession);
            new HtmlReportGenerator(TestResultDisplayLimit).Generate(testReport, ms);
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        [Fact(Skip = "Archive is expected to have a content directory")]
        public void ShouldGenerateReport()
        {
            TestSession testSession = CreateTestSessionWithTwoTestRuns();

            string html = GenerateReport(testSession);

            html.Should().Contain("<html");
            html.Should().Contain("Test 1");
            html.Should().Contain("Test 2");
            html.Should().Contain("</html>");
        }

        [Fact(Skip = "Archive is expected to have a content directory")]
        public void ShouldGenerateReportWithSummaryForAddmlFlatFile()
        {
            TestRun testRun1 = new TestRunBuilder()
                .WithDurationMillis(100L)
                .WithTestId(new TestId(TestId.TestKind.Addml, 0))
                .WithTestName("Test 1")
                .WithTestDescription("Test description 1")
                .WithTestResult(new TestResult(ResultType.Error, new Location("location"), "Test result 1"))
                .WithTestResult(new TestResult(ResultType.Error, new Location("location"), "Test result 2"))
                .Build();

            var testRuns = new List<TestRun> {testRun1};

            TestSession testSession = new TestSessionBuilder()
                .WithArchive(new Archive(ArchiveType.Noark3, null, null, null))
                .WithTestSummary(new TestSummary(41, 42, 0, 0, 0))
                .WithTestRuns(testRuns)
                .Build();

            testSession.TestSummary = new TestSummary(42, 43, 44, 0, 0);

            string html = GenerateReport(testSession);

            // xunit was not very happy to report errors on a very huge string
            html.Contains("Antall filer").Should().BeTrue();
            html.Contains("Antall poster").Should().BeTrue();
        }

        [Fact(Skip = "Archive is expected to have a content directory")]
        public void ShouldGenerateReportWithSummaryForNoark5()
        {
            TestRun testRun1 = new TestRunBuilder()
                .WithDurationMillis(100L)
                .WithTestId(new TestId(TestId.TestKind.Noark5, 0))
                .WithTestName("Test 1")
                .WithTestDescription("Test description 1")
                .WithTestResult(new TestResult(ResultType.Error, new Location("location"), "Test result 1"))
                .WithTestResult(new TestResult(ResultType.Error, new Location("location"), "Test result 2"))
                .Build();

            var testRuns = new List<TestRun> {testRun1};

            TestSession testSession = new TestSessionBuilder()
                .WithArchive(new Archive(ArchiveType.Noark5, null, null, null))
                .WithTestSummary(new TestSummary(0, 0, 44, 0, 0))
                .WithTestRuns(testRuns)
                .Build();

            string html = GenerateReport(testSession);

            // xunit was not very happy to report errors on a very huge string
            html.Contains("Antall filer").Should().BeTrue();
            html.Contains("Antall poster").Should().BeFalse();
        }

        [Fact(Skip = "Archive is expected to have a content directory")]
        public void ShouldShowArkadeVersionNumberInReport()
        {
            /*
            TestSession testSession = CreateTestSessionWithTwoTestRuns();

            string html = GenerateReport(testSession);
            string versionText = Resources.Report.FooterArkadeVersion;
            // remove version number from text - causes trouble on build server.
            versionText = versionText.Substring(0, versionText.IndexOf("{0}", StringComparison.Ordinal));
            html.Contains(versionText).Should().BeTrue();
            */
        }
    }
}