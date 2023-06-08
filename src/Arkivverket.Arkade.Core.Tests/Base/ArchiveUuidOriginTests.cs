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

            archive.Uuid.Should().BeOfType<Uuid>();
        }

        [Fact]
        public void OriginalUuidIsPreservedWhenInputIsDiasTarFile()
        {
            string n5ExtractTarFile = Path.Combine(TestDataDirectory, "Noark5-extract.tar"); // UUID defined in mets

            ArchiveFile archiveFile = ArchiveFile.Read(n5ExtractTarFile, ArchiveType.Noark5);

            Archive archive = GetResultingArchive(archiveFile);

            archive.Uuid.ToString().Should().Be("258e3353-cef2-407f-92ac-264ad887527b");
        }

        [Fact]
        public void NewUuidIsGeneratedWhenInputIsDiasTarFileWithOriginalUuidMissing()
        {
            throw new NotImplementedException();
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
