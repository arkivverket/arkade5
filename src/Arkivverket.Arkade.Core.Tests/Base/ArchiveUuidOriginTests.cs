using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Tests.Testing.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base
{
    public class UuidOriginTests
    {
        private static readonly string TestDataDirectory =
            Path.Combine(TestUtil.TestDataDirectory, "UUID-origin-control");

        [Fact]
        public void NewUuidIsGeneratedWhenInputIsArchiveContentDirectory()
        {
            string n5ExtractContentDirectory = Path.Combine(TestDataDirectory, "Noark5-extract", "content"); // No UUID

            ArchiveDirectory archiveDirectory = ArchiveDirectory.Read(n5ExtractContentDirectory, ArchiveType.Noark5);

            Archive archive = GetResultingArchive(archiveDirectory);

            archive.NewUuid.Should().BeOfType<Uuid>();
            archive.OriginalUuid.Should().BeNull();
        }

        [Fact]
        public void OriginalUuidIsPreservedWhenInputIsDiasTarFile()
        {
            string n5ExtractTarFile = Path.Combine(TestDataDirectory, "258e3353-cef2-407f-92ac-264ad887527b.tar");

            ArchiveFile archiveFile = ArchiveFile.Read(n5ExtractTarFile, ArchiveType.Noark5);

            Archive archive = GetResultingArchive(archiveFile);

            archive.OriginalUuid.ToString().Should().Be("258e3353-cef2-407f-92ac-264ad887527b");
            archive.NewUuid.Should().BeNull();
        }

        [Fact]
        public void NewUuidIsGeneratedWhenInputIsDiasTarFileWithOriginalUuidMissing()
        {
            string n5ExtractTarFile = Path.Combine(TestDataDirectory, "invalid-uuid.tar");

            ArchiveFile archiveFile = ArchiveFile.Read(n5ExtractTarFile, ArchiveType.Noark5);

            Archive archive = GetResultingArchive(archiveFile);

            archive.NewUuid.Should().BeOfType<Uuid>();
            archive.OriginalUuid.Should().BeNull();
        }

        [Fact]
        public void NewUuidIsGeneratedWhenInputIsSiardArchiveFile()
        {
            string siardArchiveFile = Path.Combine(TestDataDirectory, "Siard-extract.siard");

            ArchiveFile archiveFile = ArchiveFile.Read(siardArchiveFile, ArchiveType.Siard);

            Archive archive = GetResultingArchive(archiveFile);

            archive.NewUuid.Should().BeOfType<Uuid>();
            archive.OriginalUuid.Should().BeNull();
        }

        private static Archive GetResultingArchive(object archiveExtract)
        {
            ArkadeProcessingArea.Establish(TestDataDirectory);

            TestSession testSession = archiveExtract is ArchiveDirectory archiveDirectory
                ? new Core.Base.Arkade().CreateTestSession(archiveDirectory)
                : new Core.Base.Arkade().CreateTestSession((ArchiveFile)archiveExtract);

            ArkadeProcessingArea.Destroy();

            return testSession.Archive;
        }
    }
}
