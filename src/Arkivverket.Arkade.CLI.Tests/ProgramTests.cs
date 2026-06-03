using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.Core.Resources;
using Xunit;
using FluentAssertions;

namespace Arkivverket.Arkade.CLI.Tests
{
    public class ProgramTests : IDisposable
    {
        // Output files and directories are named after the chosen language; we pin it to get
        // deterministic names that match the OutputFileNames resource constants below.
        private const SupportedLanguage Language = SupportedLanguage.en;

        private readonly string _testRootDirectory;
        private readonly string _processingAreaPath;
        private readonly string _outputDirectoryPath;
        private readonly string _metadataFilePath;
        private readonly string _archiveDirectoryPath;

        public ProgramTests()
        {
            OutputFileNames.Culture = new CultureInfo(Language.ToString());

            _archiveDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "N5-archive");

            // Each test gets its own isolated working area to avoid cross-test interference
            // through the shared ArkadeProcessingArea state and the Serilog log file.
            _testRootDirectory = Path.Combine(Path.GetTempPath(), "arkade-cli-tests", Guid.NewGuid().ToString("N"));
            _processingAreaPath = Path.Combine(_testRootDirectory, "processing");
            _outputDirectoryPath = Path.Combine(_testRootDirectory, "output");
            _metadataFilePath = Path.Combine(_testRootDirectory, OutputFileNames.MetadataExampleFile);

            Directory.CreateDirectory(_processingAreaPath);
            Directory.CreateDirectory(_outputDirectoryPath);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void GenerateCommandTest()
        {
            Program.Main(new[]
            {
                "generate",
                "-m",
                "-s",
                "-o", _outputDirectoryPath,
                "-l", Language.ToString()
            });

            File.Exists(Path.Combine(_outputDirectoryPath, OutputFileNames.MetadataExampleFile)).Should().BeTrue();
            File.Exists(Path.Combine(_outputDirectoryPath, OutputFileNames.Noark5TestSelectionFile)).Should().BeTrue();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void TestCommandTest()
        {
            // Exercises the -s path: the selection file is read into TestSession.TestsToRun.
            string testSelectionFile = CreateTestSelectionFile();

            Program.Main(new[]
            {
                "test",
                "-a", _archiveDirectoryPath,
                "-t", "noark5",
                "-p", _processingAreaPath,
                "-o", _outputDirectoryPath,
                "-s", testSelectionFile,
                "-l", Language.ToString()
            });

            // The 'test' verb writes a stand-alone test report directory directly to the output directory.
            DirectoryInfo reportDirectory = StandaloneTestReportDirectoryIn(_outputDirectoryPath);

            reportDirectory.Should().NotBeNull("the test verb should produce a stand-alone test report directory");
            reportDirectory.GetFiles("*.html").Should().NotBeEmpty();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void PackCommandTest()
        {
            new MetadataExampleGenerator().Generate(_metadataFilePath);

            Program.Main(new[]
            {
                "pack",
                "-a", _archiveDirectoryPath,
                "-t", "noark5",
                "-m", _metadataFilePath,
                "-p", _processingAreaPath,
                "-o", _outputDirectoryPath,
                "-l", Language.ToString()
            });

            DirectoryInfo resultDirectory = ResultDirectoryIn(_outputDirectoryPath);

            resultDirectory.Should().NotBeNull("the pack verb should produce a result directory containing the package");
            resultDirectory.GetFiles("*.tar").Should().NotBeEmpty();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void ProcessCommandTest()
        {
            new MetadataExampleGenerator().Generate(_metadataFilePath);

            // No -s here: exercises the default path where all tests are run (Noark5TestProvider.GetAllTestIds).
            Program.Main(new[]
            {
                "process",
                "-a", _archiveDirectoryPath,
                "-t", "noark5",
                "-m", _metadataFilePath,
                "-p", _processingAreaPath,
                "-o", _outputDirectoryPath,
                "-l", Language.ToString()
            });

            DirectoryInfo resultDirectory = ResultDirectoryIn(_outputDirectoryPath);

            resultDirectory.Should().NotBeNull("the process verb should produce a result directory");
            resultDirectory.GetFiles("*.tar").Should().NotBeEmpty("process should create the information package");

            // For 'process' the test report is nested inside the result directory (not stand-alone).
            DirectoryInfo reportDirectory = StandaloneTestReportDirectoryIn(resultDirectory.FullName);

            reportDirectory.Should().NotBeNull("process should nest the test report inside the result directory");
            reportDirectory.GetFiles("*.html").Should().NotBeEmpty();
        }

        // Writes a Noark5 test-selection file with every test enabled, so the -s argument and the
        // selection-file reader (Noark5TestSelectionFileReader) are exercised end to end.
        private string CreateTestSelectionFile()
        {
            string path = Path.Combine(_testRootDirectory, OutputFileNames.Noark5TestSelectionFile);

            Noark5TestSelectionFileGenerator.Generate(path, Language, allTestsEnabled: true);

            return path;
        }

        private static DirectoryInfo ResultDirectoryIn(string parentDirectory)
            => SingleDirectoryStartingWith(parentDirectory, OutputFileNames.ResultOutputDirectory);

        private static DirectoryInfo StandaloneTestReportDirectoryIn(string parentDirectory)
            => SingleDirectoryStartingWith(parentDirectory, OutputFileNames.StandaloneTestReportDirectory);

        // The directory names embed an id/timestamp via a "{0}" placeholder; match on the literal prefix.
        private static DirectoryInfo SingleDirectoryStartingWith(string parentDirectory, string nameFormat)
        {
            string prefix = nameFormat.Replace("{0}", string.Empty);

            return new DirectoryInfo(parentDirectory)
                .GetDirectories()
                .SingleOrDefault(directory => directory.Name.StartsWith(prefix));
        }

        public void Dispose()
        {
            // Releases the Serilog log file handle (via CloseAndFlush) so the working area can be removed.
            ArkadeProcessingArea.Destroy();

            try
            {
                if (Directory.Exists(_testRootDirectory))
                    Directory.Delete(_testRootDirectory, recursive: true);
            }
            catch (IOException)
            {
                // Best-effort cleanup: on some platforms a lingering handle (e.g. from an external
                // format-identification process) can briefly keep a file locked. The temp files are
                // harmless and will be cleared by the OS.
            }
        }
    }
}
