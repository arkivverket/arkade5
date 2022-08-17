using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Arkivverket.Arkade.Core.Resources;
using FluentAssertions;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using Moq;

namespace Arkivverket.Arkade.Core.Tests.Util.FileFormatIdentification
{
    public class FileFormatInfoFilesGeneratorTests
    {
        private readonly FileFormatInfoFilesGenerator _generator;

        public FileFormatInfoFilesGeneratorTests()
        {
            _generator = new FileFormatInfoFilesGenerator();
        }

        [Fact]
        public void GenerateTest()
        {
            const string analyseTarget = "target";
            const FileFormatScanMode scanMode = FileFormatScanMode.Directory;

            string workingDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "Report", "FilesToBeListed");
            string testFilePath = Path.Combine(workingDirectoryPath, OutputFileNames.FileFormatInfoFile);

            if (File.Exists(testFilePath))
                File.Delete(testFilePath);

            var fileFormatAnalyser = new Mock<IFileFormatIdentifier>();
            fileFormatAnalyser.Setup(f => f.IdentifyFormats(analyseTarget, scanMode)).Returns(new List<IFileFormatInfo>());

            IEnumerable<IFileFormatInfo> analysedFiles = fileFormatAnalyser.Object.IdentifyFormats(analyseTarget, scanMode);
            _generator.Generate(analysedFiles, analyseTarget, testFilePath);

            File.Exists(testFilePath).Should().BeTrue();

            Directory.EnumerateFiles(workingDirectoryPath, "*.csv").ToList().ForEach(File.Delete);
        }
    }
}
