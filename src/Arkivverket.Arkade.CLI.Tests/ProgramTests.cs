using System;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Util;
using Xunit;
using FluentAssertions;

namespace Arkivverket.Arkade.CLI.Tests
{
    public class ProgramTests : IDisposable
    {
        // Establish needed paths:
        private static readonly string workingDirectoryPath;
        private static readonly string metadataFilePath;
        private static readonly string noark5TestListPath;
        private static readonly string testDataDirectoryPath;
        private static readonly string archiveDirectoryPath;
        private static readonly string outputDirectoryPath;

        static ProgramTests()
        {
            workingDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            metadataFilePath = Path.Combine(workingDirectoryPath, ArkadeConstants.MetadataFileName);
            noark5TestListPath = Path.Combine(workingDirectoryPath, ArkadeConstants.Noark5TestListFileName);
            testDataDirectoryPath = Path.Combine(workingDirectoryPath, "TestData");
            archiveDirectoryPath = Path.Combine(testDataDirectoryPath, "N5-archive");
            outputDirectoryPath = Path.Combine(testDataDirectoryPath, "output");

            ClearAllPaths();
        }

        [Fact(Skip = "IO-issues")]
        [Trait("Category", "Integration")]
        public void GenerateCommandTest()
        {
            // Run commands and store results:

            Program.Main(new[]
            {
                "generate",
                "-m",
                "-l",
                "-o", workingDirectoryPath
            });

            bool metadataWasGenerated = File.Exists(metadataFilePath);
            bool noark5TestListGenerated = File.Exists(noark5TestListPath);

            // Control result:

            metadataWasGenerated.Should().BeTrue();
            noark5TestListGenerated.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void TestCommandTest()
        {
            // Prepare needed files and/or directories

            Noark5TestListGenerator.Generate(ArkadeConstants.Noark5TestListFileName);
            Directory.CreateDirectory(outputDirectoryPath);

            // Run commands and store results:

            Program.Main(new[]
            {
                "test",
                "-a", archiveDirectoryPath,
                "-t", "noark5",
                "-p", testDataDirectoryPath,
                "-o", outputDirectoryPath,
                "-l", noark5TestListPath
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
            // Prepare needed files and/or directories

            new MetadataExampleGenerator().Generate(ArkadeConstants.MetadataFileName);
            Directory.CreateDirectory(outputDirectoryPath);

            // Run commands and store results:

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

        [Fact(Skip = "IO-issues")]
        [Trait("Category", "Integration")]
        public void ProcessCommandTest()
        {
            // Prepare needed files and/or directories

            new MetadataExampleGenerator().Generate(ArkadeConstants.MetadataFileName);
            Noark5TestListGenerator.Generate(ArkadeConstants.Noark5TestListFileName);
            Directory.CreateDirectory(outputDirectoryPath);

            // Run commands and store results:

            Program.Main(new[]
            {
                "process",
                "-a", archiveDirectoryPath,
                "-t", "noark5",
                "-m", metadataFilePath,
                "-p", testDataDirectoryPath,
                "-o", outputDirectoryPath,
                "-l", noark5TestListPath
            });

            FileSystemInfo[] outputDirectoryItems = new DirectoryInfo(outputDirectoryPath).GetFileSystemInfos();
            bool testReportWasCreated = outputDirectoryItems.Any(item => item.Name.StartsWith("Arkaderapport"));
            bool packageWasCreated = outputDirectoryItems.Any(item => item.Name.StartsWith("Arkadepakke"));

            // Control results:

            testReportWasCreated.Should().BeTrue();
            packageWasCreated.Should().BeTrue();
        }

        private static void ClearAllPaths()
        {
            if (File.Exists(noark5TestListPath))
                File.Delete(noark5TestListPath);

            if (File.Exists(metadataFilePath))
                File.Delete(metadataFilePath);

            if (Directory.Exists(outputDirectoryPath))
                Directory.Delete(outputDirectoryPath, true);
        }

        public void Dispose()
        {
            ArkadeProcessingArea.Destroy();

            ClearAllPaths();
        }
    }
}
