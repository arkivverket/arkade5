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
        public void GetLobTablePathsWithColumnIndexFromFromSiard2_1ArchiveFileTest()
        {
            ISiardArchiveReader siardArchiveReader = new SiardArchiveReader();

            string siardArchivePath = Path.Combine("TestData", "Siard", "testuttrekk_med_blobs.siard");

            Dictionary<string, List<int>> lobFolderPathsWithColumnIndex = siardArchiveReader.GetLobFolderPathsWithColumnIndexes(siardArchivePath);

            lobFolderPathsWithColumnIndex.Should().NotBeEmpty();
            lobFolderPathsWithColumnIndex.Should().ContainKey("schema0/table3/lob6");
            lobFolderPathsWithColumnIndex["schema0/table3/lob6"].Should().Contain(6);
        }

        [Fact]
        public void GetLobTablePathsWithColumnIndexFromFromSiard1_0ArchiveFileTest()
        {
            ISiardArchiveReader siardArchiveReader = new SiardArchiveReader();

            string siardArchivePath = Path.Combine("TestData", "Siard", "siard1_med_blobs.siard");

            Dictionary<string, List<int>> lobFolderPathsWithColumnIndex = siardArchiveReader.GetLobFolderPathsWithColumnIndexes(siardArchivePath);

            lobFolderPathsWithColumnIndex.Should().NotBeEmpty();
            lobFolderPathsWithColumnIndex.Should().ContainKey("schema0/table0/lob7");
            lobFolderPathsWithColumnIndex["schema0/table0/lob7"].Should().Contain(7);
        }

        [Fact]
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