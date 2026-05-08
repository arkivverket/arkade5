using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
using static Arkivverket.Arkade.Core.Resources.OutputFileNames;

namespace Arkivverket.Arkade.Core.Report
{
    public static class TestReportGeneratorRunner
    {
        public static void RunAllGenerators(Archive archive, DirectoryInfo outputDirectory,
            int testResultDisplayLimit, DiasPackage diasPackage, out DirectoryInfo reportsDirectory)
        {
            TestReport testReport = archive is SiardArchive siardArchive
                ? TestReportFactory.CreateForSiard(siardArchive, diasPackage?.Id)
                : TestReportFactory.Create(archive, diasPackage?.Id);

            string extensionReadyTestReportFullName = GetExtensionReadyTestReportFullName(outputDirectory,
                diasPackage, out string reportsDirectoryPath);

            reportsDirectory = Directory.CreateDirectory(reportsDirectoryPath);

            foreach (TestReportFormat testReportFormat in Enum.GetValues<TestReportFormat>())
            {
                string testReportFullName = string.Format(extensionReadyTestReportFullName, testReportFormat);
                using FileStream fileStream = new FileInfo(testReportFullName).OpenWrite();
                IReportGenerator reportGenerator = GetReportGenerator(testReportFormat, testResultDisplayLimit);
                reportGenerator.Generate(testReport, fileStream);
            }
        }

        public static string GetExtensionReadyTestReportFullName(DirectoryInfo outputDirectory, DiasPackage diasPackage, out string reportDirectoryPath)
        {
            switch (diasPackage)
            {
                case OutputDiasPackage { PackageType: PackageType.ArchivalInformationPackage } outputAip:
                {
                    reportDirectoryPath = outputAip.WorkingDirectory.RepositoryOperations()
                        .WithSubDirectory(TestReportDirectory).ToString();

                    string extensionReadyReportFileName = TestReportFile;
                    return Path.Combine(reportDirectoryPath, extensionReadyReportFileName);
                }
                case OutputDiasPackage { PackageType: PackageType.SubmissionInformationPackage } outputSip:
                {
                    string standAloneDirectoryName = string.Format(StandaloneTestReportDirectory, outputSip.Id); // NB! UUID-writeout (test results)
                    string resultOutputDirectoryName = string.Format(ResultOutputDirectory, outputSip.Id); // NB! UUID-writeout (test results)

                    reportDirectoryPath =
                        Path.Combine(outputDirectory.FullName, resultOutputDirectoryName,
                            standAloneDirectoryName);

                    string extensionReadyReportFileName = string.Format(StandaloneTestReportFile, outputSip.Id, "{0}");
                    return Path.Combine(reportDirectoryPath, extensionReadyReportFileName); // NB! UUID-writeout (test results)
                }
                case InputDiasPackage inputIp: // Test-report export 
                {
                    string standAloneDirectoryName =
                        string.Format(StandaloneTestReportDirectory, inputIp.Id); // NB! UUID-writeout (test results)

                    reportDirectoryPath = Path.Combine(outputDirectory.FullName, standAloneDirectoryName);

                    string extensionReadyReportFileName = string.Format(StandaloneTestReportFile, inputIp.Id, "{0}");
                    return Path.Combine(reportDirectoryPath, extensionReadyReportFileName); // NB! UUID-writeout (test results)
                }
                case null: // Test-report export - archive extraction input (not within a DIAS package) 
                {
                    var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    string standAloneDirectoryName = string.Format(StandaloneTestReportDirectory, timestamp);

                    reportDirectoryPath = Path.Combine(outputDirectory.FullName, standAloneDirectoryName);

                    string extensionReadyReportFileName = string.Format(StandaloneTestReportFile, timestamp, "{0}");
                    return Path.Combine(reportDirectoryPath, extensionReadyReportFileName);
                }
                default: throw new NotSupportedException();
            }
        }

        private static IReportGenerator GetReportGenerator(TestReportFormat testReportFormat,
            int testResultDisplayLimit)
        {
            return testReportFormat switch
            {
                TestReportFormat.html => new HtmlReportGenerator(testResultDisplayLimit),
                TestReportFormat.xml => new XmlReportGenerator(),
                TestReportFormat.json => new JsonReportGenerator(),
                TestReportFormat.pdf => new PdfReportGenerator(testResultDisplayLimit),
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
