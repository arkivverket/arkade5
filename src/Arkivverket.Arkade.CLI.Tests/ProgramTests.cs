using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Arkivverket.Arkade.CLI.Tests.UnitTestUtilities;
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

        private readonly TestSessionLifeTimeFilesFixture _filesFixture;
        private readonly string _archiveDirectoryPath;

        public ProgramTests(TestSessionLifeTimeFilesFixture filesFixture)
        {
            _filesFixture = filesFixture;

            OutputFileNames.Culture = new CultureInfo(Language.ToString());

            _archiveDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "N5-archive");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void GenerateCommandTest()
        {
            TestPaths paths = CreateTestPaths();

            Program.Main(new[]
            {
                "generate",
                "-m",
                "-s",
                "-o", paths.OutputDirectory,
                "-l", Language.ToString()
            });

            File.Exists(Path.Combine(paths.OutputDirectory, OutputFileNames.MetadataExampleFile)).Should().BeTrue();
            File.Exists(Path.Combine(paths.OutputDirectory, OutputFileNames.Noark5TestSelectionFile)).Should().BeTrue();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void TestCommandTest()
        {
            TestPaths paths = CreateTestPaths();

            // Exercises the -s path: the selection file is read into TestSession.TestsToRun.
            string testSelectionFile = CreateTestSelectionFile(paths.Root);

            Program.Main(new[]
            {
                "test",
                "-a", _archiveDirectoryPath,
                "-t", "noark5",
                "-p", paths.ProcessingArea,
                "-o", paths.OutputDirectory,
                "-s", testSelectionFile,
                "-l", Language.ToString()
            });

            // The 'test' verb writes a stand-alone test report directory directly to the output directory.
            DirectoryInfo reportDirectory = StandaloneTestReportDirectoryIn(paths.OutputDirectory);

            reportDirectory.Should().NotBeNull("the test verb should produce a stand-alone test report directory");
            reportDirectory.GetFiles("*.html").Should().NotBeEmpty();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void PackCommandTest()
        {
            TestPaths paths = CreateTestPaths();

            new MetadataExampleGenerator().Generate(paths.MetadataFile);

            Program.Main(new[]
            {
                "pack",
                "-a", _archiveDirectoryPath,
                "-t", "noark5",
                "-m", paths.MetadataFile,
                "-p", paths.ProcessingArea,
                "-o", paths.OutputDirectory,
                "-l", Language.ToString()
            });

            DirectoryInfo resultDirectory = ResultDirectoryIn(paths.OutputDirectory);

            resultDirectory.Should().NotBeNull("the pack verb should produce a result directory containing the package");
            resultDirectory.GetFiles("*.tar").Should().NotBeEmpty();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void ProcessCommandTest()
        {
            TestPaths paths = CreateTestPaths();

            new MetadataExampleGenerator().Generate(paths.MetadataFile);

            // No -s here: exercises the default path where all tests are run (Noark5TestProvider.GetAllTestIds).
            Program.Main(new[]
            {
                "process",
                "-a", _archiveDirectoryPath,
                "-t", "noark5",
                "-m", paths.MetadataFile,
                "-p", paths.ProcessingArea,
                "-o", paths.OutputDirectory,
                "-l", Language.ToString()
            });

            DirectoryInfo resultDirectory = ResultDirectoryIn(paths.OutputDirectory);

            resultDirectory.Should().NotBeNull("the process verb should produce a result directory");
            resultDirectory.GetFiles("*.tar").Should().NotBeEmpty("process should create the information package");

            // For 'process' the test report is nested inside the result directory (not stand-alone).
            DirectoryInfo reportDirectory = StandaloneTestReportDirectoryIn(resultDirectory.FullName);

            reportDirectory.Should().NotBeNull("process should nest the test report inside the result directory");
            reportDirectory.GetFiles("*.html").Should().NotBeEmpty();
        }

        // Claims an isolated directory for the calling test (named after the test method) and
        // prepares the input/output sub-directories the CLI verbs expect to already exist.
        private TestPaths CreateTestPaths([CallerMemberName] string testName = null)
        {
            DirectoryInfo root = _filesFixture.CreateIsolatedDirectory<ProgramTests>(testName);

            string processingArea = root.CreateSubdirectory("processing").FullName;
            string outputDirectory = root.CreateSubdirectory("output").FullName;

            return new TestPaths(root.FullName, processingArea, outputDirectory);
        }

        // Writes a Noark5 test-selection file with every test enabled, so the -s argument and the
        // selection-file reader (Noark5TestSelectionFileReader) are exercised end to end.
        private static string CreateTestSelectionFile(string directory)
        {
            string path = Path.Combine(directory, OutputFileNames.Noark5TestSelectionFile);

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
            // Releases the Serilog log file handle (via CloseAndFlush) so the isolated test directory
            // can be removed when the assembly's test session ends. The directory itself is cleaned
            // up by TestSessionLifeTimeFilesFixture.
            ArkadeProcessingArea.Destroy();
        }

        private sealed record TestPaths(string Root, string ProcessingArea, string OutputDirectory)
        {
            public string MetadataFile => Path.Combine(Root, OutputFileNames.MetadataExampleFile);
        }
    }
}
