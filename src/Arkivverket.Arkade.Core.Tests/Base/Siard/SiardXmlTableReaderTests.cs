using Xunit;
using Arkivverket.Arkade.Core.Base.Siard;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using FluentAssertions;
using Moq;

namespace Arkivverket.Arkade.Core.Tests.Base.Siard
{
    public class SiardXmlTableReaderTests
    {
        private readonly IFileFormatIdentifier _fileFormatIdentifier;

        public SiardXmlTableReaderTests()
        {
            IStatusEventHandler statusEventHandler = new Mock<IStatusEventHandler>().Object;
            _fileFormatIdentifier = new SiegfriedFileFormatIdentifier(new SiegfriedProcessRunner(statusEventHandler),
                statusEventHandler, new FileSystemInfoSizeCalculator(statusEventHandler));
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void GetFormatAnalysedLobsFromSiard2_1FileTest()
        {
            SiardXmlTableReader xmlTableReader = CreateReader();

            string siardArchivePath = Path.Combine("TestData", "Siard", "testuttrekk_med_blobs.siard");

            var lobStreams = xmlTableReader.CreateLobByteArrays(siardArchivePath);
            List<IFileFormatInfo> formatAnalysedLobs = _fileFormatIdentifier.IdentifyFormats(lobStreams).ToList();

            formatAnalysedLobs.Should().NotBeEmpty();
            formatAnalysedLobs.Count.Should().Be(9);
            foreach (IFileFormatInfo lob in formatAnalysedLobs)
            {
                lob.FileExtension.Should().Be(".bin");
            }
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void GetFormatAnalysedLobsFromSiard1_0FileTest()
        {
            SiardXmlTableReader xmlTableReader = CreateReader();

            string archiveFolder = Path.Combine("TestData", "Siard", "siard1_med_blobs.siard");

            var lobStreams = xmlTableReader.CreateLobByteArrays(archiveFolder);
            List<IFileFormatInfo> formatAnalysedLobs = _fileFormatIdentifier.IdentifyFormats(lobStreams).ToList();

            formatAnalysedLobs.Should().NotBeEmpty();
            formatAnalysedLobs.Count.Should().Be(10);
            foreach (IFileFormatInfo lob in formatAnalysedLobs)
            {
                lob.FileExtension.Should().Be(".bin");
            }
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void GetFormatAnalysedLobsFromSiard2_1ArchiveFileWithExternalLobsCreatedBySiardGuiTest()
        {
            SiardXmlTableReader xmlTableReader = CreateReader();

            string siardArchivePath = Path.Combine("TestData", "Siard", "siard2", "siardGui", "external", "siardGui.siard");

            var lobStreams = xmlTableReader.CreateLobByteArrays(siardArchivePath);
            List<IFileFormatInfo> formatAnalysedLobs = _fileFormatIdentifier.IdentifyFormats(lobStreams).ToList();

            formatAnalysedLobs.Should().NotBeEmpty();
            formatAnalysedLobs.Count.Should().Be(35);
            var lobCounter = 1;
            foreach (IFileFormatInfo lob in formatAnalysedLobs)
            {
                lob.FileExtension.Should().Be(lobCounter <= 28 ? ".bin" : ".txt");

                lobCounter++;
            }
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void GetFormatAnalysedLobsFromSiard2_1ArchiveFileWithInternalLobsCreatedBySiardGuiTest()
        {
            SiardXmlTableReader xmlTableReader = CreateReader();

            string siardArchivePath = Path.Combine("TestData", "Siard", "siard2", "siardGui", "internal", "siardGui.siard");

            var lobStreams = xmlTableReader.CreateLobByteArrays(siardArchivePath);
            List<IFileFormatInfo> formatAnalysedLobs = _fileFormatIdentifier.IdentifyFormats(lobStreams).ToList();

            formatAnalysedLobs.Should().NotBeEmpty();
            formatAnalysedLobs.Count.Should().Be(35);
            var lobCounter = 1;
            foreach (IFileFormatInfo lob in formatAnalysedLobs)
            {
                lob.FileExtension.Should().Be(lobCounter <= 28 ? ".bin" : ".txt");

                lobCounter++;
            }
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void GetFormatAnalysedLobsFromSiard2_1ArchiveFileWithExternalLobsCreatedByDatabasePreservationToolkitTest()
        {
            SiardXmlTableReader xmlTableReader = CreateReader();

            string siardArchivePath = Path.Combine("TestData", "Siard", "siard2", "dbPtk", "external", "dbptk.siard");

            var lobStreams = xmlTableReader.CreateLobByteArrays(siardArchivePath);
            List<IFileFormatInfo> formatAnalysedLobs = _fileFormatIdentifier.IdentifyFormats(lobStreams).ToList();

            formatAnalysedLobs.Should().NotBeEmpty();
            formatAnalysedLobs.Count.Should().Be(35);
            var lobCounter = 1;
            foreach (IFileFormatInfo lob in formatAnalysedLobs)
            {
                lob.FileExtension.Should().Be(lobCounter <= 28 ? ".bin" : ".txt");

                lobCounter++;
            }
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void GetFormatAnalysedLobsFromSiard2_1ArchiveFileWithInternalLobsCreatedByDatabasePreservationToolkitTest()
        {
            SiardXmlTableReader xmlTableReader = CreateReader();

            string siardArchivePath = Path.Combine("TestData", "Siard", "siard2", "dbPtk", "internal", "dbptk.siard");

            var lobStreams = xmlTableReader.CreateLobByteArrays(siardArchivePath);
            List<IFileFormatInfo> formatAnalysedLobs = _fileFormatIdentifier.IdentifyFormats(lobStreams).ToList();

            formatAnalysedLobs.Should().NotBeEmpty();
            formatAnalysedLobs.Count.Should().Be(35);
            var lobCounter = 1;
            foreach (IFileFormatInfo lob in formatAnalysedLobs)
            {
                switch (lobCounter)
                {
                    case 1 or 3 or 5:
                        lob.FileName.StartsWith("table1").Should().BeTrue();
                        break;
                    case 29 or 30 or 31 or 33: //file content stored as plain text in database - not possible to analyze(?)
                        lob.Errors.Should().Be(Resources.SiardMessages.InlinedLobContentHasUnsupportedEncoding);
                        break;
                    case <= 28:
                        lob.FileExtension.Should().Be(".bin");
                        break;
                    default:
                        lob.FileExtension.Should().Be(".txt");
                        break;
                }

                lobCounter++;
            }
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void GetFormatAnalysedLobsFromSiard2_1ArchiveFileWithExternalLobsCreatedBySpectralCoreFullConvertTest()
        {
            SiardXmlTableReader xmlTableReader = CreateReader();

            string siardArchivePath = Path.Combine("TestData", "Siard", "siard2", "fullConvert", "external", "scfc.siard");

            var lobStreams = xmlTableReader.CreateLobByteArrays(siardArchivePath);
            List<IFileFormatInfo> formatAnalysedLobs = _fileFormatIdentifier.IdentifyFormats(lobStreams).ToList();

            formatAnalysedLobs.Should().NotBeEmpty();
            formatAnalysedLobs.Count.Should().Be(35);
            var lobCounter = 1;
            foreach (IFileFormatInfo lob in formatAnalysedLobs)
            {
                switch (lobCounter)
                {
                    case 1 or 3 or 5:
                        lob.FileName.StartsWith("table0").Should().BeTrue();
                        break;
                    case 29 or 31 or 33: //file content stored as plain text in database - not possible to analyze(?)
                        lob.Errors.Should().Be(Resources.SiardMessages.InlinedLobContentHasUnsupportedEncoding);
                        break;
                    case <= 28:
                        lob.FileExtension.Should().Be(".bin");
                        break;
                    default:
                        lob.FileExtension.Should().Be(".txt");
                        break;
                }

                lobCounter++;
            }
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void GetFormatAnalysedLobsFromSiard2_1ArchiveFileWithInternalLobsCreatedBySpectralCoreFullConvertTest()
        {
            SiardXmlTableReader xmlTableReader = CreateReader();

            string siardArchivePath = Path.Combine("TestData", "Siard", "siard2", "fullConvert", "internal", "scfc.siard");

            var lobStreams = xmlTableReader.CreateLobByteArrays(siardArchivePath);
            List<IFileFormatInfo> formatAnalysedLobs = _fileFormatIdentifier.IdentifyFormats(lobStreams).ToList();

            formatAnalysedLobs.Should().NotBeEmpty();
            formatAnalysedLobs.Count.Should().Be(35);
            var lobCounter = 1;
            foreach (IFileFormatInfo lob in formatAnalysedLobs)
            {
                switch (lobCounter)
                {
                    case 1 or 3 or 5:
                        lob.FileName.StartsWith("table0").Should().BeTrue();
                        break;
                    case 29 or 31 or 33: //file content stored as plain text in database - not possible to analyze(?)
                        lob.Errors.Should().Be(Resources.SiardMessages.InlinedLobContentHasUnsupportedEncoding);
                        break;
                    case <= 28:
                        lob.FileExtension.Should().Be(".bin");
                        break;
                    default:
                        lob.FileExtension.Should().Be(".txt");
                        break;
                }

                lobCounter++;
            }
        }

        [Fact]
        public void ShouldGetPathsToExternalLobsReferencedFromSiardCreatedByDatabasePreservationToolkit()
        {
            const string siardArchiveDirectoryPath = "TestData/Siard/siard2/dbPtk/external";
            const string siardArchivePath = $"{siardArchiveDirectoryPath}/dbptk.siard";

            const string pathToSchemaFolder = $"{siardArchiveDirectoryPath}/t01bclob12_dbptk-desktop-2.5.9_ext.siard_lobseg_1/content/schema1";

            List<string> referencedLobs = new()
            {
                $"{pathToSchemaFolder}/table1/lob9/record1.bin",
                $"{pathToSchemaFolder}/table1/lob9/record2.bin",
                $"{pathToSchemaFolder}/table1/lob9/record3.bin",
                $"{pathToSchemaFolder}/table1/lob9/record4.bin",
                $"{pathToSchemaFolder}/table1/lob9/record5.bin",
                $"{pathToSchemaFolder}/table1/lob9/record6.bin",
                $"{pathToSchemaFolder}/table1/lob9/record7.bin",
                $"{pathToSchemaFolder}/table1/lob9/record8.bin",
                $"{pathToSchemaFolder}/table1/lob9/record9.bin",
                $"{pathToSchemaFolder}/table1/lob9/record10.bin",
                $"{pathToSchemaFolder}/table1/lob9/record11.bin",
                $"{pathToSchemaFolder}/table1/lob9/record12.bin",
                $"{pathToSchemaFolder}/table1/lob9/record13.bin",
                $"{pathToSchemaFolder}/table1/lob9/record14.bin",
                $"{pathToSchemaFolder}/table1/lob9/record15.bin",
                $"{pathToSchemaFolder}/table1/lob9/record16.bin",
                $"{pathToSchemaFolder}/table1/lob9/record17.bin",
                $"{pathToSchemaFolder}/table1/lob9/record18.bin",
                $"{pathToSchemaFolder}/table1/lob9/record19.bin",
                $"{pathToSchemaFolder}/table1/lob9/record20.bin",
                $"{pathToSchemaFolder}/table1/lob9/record21.bin",
                $"{pathToSchemaFolder}/table1/lob9/record22.bin",
                $"{pathToSchemaFolder}/table1/lob9/record23.bin",
                $"{pathToSchemaFolder}/table1/lob9/record24.bin",
                $"{pathToSchemaFolder}/table1/lob9/record25.bin",
                $"{pathToSchemaFolder}/table1/lob9/record26.bin",
                $"{pathToSchemaFolder}/table1/lob9/record27.bin",
                $"{pathToSchemaFolder}/table1/lob9/record28.bin",
                $"{pathToSchemaFolder}/table2/lob9/record1.txt",
                $"{pathToSchemaFolder}/table2/lob9/record2.txt",
                $"{pathToSchemaFolder}/table2/lob9/record3.txt",
                $"{pathToSchemaFolder}/table2/lob9/record4.txt",
                $"{pathToSchemaFolder}/table2/lob9/record5.txt",
                $"{pathToSchemaFolder}/table2/lob9/record6.txt",
                $"{pathToSchemaFolder}/table2/lob9/record7.txt"
            };

            const string pathToNonReferencedLob = $"{pathToSchemaFolder}/t01bclob12_dbptk-desktop-2.5.9_ext.siard_lobseg_1/content/schema1/table1/lob9/unreferenced-file.bin";

            ValidateFacts(siardArchivePath, referencedLobs, pathToNonReferencedLob);
        }

        [Fact]
        public void ShouldGetPathsToExternalLobsReferencedFromSiardCreatedBySiardGui()
        {
            const string siardArchiveDirectoryPath = "TestData/Siard/siard2/siardGui/external";
            const string siardArchivePath = $"{siardArchiveDirectoryPath}/siardGui.siard";

            const string pathToLobsFolder = $"{siardArchiveDirectoryPath}/lobs";

            List<string> referencedLobs = new()
            {
                $"{pathToLobsFolder}/blobs/record0.bin",
                $"{pathToLobsFolder}/blobs/record1.bin",
                $"{pathToLobsFolder}/blobs/record2.bin",
                $"{pathToLobsFolder}/blobs/record3.bin",
                $"{pathToLobsFolder}/blobs/record4.bin",
                $"{pathToLobsFolder}/blobs/record5.bin",
                $"{pathToLobsFolder}/blobs/record6.bin",
                $"{pathToLobsFolder}/blobs/record7.bin",
                $"{pathToLobsFolder}/blobs/record8.bin",
                $"{pathToLobsFolder}/blobs/record9.bin",
                $"{pathToLobsFolder}/blobs/record10.bin",
                $"{pathToLobsFolder}/blobs/record11.bin",
                $"{pathToLobsFolder}/blobs/record12.bin",
                $"{pathToLobsFolder}/blobs/record13.bin",
                $"{pathToLobsFolder}/blobs/record14.bin",
                $"{pathToLobsFolder}/blobs/record15.bin",
                $"{pathToLobsFolder}/blobs/record16.bin",
                $"{pathToLobsFolder}/blobs/record17.bin",
                $"{pathToLobsFolder}/blobs/record18.bin",
                $"{pathToLobsFolder}/blobs/record19.bin",
                $"{pathToLobsFolder}/blobs/record20.bin",
                $"{pathToLobsFolder}/blobs/record21.bin",
                $"{pathToLobsFolder}/blobs/record22.bin",
                $"{pathToLobsFolder}/blobs/record23.bin",
                $"{pathToLobsFolder}/blobs/record24.bin",
                $"{pathToLobsFolder}/blobs/record25.bin",
                $"{pathToLobsFolder}/blobs/record26.bin",
                $"{pathToLobsFolder}/blobs/record27.bin",
                $"{pathToLobsFolder}/clobs/record0.txt",
                $"{pathToLobsFolder}/clobs/record1.txt",
                $"{pathToLobsFolder}/clobs/record2.txt",
                $"{pathToLobsFolder}/clobs/record3.txt",
                $"{pathToLobsFolder}/clobs/record4.txt",
                $"{pathToLobsFolder}/clobs/record5.txt",
                $"{pathToLobsFolder}/clobs/record6.txt"
            };

            const string pathToNonReferencedLob = $"{pathToLobsFolder}/blobs/unreferenced-file.bin";

            ValidateFacts(siardArchivePath, referencedLobs, pathToNonReferencedLob);
        }

        [Fact]
        public void ShouldGetPathsToExternalLobsReferencedFromSiardCreatedBySpectralCoreFullConvert()
        {
            const string siardArchiveDirectoryPath = "TestData/Siard/siard2/fullConvert/external";
            const string siardArchivePath = $"{siardArchiveDirectoryPath}/scfc.siard";

            const string pathToSchemaFolder = $"{siardArchiveDirectoryPath}/t01bclob12_scfc1654_ext.siard_documents/content/schema0";

            List<string> referencedLobs = new()
            {
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec2.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec4.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec6.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec7.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec8.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec9.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec10.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec11.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec12.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec13.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec14.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec15.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec16.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec17.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec18.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec19.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec20.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec21.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec22.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec23.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec24.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec25.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec26.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec27.bin",
                $"{pathToSchemaFolder}/table0/lob9/seg0/rec28.bin",
                $"{pathToSchemaFolder}/table1/lob9/seg0/rec2.txt",
                $"{pathToSchemaFolder}/table1/lob9/seg0/rec4.txt",
                $"{pathToSchemaFolder}/table1/lob9/seg0/rec6.txt",
                $"{pathToSchemaFolder}/table1/lob9/seg0/rec7.txt"
            };

            const string pathToNonReferencedLob = $"{pathToSchemaFolder}/table0/lob9/seg0/unreferenced-file.bin";

            ValidateFacts(siardArchivePath, referencedLobs, pathToNonReferencedLob);
        }

        private static SiardXmlTableReader CreateReader()
        {
            return new SiardXmlTableReader(new SiardArchiveReader());
        }

        private static void ValidateFacts(string siardArchivePath, List<string> referencedLobs,
            string pathToNonReferencedLob)
        {
            SiardXmlTableReader xmlTableReader = CreateReader();

            List<string> fullPathsToExternalLobs = xmlTableReader.GetFullPathsToExternalLobs(siardArchivePath).ToList();

            // Ensure only referenced files are fetched
            fullPathsToExternalLobs.Except(referencedLobs).Should().BeEmpty();

            // Ensure all referenced files are fetched
            referencedLobs.Except(fullPathsToExternalLobs).Should().BeEmpty();

            // When the former statements validates successfully, success of the following
            // statement implies that the list of fetched lob paths contains no duplicates.
            fullPathsToExternalLobs.Should().HaveCount(referencedLobs.Count);

            // Explicitly check that non referenced files are not included. Redundant, but
            // included for emphasis on the importance of not including unreferenced files.
            fullPathsToExternalLobs.Should().NotContain(pathToNonReferencedLob);
        }
    }
}