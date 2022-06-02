using System;
using System.IO;
using System.Linq;
using Xunit;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Report;
using Arkivverket.Arkade.Core.Resources;
using FluentAssertions;

namespace Arkivverket.Arkade.Core.Tests.Report
{
    public class FileFormatInfoGeneratorTests
    {
        [Fact]
        public void GenerateTest()
        {
            string workingDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "Report", "FilesToBeListed");
            string testFilePath = Path.Combine(workingDirectoryPath, OutputFileNames.FileFormatInfoFile);

            var testArchive = new Archive(ArchiveType.Noark5, null,
                new WorkingDirectory(new DirectoryInfo(workingDirectoryPath)));

            if (File.Exists(testFilePath))
                File.Delete(testFilePath);

            FileFormatInfoGenerator.Generate(testArchive.GetDocumentsDirectory(), testFilePath, new StatusEventHandler(), true);

            File.Exists(testFilePath).Should().BeTrue();

            Directory.EnumerateFiles(workingDirectoryPath, "*.csv").ToList().ForEach(File.Delete);
        }
    }
}
