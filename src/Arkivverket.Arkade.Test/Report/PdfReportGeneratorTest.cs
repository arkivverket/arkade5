using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Report;
using Arkivverket.Arkade.Test.Core;
using Arkivverket.Arkade.Tests;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Report
{
    public class PdfReportGeneratorTest
    {
        [Fact]
        public void ShouldGeneratePdfReport()
        {
            TestRun testRun1 = new TestRunBuilder()
                .WithDurationMillis(100L)
                .WithTestName("Test 1")
                .WithTestDescription("Test description 1")
                .WithTestCategory("Testkategori 1")
                .WithTestResult(new TestResult(ResultType.Error, "Test result 1"))
                .WithTestResult(new TestResult(ResultType.Error, "Test result 2"))
                .Build();

            TestRun testRun2 = new TestRunBuilder()
                .WithDurationMillis(100L)
                .WithTestName("Test 2")
                .WithTestDescription("Test description 2")
                .WithTestCategory("Testkategori 2")
                .WithTestResult(new TestResult(ResultType.Error, "Test result 1"))
                .WithTestResult(new TestResult(ResultType.Error, "Test result 2"))
                .WithTestResult(new TestResult(ResultType.Error, "Test result 3"))
                .Build();

            List<TestRun> testRuns = new List<TestRun> {testRun1, testRun2};

            TestSession testSession = new TestSessionBuilder()
                .WithTestRuns(testRuns)
                .Build();


            PdfReportGenerator reportGenerator = new PdfReportGenerator();
            PdfReport pdfReport = reportGenerator.Generate(testSession);

            //pdfReport.Save(new FileInfo("c://tmp/test.pdf"));
            pdfReport.ToBytes().Should().NotBeNullOrEmpty();
        }
    }
}