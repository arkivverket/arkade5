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
                .Build();


            HtmlReportGenerator htmlReportGenerator = new HtmlReportGenerator();
            HtmlReport htmlReport = htmlReportGenerator.Generate(testSession);

            //htmlReport.Save(new DirectoryInfo("c://tmp/report/"));

            string html = htmlReport.GetHtml();
            html.Should().Contain("<html");
            html.Should().Contain("Test 1");
            html.Should().Contain("Test 2");
            html.Should().Contain("</html>");
        }
    }
}