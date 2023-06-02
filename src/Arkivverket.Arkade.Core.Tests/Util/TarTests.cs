using System;
using System.IO;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Arkivverket.Arkade.Core.Tests.Util
{
    public class TarTests
    {
        public TarTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;


        [Fact]
        public void CompressAndExtractArchive()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            var pathSourceFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\TestData\\Noark5\\AliceInWonderland\\");
            var pathToTargetTarFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + $"\\TestData\\Noark5\\testSuiteArchive{timestamp}.tar");
            var pathToExtractedDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + $"\\TestData\\Noark5\\ExtractedData{timestamp}\\");

            _output.WriteLine("Source folder   : " + pathSourceFolder);
            _output.WriteLine("Archived TAR    : " + pathToTargetTarFile);
            _output.WriteLine("Extracted folder: " + pathToExtractedDirectory);

            new TarCompressionUtility().CompressFolderContentToArchiveFile(pathToTargetTarFile, pathSourceFolder);
            _output.WriteLine("Generated Archive");

            new TarCompressionUtility().ExtractFolderFromArchive(pathToTargetTarFile, pathToExtractedDirectory, false, null);
            _output.WriteLine("Extracted Archive");

            // Check that correct number of files were extracted:
            var filenames = System.IO.Directory.GetFiles(pathToExtractedDirectory.FullName, "*.*", SearchOption.AllDirectories);
            var numFilesExtracted = filenames.Length;
            numFilesExtracted.Should().Be(12);
            _output.WriteLine("Verified file count in extracted Archive");

            // Clean up temp directory
            System.IO.Directory.Delete(pathToExtractedDirectory.FullName, true);
            File.Delete(pathToTargetTarFile.FullName);
        }
    }
}