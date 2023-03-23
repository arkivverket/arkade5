using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Arkivverket.Arkade.Core.Base;
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

            new TarCompressionUtility().ExtractFolderFromArchive(pathToTargetTarFile, pathToExtractedDirectory, Uuid.Random());//FIX!
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
            using Stream originalArkivstrukturStream = new MemoryStream();
            using (var tarInputStream = new TarInputStream(targetTarFile.OpenRead(), Encoding.UTF8))
            {
                var previousEntryName = "";
                var previousEntrySize = 0L;
                long numberOfEntries = 0;
                TarEntry tarEntry;
                var previousTarEntry = new TarEntry(new TarHeader());
                while ((tarEntry = tarInputStream.GetNextEntry()) != null && tarEntry.Name != null)
                {
                    /*using var entryStream = new MemoryStream();
                    tarInputStream.CopyEntryContents(entryStream);
                    var checkSum = new Sha256ChecksumGenerator().GenerateChecksum(entryStream);*/
                    //legg til sjekksum i dokumentfil-liste

                    if (numberOfEntries > 0)
                    {
                        tarEntryPositions.Add(previousEntryName, new TarEntryInfo { Position = tarInputStream.Position - previousEntrySize+1, Size = previousEntrySize });
                    }
                    numberOfEntries++;

                    if (tarEntry.Name == "arkivstrukturAbbreviated.xml")
                    {
                        var available1 = tarInputStream.Available;
                        Debug.WriteLine($"TarInputStream available: {tarInputStream.Available}");
                        Debug.WriteLine($"TarInputStream Position: {tarInputStream.Position}");
                        tarInputStream.CopyEntryContents(originalArkivstrukturStream);
                        var available2 = tarInputStream.Available;
                        Debug.WriteLine($"TarInputStream available: {tarInputStream.Available}");
                        Debug.WriteLine($"TarInputStream Position: {tarInputStream.Position}");
                    }

                    previousEntryName = tarEntry.Name;
                    previousEntrySize = tarEntry.Size;
                    previousTarEntry = tarEntry;
                    var tarHeaderSize = tarEntry.TarHeader.Size;
                }

                if (previousTarEntry.Name != null)
                {
                    tarEntryPositions.Add(previousTarEntry.Name, new TarEntryInfo { Position = tarInputStream.Position, Size = previousTarEntry.Size });
                }

                Debug.WriteLine($"TarInputStream length: {tarInputStream.Length}");
            }

            TarEntryInfo arkivstrukturXmlEntryInfo = tarEntryPositions["arkivstrukturAbbreviated.xml"];
            var arkivstrukturXmlAsBytes = new List<byte>();

            using (FileStream tmpTarInputStream = targetTarFile.OpenRead())
            {
                tmpTarInputStream.Position = arkivstrukturXmlEntryInfo.Position;
                while (tmpTarInputStream.Position < arkivstrukturXmlEntryInfo.Position + arkivstrukturXmlEntryInfo.Size)
                {
                    arkivstrukturXmlAsBytes.Add((byte)tmpTarInputStream.ReadByte());
                }
                Debug.WriteLine($"Tar as FileStream length: {tmpTarInputStream.Length}");
            }

            var originalBytes = new List<byte>();
            originalArkivstrukturStream.Position = 0;
            var index = 0;
            while (true)
            {
                var readByte = originalArkivstrukturStream.ReadByte();

                //byte fetchedByte = arkivstrukturXmlAsBytes[index];

                //if (fetchedByte != readByte)
                //{
                //    Debug.WriteLine($"At index '{index}': byte '{fetchedByte}' is not equal to original byte '{readByte}'");
                //}
                if (readByte == -1) break;

                originalBytes.Add((byte)readByte);

                index++;
            }

            //originalBytes.ToArray().Should().ContainInOrder(arkivstrukturXmlAsBytes);
            arkivstrukturXmlAsBytes.Count.Should().Be((int)arkivstrukturXmlEntryInfo.Size);
            //TODO: Check content for equality!!

            // Clean up temp directory
            File.Delete(targetTarFile.FullName);
        }

        [Fact]
        public void CompareByteContentOfTarFileTarInputStreamAndFileStream()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            var pathSourceFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\TestData\\Noark5\\AliceInWonderland\\");
            var targetTarFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + $"\\TestData\\Noark5\\testSuiteArchive{timestamp}.tar");

            _output.WriteLine("Source folder   : " + pathSourceFolder);
            _output.WriteLine("Archived TAR    : " + targetTarFile);

            new TarCompressionUtility().CompressFolderContentToArchiveFile(targetTarFile, pathSourceFolder);
            _output.WriteLine("Generated Archive");


            var tarInputStreamAsBytes = new List<byte>();
            using (var tarInputStream = new TarInputStream(targetTarFile.OpenRead(), Encoding.UTF8))
            {
                while (true)
                {
                    var readByte = tarInputStream.ReadByte();
                    if (readByte == -1) break;
                    tarInputStreamAsBytes.Add((byte)readByte);
                }
            }

            var tarFileStreamAsBytes = new List<byte>();
            using (var tarFileStream = targetTarFile.OpenRead())
            {
                while (true)
                {
                    var readByte = tarFileStream.ReadByte();
                    if (readByte == -1) break;
                    tarFileStreamAsBytes.Add((byte)readByte);
                }
            }

            tarInputStreamAsBytes.Should().ContainInOrder(tarFileStreamAsBytes);
        }

        private class TarEntryInfo
        {
            public long Position { get; set; }
            public long Size { get; set; }
        }
    }
}