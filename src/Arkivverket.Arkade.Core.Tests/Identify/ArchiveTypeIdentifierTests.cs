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
        private readonly IArchiveTypeIdentifier _archiveTypeIdentifier = new ArchiveTypeIdentifier();

        [Fact]
        public void IdentifyTypeOfChosenArchiveDirectoryTest()
        {
            _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveDirectory(
                Path.Combine("TestData", "noark3")).Should().Be(ArchiveType.Noark3);

            _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveDirectory(
                Path.Combine("TestData", "Noark5", "Noark5Archive")).Should().Be(ArchiveType.Noark5);

            _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveDirectory(
                Path.Combine("TestData", "fagsystem", "autodetect")).Should().Be(ArchiveType.SpecializedSystem);

            _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveDirectory(
                Path.Combine("TestData", "Siard", "siard2", "dbPtk", "internal")).Should().Be(ArchiveType.Siard);

            // In cases where the archive type is undeterminable, null should be the result (not an exception thrown): 

            _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveDirectory(
                Path.Combine("TestData", "TypeUndeterminableAddml", "InValidAddml")).Should().BeNull();

            _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveDirectory(
                Path.Combine("TestData", "TypeUndeterminableAddml", "UnserializableAddml")).Should().BeNull();

            _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveDirectory(
                Path.Combine("TestData", "TypeUndeterminableAddml", "ValidAddml")).Should().BeNull();
        }

        [Fact]
        public void IdentifyTypeOfChosenArchiveFileTest()
        {
            string tarTestDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "tar");

            _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveFile(
                Path.Combine(tarTestDataPath, "n5-eksempel", "n5-guid.tar")).Should().Be(ArchiveType.Noark5);

            _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveFile(
                Path.Combine(tarTestDataPath, "n3-eksempel", "n3-guid.tar")).Should().Be(ArchiveType.Noark3);

            _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveFile(
                Path.Combine(tarTestDataPath, "fagsystem-eksempel", "fs-guid.tar")).Should().Be(ArchiveType.SpecializedSystem);
            
            _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveFile(
                Path.Combine("TestData", "Siard", "dbptk_produced.siard")).Should().Be(ArchiveType.Siard);
        }
    }
}
