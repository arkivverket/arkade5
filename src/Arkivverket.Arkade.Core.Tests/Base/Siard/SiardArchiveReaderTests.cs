using Xunit;
using Arkivverket.Arkade.Core.Base.Siard;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.ExternalModels.Metadata;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;

namespace Arkivverket.Arkade.Core.Tests.Base.Siard
{
    public class SiardArchiveReaderTests
    {
        [Fact]
        [Trait("Category", "Integration")]
        public void GetLobTablePathsWithColumnIndexFromSiard2_1ArchiveFileCreatedBySiardGuiTest()
        {
            ISiardArchiveReader siardArchiveReader = new SiardArchiveReader();

            string siardArchivePath = Path.Combine("TestData", "Siard", "siard2-1_med_eksterne_lobs", "siardGui", "siardGui.siard");

            Dictionary<string, List<SiardLobReference>> lobFolderPathsWithColumnIndex = siardArchiveReader.GetLobFolderPathsWithColumnIndexes(siardArchivePath);

            lobFolderPathsWithColumnIndex.Should().NotBeEmpty();
            lobFolderPathsWithColumnIndex.Should().ContainKey("../lobs/blobs");
            lobFolderPathsWithColumnIndex.Should().ContainKey("../lobs/clobs");
            lobFolderPathsWithColumnIndex["../lobs/blobs"].Should().Contain(s => s.Column.Index == 9);
            lobFolderPathsWithColumnIndex["../lobs/clobs"].Should().Contain(s => s.Column.Index == 9);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void GetLobTablePathsWithColumnIndexFromSiard2_1ArchiveFileCreatedByDatabasePreservationToolkitTest()
        {
            ISiardArchiveReader siardArchiveReader = new SiardArchiveReader();

            string siardArchivePath = Path.Combine("TestData", "Siard", "siard2-1_med_eksterne_lobs", "dbPtk", "dbptk.siard");

            Dictionary<string, List<SiardLobReference>> lobFolderPathsWithColumnIndex = siardArchiveReader.GetLobFolderPathsWithColumnIndexes(siardArchivePath);

            lobFolderPathsWithColumnIndex.Should().NotBeEmpty();
            lobFolderPathsWithColumnIndex.Should().ContainKey("schema1\\table1\\lob9");
            lobFolderPathsWithColumnIndex.Should().ContainKey("schema1\\table2\\lob9");
            lobFolderPathsWithColumnIndex["schema1\\table1\\lob9"].Should().Contain(s => s.Column.Index == 9);
            lobFolderPathsWithColumnIndex["schema1\\table2\\lob9"].Should().Contain(s => s.Column.Index == 9);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void GetLobTablePathsWithColumnIndexFromSiard2_1ArchiveFileCreatedByDatabaseSpectralCoreFullConvertTest()
        {
            ISiardArchiveReader siardArchiveReader = new SiardArchiveReader();

            string siardArchivePath = Path.Combine("TestData", "Siard", "siard2-1_med_eksterne_lobs", "fullConvert", "scfc.siard");

            Dictionary<string, List<SiardLobReference>> lobFolderPathsWithColumnIndex = siardArchiveReader.GetLobFolderPathsWithColumnIndexes(siardArchivePath);

            lobFolderPathsWithColumnIndex.Should().NotBeEmpty();
            lobFolderPathsWithColumnIndex.Should().ContainKey("..\\t01bclob12_scfc1654_ext.siard_documents\\content\\schema0/table0/lob9");
            lobFolderPathsWithColumnIndex.Should().ContainKey("..\\t01bclob12_scfc1654_ext.siard_documents\\content\\schema0/table1/lob9");
            lobFolderPathsWithColumnIndex["..\\t01bclob12_scfc1654_ext.siard_documents\\content\\schema0/table0/lob9"].Should().Contain(s => s.Column.Index == 9);
            lobFolderPathsWithColumnIndex["..\\t01bclob12_scfc1654_ext.siard_documents\\content\\schema0/table1/lob9"].Should().Contain(s => s.Column.Index == 9);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void GetLobTablePathsWithColumnIndexFromSiard2_1ArchiveFileTest()
        {
            ISiardArchiveReader siardArchiveReader = new SiardArchiveReader();

            string siardArchivePath = Path.Combine("TestData", "Siard", "testuttrekk_med_blobs.siard");

            Dictionary<string, List<SiardLobReference>> lobFolderPathsWithColumnIndex = siardArchiveReader.GetLobFolderPathsWithColumnIndexes(siardArchivePath);

            lobFolderPathsWithColumnIndex.Should().NotBeEmpty();
            lobFolderPathsWithColumnIndex.Should().ContainKey("schema0/table3/lob6");
            lobFolderPathsWithColumnIndex["schema0/table3/lob6"].Should().Contain(s => s.Column.Index == 6);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void GetLobTablePathsWithColumnIndexFromSiard1_0ArchiveFileTest()
        {
            ISiardArchiveReader siardArchiveReader = new SiardArchiveReader();

            string siardArchivePath = Path.Combine("TestData", "Siard", "siard1_med_blobs.siard");

            Dictionary<string, List<SiardLobReference>> lobFolderPathsWithColumnIndex = siardArchiveReader.GetLobFolderPathsWithColumnIndexes(siardArchivePath);

            lobFolderPathsWithColumnIndex.Should().NotBeEmpty();
            lobFolderPathsWithColumnIndex.Should().ContainKey("schema0\\table0\\lob7");
            lobFolderPathsWithColumnIndex["schema0\\table0\\lob7"].Should().Contain(s => s.Column.Index == 7);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void GetMetadataXmlFromSiard2_1ArchiveFileTest()
        {
            ISiardArchiveReader siardArchiveReader = new SiardArchiveReader();

            const string namedEntry = ArkadeConstants.SiardMetadataXmlFileName;

            string siardArchiveFilePath = Path.Combine("TestData", "Siard", "testuttrekk_med_blobs.siard");

            using var siardFileStream = new FileStream(siardArchiveFilePath, FileMode.Open, FileAccess.Read);

            string metadataXmlStringContent = siardArchiveReader.GetNamedEntryFromSiardFileStream(siardFileStream, namedEntry);

            metadataXmlStringContent.Should().NotBeEmpty();

            siardArchive siardArchive = null;

            try
            {
                siardArchive = SerializeUtil.DeserializeFromString<siardArchive>(metadataXmlStringContent);
            }
            finally
            {
                siardArchive.Should().NotBe(null);
            }
        }
    }
}