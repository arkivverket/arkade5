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
                Path.Combine("TestData", "Archives", "Noark3", "extraction")).Should().Be(ArchiveType.Noark3);

            _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveDirectory(
                Path.Combine("TestData", "Archives", "Noark5", "extraction")).Should().Be(ArchiveType.Noark5);

            _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveDirectory(
                Path.Combine("TestData", "Archives", "SpecializedSystem", "extraction")).Should().Be(ArchiveType.SpecializedSystem);

            _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveDirectory(
                Path.Combine("TestData","Archives", "Siard", "extraction")).Should().Be(ArchiveType.Siard);

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
            string tarTestDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");

            _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveFile(
                Path.Combine(tarTestDataPath, "Archives", "Noark5", "diasPackage", "4b73981c-1fab-4d4c-91e7-fcc6a3bc057f.tar")).Should().Be(ArchiveType.Noark5);

            _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveFile(
                Path.Combine(tarTestDataPath, "Archives", "Noark3", "diasPackage", "8851c420-80e0-4681-b838-8eeb542d46f1.tar")).Should().Be(ArchiveType.Noark3);

            _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveFile(
                Path.Combine(tarTestDataPath, "Archives", "SpecializedSystem", "diasPackage", "bf193afe-4483-4481-b457-e9ba4f19681c.tar")).Should().Be(ArchiveType.SpecializedSystem);
            
            _archiveTypeIdentifier.IdentifyTypeOfChosenArchiveFile(
                Path.Combine("TestData", "Archives", "Siard", "diasPackage", "841c0a18-7308-4407-b421-3efaa420d891.tar")).Should().Be(ArchiveType.Siard);
        }
    }
}
