using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using Xunit;
using FluentAssertions;

namespace Arkivverket.Arkade.CLI.Tests
{
    public class ProgramTests : IDisposable
    {
        // Setup language
        private const SupportedLanguage Language = SupportedLanguage.en;

        // Establish needed paths:
        private static readonly string workingDirectoryPath;
        private static readonly string metadataFilePath;
        private static readonly string noark5TestSelectionFilePath;
        private static readonly string testDataDirectoryPath;
        private static readonly string archiveDirectoryPath;
        private static readonly string outputDirectoryPath;

        static ProgramTests()
        {
            OutputFileNames.Culture = new CultureInfo(Language.ToString());

            workingDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            metadataFilePath = Path.Combine(workingDirectoryPath, OutputFileNames.MetadataFile);
            noark5TestSelectionFilePath = Path.Combine(workingDirectoryPath, OutputFileNames.Noark5TestSelectionFile);
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
                "-s",
                "-o", workingDirectoryPath
            });

            bool metadataWasGenerated = File.Exists(metadataFilePath);
            bool noark5TestSelectionFileGenerated = File.Exists(noark5TestSelectionFilePath);

            // Control result:

            metadataWasGenerated.Should().BeTrue();
            noark5TestSelectionFileGenerated.Should().BeTrue();
        }

        [Fact(Skip = "IO-issues")]
        [Trait("Category", "Integration")]
        public void TestCommandTest()
        {
            // Prepare needed files and/or directories

            Noark5TestSelectionFileGenerator.Generate(OutputFileNames.Noark5TestSelectionFile, Language);
            Directory.CreateDirectory(outputDirectoryPath);

            // Run commands and store results:

            Program.Main(new[]
            {
                "test",
                "-a", archiveDirectoryPath,
                "-t", "noark5",
                "-p", testDataDirectoryPath,
                "-o", outputDirectoryPath,
                "-s", noark5TestSelectionFilePath
            });

            FileSystemInfo[] outputDirectoryItems = new DirectoryInfo(outputDirectoryPath).GetFileSystemInfos();
            bool testReportWasCreated = outputDirectoryItems.Any(item => item.Name.StartsWith("Arkade-report"));

            // Control result:

            testReportWasCreated.Should().BeTrue();
        }

        [Fact(Skip = "IO-issues")]
        [Trait("Category", "Integration")]
        public void PackCommandTest()
        {
            // Prepare needed files and/or directories

            new MetadataExampleGenerator().Generate(OutputFileNames.MetadataFile);
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
            bool packageWasCreated = outputDirectoryItems.Any(item =>
                item.Name.StartsWith(OutputFileNames.ResultOutputDirectory));

            // Control result:

            packageWasCreated.Should().BeTrue();
        }

        [Fact(Skip = "IO-issues")]
        [Trait("Category", "Integration")]
        public void ProcessCommandTest()
        {
            // Prepare needed files and/or directories

            new MetadataExampleGenerator().Generate(OutputFileNames.MetadataFile);
            Noark5TestSelectionFileGenerator.Generate(OutputFileNames.Noark5TestSelectionFile, Language);
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
                "-s", noark5TestSelectionFilePath,
                "-l", "nb"
            });

            FileSystemInfo[] outputDirectoryItems = new DirectoryInfo(outputDirectoryPath).GetFileSystemInfos();
            bool testReportWasCreated = outputDirectoryItems.Any(item => item.Name.StartsWith("Arkade-rapport"));
            bool packageWasCreated = outputDirectoryItems.Any(item =>
                item.Name.StartsWith(OutputFileNames.ResultOutputDirectory));

            // Control results:

            testReportWasCreated.Should().BeTrue();
            packageWasCreated.Should().BeTrue();
        }

        private static void ClearAllPaths()
        {
            if (File.Exists(noark5TestSelectionFilePath))
                File.Delete(noark5TestSelectionFilePath);

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
