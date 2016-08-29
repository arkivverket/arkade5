using System;
using Arkivverket.Arkade.Util;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using System.IO;

namespace Arkivverket.Arkade.Test.Util
{
    public class TarTests
    {
        private readonly ITestOutputHelper _output;

        public TarTests(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void CompressAndExtractArchive()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            var pathSourceFolder = AppDomain.CurrentDomain.BaseDirectory + $"\\TestData\\Noark5\\AliceInWonderland\\";
            var pathToTargetTarFile = AppDomain.CurrentDomain.BaseDirectory + $"\\TestData\\Noark5\\testSuiteArchive{timestamp}.tar";
            var pathToExtractedDirectory = AppDomain.CurrentDomain.BaseDirectory + $"\\TestData\\Noark5\\ExtractedData{timestamp}\\";

            _output.WriteLine("Source folder   : " + pathSourceFolder);
            _output.WriteLine("Archived TAR    : " + pathToTargetTarFile);
            _output.WriteLine("Extracted folder: " + pathToExtractedDirectory);

            int resultTar = new TarCompressionUtility().CompressFolderContentToArchiveFile(pathToTargetTarFile, pathSourceFolder);
            resultTar.Should().Be(0);
            _output.WriteLine("Generated Archive");

            int resultUnTar = new TarCompressionUtility().ExtractFolderFromArchive(pathToTargetTarFile, pathToExtractedDirectory);
            resultUnTar.Should().Be(0);
            _output.WriteLine("Extracted Archive");

            // Check that correct number of files were extracted:
            string[] filenames = Directory.GetFiles(pathToExtractedDirectory, "*.*", System.IO.SearchOption.AllDirectories);
            int numFilesExtracted = filenames.Length;
            numFilesExtracted.Should().Be(12);
            _output.WriteLine("Verified file count in extracted Archive");

            // Clean up temp directory
            Directory.Delete(pathToExtractedDirectory, true);
            File.Delete(pathToTargetTarFile);
        }




        public void ExtractFileToDestinationFolder()
        {
            var pathToFile = AppDomain.CurrentDomain.BaseDirectory + "\\TestData\\Noark5\\noark5_testdata-alice-in-wonderland.tar";
            var pathToExtractedDirectory = AppDomain.CurrentDomain.BaseDirectory + $"\\TestData\\Noark5\\ExtractedData{DateTime.Now.ToString("yyyyMMddHHmmss")}\\";
            _output.WriteLine("TAR file path: " + pathToFile);
            _output.WriteLine("TAR out directory (deleted at end of test): " + pathToExtractedDirectory);

            int result = new TarCompressionUtility().ExtractFolderFromArchive(pathToFile, pathToExtractedDirectory);
            result.Should().Be(0);

            // Check that correct number of files were extracted:
            string[] filenames = Directory.GetFiles(pathToExtractedDirectory, "*.*", System.IO.SearchOption.AllDirectories);
            int numFilesExtracted = filenames.Length;

            numFilesExtracted.Should().Be(12);

            // Clean up temp directory
            Directory.Delete(pathToExtractedDirectory, true);
            _output.WriteLine($"Function result: {result}");
        }


        public void CompressSourceFolderToArchiveFile()
        {
            var pathToTargetTarFile = AppDomain.CurrentDomain.BaseDirectory + $"\\TestData\\Noark5\\testSuiteArchive{DateTime.Now.ToString("yyyyMMddHHmmss")}.tar";
            var pathSourceFolder = AppDomain.CurrentDomain.BaseDirectory + $"\\TestData\\Noark5\\AliceInWonderland\\";
            _output.WriteLine("TAR file path: " + pathToTargetTarFile);
            _output.WriteLine("Source folder: " + pathSourceFolder);

            int result = new TarCompressionUtility().CompressFolderContentToArchiveFile(pathToTargetTarFile, pathSourceFolder);
            result.Should().Be(0);

            // Check that correct number of files were extracted:

            // Clean up temp file
            _output.WriteLine($"Function result: {result}");
        }
    }
}
