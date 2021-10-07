using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Report
{
    public static class TestReportGeneratorRunner
    {
        public static void RunAllGenerators(TestSession testSession, DirectoryInfo testReportDirectory, bool standalone)
        {
            TestReport testReport = testSession.Archive.ArchiveType.Equals(ArchiveType.Siard)
                ? TestReportFactory.CreateForSiard(testSession, standalone)
                : TestReportFactory.Create(testSession);

            foreach (TestReportFormat testReportFormat in Enum.GetValues<TestReportFormat>())
            {
                string testReportFileName = Path.Combine(testReportDirectory.FullName, standalone
                    ? string.Format(Resources.OutputFileNames.StandaloneTestReportFile, testReport.Summary.Uuid, testReportFormat.ToString())
                    : string.Format(Resources.OutputFileNames.TestReportFile, testReportFormat.ToString()));

                using FileStream fileStream = new FileInfo(testReportFileName).OpenWrite();
                
                IReportGenerator reportGenerator = GetReportGenerator(testReportFormat);
                reportGenerator.Generate(testReport, fileStream);
            }
        }

        private static IReportGenerator GetReportGenerator(TestReportFormat testReportFormat)
        {
            return testReportFormat switch
            {
                TestReportFormat.html => new HtmlReportGenerator(),
                TestReportFormat.xml => new XmlReportGenerator(),
                TestReportFormat.json => new JsonReportGenerator(),
                TestReportFormat.pdf => new PdfReportGenerator(),
                _ => null
            };
        }
    }
    public enum TestReportFormat
    {
        html,
        xml,
        json,
        pdf,
    }
}
