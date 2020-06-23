using System;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Xunit;
using FluentAssertions;

namespace Arkivverket.Arkade.CLI.Tests
{
    public class ProgramTests : IDisposable
    {
        // Establish needed paths:
        private static readonly string workingDirectoryPath;
        private static readonly string testDataDirectoryPath;
        private static readonly string metadataFilePath;
        private static readonly string archiveDirectoryPath;
        private static readonly string outputDirectoryPath;

        static ProgramTests()
        {
            workingDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            testDataDirectoryPath = Path.Combine(workingDirectoryPath, "TestData");
            metadataFilePath = Path.Combine(testDataDirectoryPath, "metadata.json");
            archiveDirectoryPath = Path.Combine(testDataDirectoryPath, "N5-archive");
            outputDirectoryPath = Path.Combine(testDataDirectoryPath, "output");
        }

        public void Dispose()
        {
            if (File.Exists(metadataFilePath))
                File.Delete(metadataFilePath);

            if (Directory.Exists(outputDirectoryPath))
                Directory.Delete(outputDirectoryPath, true);

            ArkadeProcessingArea.Destroy();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void GenerateCommandTest()
        {
            // Clear needed paths:

            File.Delete(metadataFilePath);

            // Run commands and store results:

            Program.Main(new[]
            {
                "generate",
                "-m", metadataFilePath,
                "-p", testDataDirectoryPath
            });

            bool metadataWasGenerated = File.Exists(metadataFilePath);

            // Control result:

            metadataWasGenerated.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void TestCommandTest()
        {
            // Clear needed paths:

            if (Directory.Exists(outputDirectoryPath))
                Directory.Delete(outputDirectoryPath, true);
            Directory.CreateDirectory(outputDirectoryPath);

            // Run commands and store results:

            Program.Main(new[]
            {
                "test",
                "-a", archiveDirectoryPath,
                "-t", "noark5",
                "-p", testDataDirectoryPath,
                "-o", outputDirectoryPath
            });

            FileSystemInfo[] outputDirectoryItems = new DirectoryInfo(outputDirectoryPath).GetFileSystemInfos();
            bool testReportWasCreated = outputDirectoryItems.Any(item => item.Name.StartsWith("Arkaderapport"));

            // Control result:

            testReportWasCreated.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void PackCommandTest()
        {
            // Clear needed paths:

            File.Delete(metadataFilePath);

            if (Directory.Exists(outputDirectoryPath))
                Directory.Delete(outputDirectoryPath, true);
            Directory.CreateDirectory(outputDirectoryPath);

            // Run commands and store results:

            Program.Main(new[]
            {
                "generate",
                "-m", metadataFilePath,
                "-p", testDataDirectoryPath
            });

            Program.Main(new[]
            {
                "pack",
                "-a", archiveDirectoryPath,
                "-t", "noark5",
                "-m", metadataFilePath,
                "-p", testDataDirectoryPath,
                "-o", outputDirectoryPath
            });

            FileSystemInfo[] outputDirectoryItems = new DirectoryInfo(outputDirectoryPath).GetFileSystemInfos();
            bool packageWasCreated = outputDirectoryItems.Any(item => item.Name.StartsWith("Arkadepakke"));

            // Control result:

            packageWasCreated.Should().BeTrue();
        }

        [Fact(Skip = "I/O issues")]
        [Trait("Category", "Integration")]
        public void ProcessCommandTest()
        {
            // Clear needed paths:

            File.Delete(metadataFilePath);

            if (Directory.Exists(outputDirectoryPath))
                Directory.Delete(outputDirectoryPath, true);
            Directory.CreateDirectory(outputDirectoryPath);

            // Run commands and store results:

            Program.Main(new[]
            {
                "generate",
                "-m", metadataFilePath,
                "-p", testDataDirectoryPath
            });

            Program.Main(new[]
            {
                "process",
                "-a", archiveDirectoryPath,
                "-t", "noark5",
                "-m", metadataFilePath,
                "-p", testDataDirectoryPath,
                "-o", outputDirectoryPath
            });

            FileSystemInfo[] outputDirectoryItems = new DirectoryInfo(outputDirectoryPath).GetFileSystemInfos();
            bool testReportWasCreated = outputDirectoryItems.Any(item => item.Name.StartsWith("Arkaderapport"));
            bool packageWasCreated = outputDirectoryItems.Any(item => item.Name.StartsWith("Arkadepakke"));

            // Control results:

            testReportWasCreated.Should().BeTrue();
            packageWasCreated.Should().BeTrue();
        }
    }
}
