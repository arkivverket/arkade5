using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Report
{
    public static class TestReportGenerator
    {
        public static void Generate(TestSession testSession, DirectoryInfo testReportDirectory)
        {
            TestReport testReport = TestReportFactory.Create(testSession);
            string testReportFileName = Path.Combine(testReportDirectory.FullName, 
                string.Format(Resources.OutputFileNames.TestReportFile, testReport.Summary.Uuid, "{0}"));

            foreach (TestReportFormat testReportFormat in Enum.GetValues<TestReportFormat>())
            {
                var reportFile = new FileInfo(string.Format(testReportFileName, testReportFormat.ToString()));
                IReportGenerator reportGenerator = GetReportGenerator(testReportFormat);
                using FileStream fileStream = reportFile.OpenWrite();
                using var streamWriter = new StreamWriter(fileStream);
                reportGenerator.Generate(testReport, streamWriter);
            }
        }

        private static IReportGenerator GetReportGenerator(TestReportFormat testReportFormat)
        {
            return testReportFormat switch
            {
                TestReportFormat.html => new HtmlReportGenerator(),
                TestReportFormat.json => new JsonReportGenerator(),
                _ => null
            };
        }

        private enum TestReportFormat
        {
            html,
            json,
        }
    }
}
