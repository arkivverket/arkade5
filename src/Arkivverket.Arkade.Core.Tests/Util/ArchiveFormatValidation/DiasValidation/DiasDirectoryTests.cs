using System;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.Util.ArchiveFormatValidation;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Util.ArchiveFormatValidation.DiasValidation
{
    public class DiasDirectoryTests
    {
        [Fact]
        public void GetEntryPathsTest()
        {
            var dias =
                new DiasDirectory("SomeDiasDirectory",
                    new DiasDirectory("SomeDiasSubDirectory",
                        new DiasFile("SomeDiasFile.txt")));

            IEnumerable<string> entryPaths = dias.GetEntryPaths("SomeDiasDirectory", recursive: true);

            string expectedEntryPath = Path.Combine("SomeDiasDirectory", "SomeDiasSubDirectory", "SomeDiasFile.txt");

            entryPaths.Should().Contain(e => e.Equals(expectedEntryPath));
        }

        [Fact]
        public void GetMissingEntryPathsTest()
        {
            DirectoryInfo diasOnDisk = CreateTestDiasDirectory();

            var dias =
                new DiasDirectory(diasOnDisk.FullName,
                    new DiasDirectory("SomeDiasSubDirectory",
                        new DiasFile("SomeDiasFile.txt"),
                        new DiasFile("SomeOtherDiasFile.txt") // Not on disk
                    ));

            IEnumerable<string> entryPaths =
                dias.GetEntryPaths(diasOnDisk.FullName, getNonExistingOnly: true, recursive: true);

            string expectedMissingEntryPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "SomeDiasDirectory",
                "SomeDiasSubDirectory",
                "SomeOtherDiasFile.txt"
            );

            entryPaths.Should().Contain(e => e.Equals(expectedMissingEntryPath));

            diasOnDisk.Delete(true);
        }

        private static DirectoryInfo CreateTestDiasDirectory()
        {
            DirectoryInfo diasRootDirectory = Directory.CreateDirectory(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SomeDiasDirectory")
            );

            DirectoryInfo testSubDirectory = Directory.CreateDirectory(
                Path.Combine(diasRootDirectory.FullName, "SomeDiasSubDirectory")
            );

            File.Create(Path.Combine(testSubDirectory.FullName, "SomeDiasFile.txt")).Dispose();

            return diasRootDirectory;
        }
    }
}
