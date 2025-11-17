using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Identify;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Identify
{
    public class ArchiveTypeIdentifierTests
    {
        private readonly IArchiveTypeIdentifier _archiveTypeIdentifier;

        public ArchiveTypeIdentifierTests()
        {
            _archiveTypeIdentifier = new ArchiveTypeIdentifier();
        }

        [Fact()]
        public void IdentifyTypeOfChosenArchiveDirectoryTest()
        {
            string n3DirectoryPath = Path.Combine("TestData", "noark3");
            string n5DirectoryPath = Path.Combine("TestData", "Noark5", "Noark5Archive");
            string specializedSystemDirectoryPath = Path.Combine("TestData", "fagsystem", "autodetect");

            ArchiveType? n3ArchiveType = _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveDirectory(n3DirectoryPath);

            n3ArchiveType.Should().Be(ArchiveType.Noark3);
            n3ArchiveType.Should().NotBe(ArchiveType.Noark5);
            n3ArchiveType.Should().NotBe(ArchiveType.SpecializedSystem);

            ArchiveType? n5ArchiveType = _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveDirectory(n5DirectoryPath);

            n5ArchiveType.Should().Be(ArchiveType.Noark5);
            n5ArchiveType.Should().NotBe(ArchiveType.Noark3);
            n5ArchiveType.Should().NotBe(ArchiveType.SpecializedSystem);

            ArchiveType? specializedSystemArchiveType = _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveDirectory(specializedSystemDirectoryPath);

            specializedSystemArchiveType.Should().Be(ArchiveType.SpecializedSystem);
            specializedSystemArchiveType.Should().NotBe(ArchiveType.Noark3);
            specializedSystemArchiveType.Should().NotBe(ArchiveType.Noark5);

        }

        [Fact()]
        public void IdentifyTypeOfChosenArchiveFileTest()
        {
            string tarTestDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "tar");

            string n3TarArchivePath = Path.Combine(tarTestDataPath, "n3-eksempel", "n3-guid.tar");
            string n5TarArchivePath = Path.Combine(tarTestDataPath, "n5-eksempel", "n5-guid.tar");
            string specializedSystemTarArchivePath = Path.Combine(tarTestDataPath, "fagsystem-eksempel", "fs-guid.tar");

            ArchiveType? n5ArchiveType = _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveFile(n5TarArchivePath);

            n5ArchiveType.Should().Be(ArchiveType.Noark5);
            n5ArchiveType.Should().NotBe(ArchiveType.SpecializedSystem);
            n5ArchiveType.Should().NotBe(ArchiveType.Noark3);

            ArchiveType? n3ArchiveType = _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveFile(n3TarArchivePath);

            n3ArchiveType.Should().Be(ArchiveType.Noark3);
            n3ArchiveType.Should().NotBe(ArchiveType.Noark5);
            n3ArchiveType.Should().NotBe(ArchiveType.SpecializedSystem);

            ArchiveType? specializedSystemArchiveType = _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveFile(specializedSystemTarArchivePath);

            specializedSystemArchiveType.Should().Be(ArchiveType.SpecializedSystem);
            specializedSystemArchiveType.Should().NotBe(ArchiveType.Noark3);
            specializedSystemArchiveType.Should().NotBe(ArchiveType.Noark5);
        }
    }
}