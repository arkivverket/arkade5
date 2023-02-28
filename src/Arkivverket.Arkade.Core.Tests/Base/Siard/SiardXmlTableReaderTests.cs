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

        private static SiardXmlTableReader CreateReader()
        {
            return new SiardXmlTableReader(new SiardArchiveReader());
        }
    }
}