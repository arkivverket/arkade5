using System;
using System.Collections.Generic;
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

            new TarCompressionUtility().ExtractFolderFromArchive(pathToTargetTarFile, pathToExtractedDirectory);
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
        public void ExtractCertainFilesFromTarArchive()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            var pathSourceFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\TestData\\Noark5\\AliceInWonderland\\");
            var targetTarFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + $"\\TestData\\Noark5\\testSuiteArchive{timestamp}.tar");

            _output.WriteLine("Source folder   : " + pathSourceFolder);
            _output.WriteLine("Archived TAR    : " + targetTarFile);

            new TarCompressionUtility().CompressFolderContentToArchiveFile(targetTarFile, pathSourceFolder);
            _output.WriteLine("Generated Archive");

            var tarEntryPositions = new Dictionary<string, TarEntryInfo>();
            Stream originalArkivstrukturStream = new MemoryStream();
            using (var tarInputStream = new TarInputStream(targetTarFile.OpenRead(), Encoding.UTF8))
            {
                while (tarInputStream.GetNextEntry() is { Name: { } } tarEntry)
                {
                    tarEntryPositions.Add(tarEntry.Name, new TarEntryInfo { Position = tarInputStream.Position, Size = tarEntry.Size });

                    if (tarEntry.Name == "arkivstrukturAbbreviated.xml")
                        tarInputStream.CopyEntryContents(originalArkivstrukturStream);
                }
            }

            TarEntryInfo arkivstrukturXmlEntryInfo = tarEntryPositions["arkivstrukturAbbreviated.xml"];
            var arkivstrukturXmlAsStream = new byte[arkivstrukturXmlEntryInfo.Size];

            using (FileStream tmpTarInputStream = targetTarFile.OpenRead())
            {
                tmpTarInputStream.Position = arkivstrukturXmlEntryInfo.Position;
                int bytesRead = tmpTarInputStream.ReadAsync(arkivstrukturXmlAsStream, 0, (int)arkivstrukturXmlEntryInfo.Size).Result;
            }

            var originalBytes = new List<byte>();
            originalArkivstrukturStream.Position = 0;
            while (true)
            {
                var readByte = originalArkivstrukturStream.ReadByte();

                if (readByte == -1) break;

                originalBytes.Add((byte)readByte);
            }

            originalBytes.ToArray().Should().ContainInOrder(arkivstrukturXmlAsStream);
            arkivstrukturXmlAsStream.Length.Should().Be((int)arkivstrukturXmlEntryInfo.Size);
            //TODO: Check content for equality!!

            // Clean up temp directory
            File.Delete(targetTarFile.FullName);
        }

        private class TarEntryInfo
        {
            public long Position { get; set; }
            public long Size { get; set; }
        }
    }
}