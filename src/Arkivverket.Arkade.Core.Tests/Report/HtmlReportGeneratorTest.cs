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
    public class HtmlReportGeneratorTest : LanguageDependentTest
    {
        private const int TestResultDisplayLimit = 100;

        private static Archive CreateNoark3Archive()
        {
            return new ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark3)
                .WithWorkingDirectoryExternalContent(Path.Combine("TestData", "noark3"))
                .Build();
        }

        private static Archive CreateNoark5Archive()
        {
            return new ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryRoot(Path.Combine("TestData", "Report", "FilesToBeListed"))
                .Build();
        }

        private static List<TestRun> CreateTwoTestRuns()
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

            return new List<TestRun> {testRun1, testRun2};
        }

        private static string GenerateReport(Archive archive)
        {
            var ms = new MemoryStream();
            TestReport testReport = TestReportFactory.Create(archive, packageId: null);
            new HtmlReportGenerator(TestResultDisplayLimit).Generate(testReport, ms);
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        [Fact]
        public void ShouldGenerateReport()
        {
            Archive archive = CreateNoark3Archive();

            new TestSessionBuilder()
                .WithArchive(archive)
                .WithTestRuns(CreateTwoTestRuns())
                .WithTestSummary(new TestSummary(0, 0, 0, 0, 0))
                .Build();

            string html = GenerateReport(archive);

            html.Should().Contain("<html");
            // Test names render via ArkadeTestNameProvider (TestId-based display name), not the mock's name
            html.Should().Contain("U.01");
            html.Should().Contain("U.02");
            html.Should().Contain("Test description 1");
            html.Should().Contain("Test description 2");
            html.Should().Contain("</html>");
        }

        [Fact]
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

            Archive archive = CreateNoark3Archive();

            new TestSessionBuilder()
                .WithArchive(archive)
                .WithTestSummary(new TestSummary(41, 42, 0, 0, 0))
                .WithTestRuns(testRuns)
                .Build();

            archive.TestSession.TestSummary = new TestSummary(42, 43, 44, 0, 0);

            string html = GenerateReport(archive);

            // xunit was not very happy to report errors on a very huge string
            html.Contains("Antall filer").Should().BeTrue();
            html.Contains("Antall poster").Should().BeTrue();
        }

        [Fact]
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

            Archive archive = CreateNoark5Archive();

            new TestSessionBuilder()
                .WithArchive(archive)
                .WithTestSummary(new TestSummary(0, 0, 44, 0, 0))
                .WithTestRuns(testRuns)
                .Build();

            string html = GenerateReport(archive);

            // xunit was not very happy to report errors on a very huge string
            // Noark5 reports show tests-executed instead of processed files/records
            html.Contains("Antall tester utført").Should().BeTrue();
            html.Contains("Antall filer").Should().BeFalse();
            html.Contains("Antall poster").Should().BeFalse();
        }

        [Fact(Skip = "Test body commented out long before ARKADE-782 (version text troubled the build server)")]
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