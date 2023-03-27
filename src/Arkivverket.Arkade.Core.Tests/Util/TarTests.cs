using System;
using System.IO;
using System.Text;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using ICSharpCode.SharpZipLib.Tar;
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

        [Fact]
        public void ExtractAndGenerateChecksumForTarEntry()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            var pathSourceFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "TestData\\Noark5\\AliceInWonderland\\");
            var pathToTargetTarFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + $"TestData\\Noark5\\testSuiteArchive{timestamp}.tar");
            var directoryWithExtractedFile = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + $"TestData\\Noark5\\ExtractedData{timestamp}\\");

            _output.WriteLine("Source folder   : " + pathSourceFolder);
            _output.WriteLine("Archived TAR    : " + pathToTargetTarFile);
            _output.WriteLine("Extracted folder: " + directoryWithExtractedFile);

            new TarCompressionUtility().CompressFolderContentToArchiveFile(pathToTargetTarFile, pathSourceFolder);
            _output.WriteLine("Generated Archive");

            directoryWithExtractedFile.Create();

            var checksum = "";
            string targetFileFullName = Path.Combine(directoryWithExtractedFile.FullName, "5011115.pdf");

            using (var tarInputStream = new TarInputStream(pathToTargetTarFile.OpenRead(), Encoding.UTF8))
            {
                while (tarInputStream.GetNextEntry() is { Name: { } } entry)
                {
                    if (!entry.Name.Replace('\\', '/').Contains("dokumenter/5011115.pdf")) continue;

                    checksum = new TarCompressionUtility().ExtractEntryAndGenerateSHA256Checksum(tarInputStream, targetFileFullName);
                }
            }

            File.Exists(targetFileFullName).Should().BeTrue();

            directoryWithExtractedFile.Delete(true);

            checksum.Should().Be("E7DA31840F396A9A58A2C7EF60632FCDBFD24BE2F8F3B3C03040363D1E33A3E7");
        }
    }
}