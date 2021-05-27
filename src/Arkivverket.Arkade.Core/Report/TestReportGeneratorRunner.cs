using System;
using System.ComponentModel;
using System.IO;
using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Report
{
    public static class TestReportGeneratorRunner
    {
        public static void RunAllGenerators(TestSession testSession, DirectoryInfo testReportDirectory)
        {
            TestReport testReport = TestReportFactory.Create(testSession);
            string testReportFileName = Path.Combine(testReportDirectory.FullName, 
                string.Format(Resources.OutputFileNames.TestReportFile, testReport.Summary.Uuid, "{0}"));

            foreach (TestReportFormat testReportFormat in Enum.GetValues<TestReportFormat>())
            {
                var reportFile = new FileInfo(string.Format(testReportFileName, testReportFormat.ToString()));
                IReportGenerator reportGenerator = GetReportGenerator(testReportFormat);
                using FileStream fileStream = reportFile.OpenWrite();
                reportGenerator.Generate(testReport, fileStream);
                if (testReportFormat is TestReportFormat.json or TestReportFormat.xml)
                    GenerateSchema(typeof(TestReport), testReportFormat, testReportDirectory.FullName);
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

        private static void GenerateSchema(Type reportType, TestReportFormat testReportFormat, string testReportDirectory)
        {
            string testReportSchemaDirectory = Path.Combine(testReportDirectory, "schemas");
            if (!Directory.Exists(testReportSchemaDirectory))
                Directory.CreateDirectory(testReportSchemaDirectory);

            using FileStream fileStream = File.OpenWrite(GetSchemaName(testReportFormat, testReportSchemaDirectory));
            using var schemaWriter = new StreamWriter(fileStream);

            switch (testReportFormat)
            {
                case TestReportFormat.json:
                    new JsonReportSchemaGenerator().Generate(reportType, schemaWriter);
                    break;
                case TestReportFormat.xml:
                    new XmlReportSchemaGenerator().Generate(reportType, schemaWriter);
                    break;
                default:
                    throw new InvalidEnumArgumentException(string.Format(
                        Resources.ExceptionMessages.SchemaGeneratorIsNotImplementedMessage, testReportFormat));
            }

            schemaWriter.Flush();
        }

        private static string GetSchemaName(TestReportFormat testReportFormat, string testReportSchemaDirectory)
        {
            return testReportFormat switch
            {
                TestReportFormat.json => Path.Combine(testReportSchemaDirectory, "testReport.json"),
                TestReportFormat.xml => Path.Combine(testReportSchemaDirectory, "testReport.xsd"),
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
