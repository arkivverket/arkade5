using Xunit;
using Arkivverket.Arkade.Core.Base.Siard;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using FluentAssertions;

namespace Arkivverket.Arkade.Core.Tests.Base.Siard
{
    public class SiardXmlTableReaderTests
    {
        [Fact]
        [Trait("Category", "Integration")]
        public void GetFormatAnalysedLobsFromSiard2_1FileTest()
        {
            var xmlTableReader = new SiardXmlTableReader(new SiardArchiveReader(), new SiegfriedFileFormatIdentifier());

            string siardArchivePath = Path.Combine("TestData", "Siard", "testuttrekk_med_blobs.siard");

            List<IFileFormatInfo> formatAnalysedLobs = xmlTableReader.GetFormatAnalysedLobs(siardArchivePath);

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
            var xmlTableReader = new SiardXmlTableReader(new SiardArchiveReader(), new SiegfriedFileFormatIdentifier());

            string archiveFolder = Path.Combine("TestData", "Siard", "siard1_med_blobs.siard");

            List<IFileFormatInfo> formatAnalysedLobs = xmlTableReader.GetFormatAnalysedLobs(archiveFolder);

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
            var xmlTableReader = new SiardXmlTableReader(new SiardArchiveReader(), new SiegfriedFileFormatIdentifier());

            string siardArchivePath = Path.Combine("TestData", "Siard", "siard2", "siardGui", "external", "siardGui.siard");

            List<IFileFormatInfo> formatAnalysedLobs = xmlTableReader.GetFormatAnalysedLobs(siardArchivePath);

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
            var xmlTableReader = new SiardXmlTableReader(new SiardArchiveReader(), new SiegfriedFileFormatIdentifier());

            string siardArchivePath = Path.Combine("TestData", "Siard", "siard2", "siardGui", "internal", "siardGui.siard");

            List<IFileFormatInfo> formatAnalysedLobs = xmlTableReader.GetFormatAnalysedLobs(siardArchivePath);

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
            var xmlTableReader = new SiardXmlTableReader(new SiardArchiveReader(), new SiegfriedFileFormatIdentifier());

            string siardArchivePath = Path.Combine("TestData", "Siard", "siard2", "dbPtk", "external", "dbptk.siard");

            List<IFileFormatInfo> formatAnalysedLobs = xmlTableReader.GetFormatAnalysedLobs(siardArchivePath);

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
            var xmlTableReader = new SiardXmlTableReader(new SiardArchiveReader(), new SiegfriedFileFormatIdentifier());

            string siardArchivePath = Path.Combine("TestData", "Siard", "siard2", "dbPtk", "internal", "dbptk.siard");

            List<IFileFormatInfo> formatAnalysedLobs = xmlTableReader.GetFormatAnalysedLobs(siardArchivePath);

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
            var xmlTableReader = new SiardXmlTableReader(new SiardArchiveReader(), new SiegfriedFileFormatIdentifier());

            string siardArchivePath = Path.Combine("TestData", "Siard", "siard2", "fullConvert", "external", "scfc.siard");

            List<IFileFormatInfo> formatAnalysedLobs = xmlTableReader.GetFormatAnalysedLobs(siardArchivePath);

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
            var xmlTableReader = new SiardXmlTableReader(new SiardArchiveReader(), new SiegfriedFileFormatIdentifier());

            string siardArchivePath = Path.Combine("TestData", "Siard", "siard2", "fullConvert", "internal", "scfc.siard");

            List<IFileFormatInfo> formatAnalysedLobs = xmlTableReader.GetFormatAnalysedLobs(siardArchivePath);

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
    }
}