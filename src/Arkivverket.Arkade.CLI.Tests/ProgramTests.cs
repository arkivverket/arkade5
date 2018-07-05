using System.IO;
using System.Linq;
using System.Reflection;
using Arkivverket.Arkade.Core;
using Xunit;
using FluentAssertions;

namespace Arkivverket.Arkade.CLI.Tests
{
    public class ProgramTests
    {
        [Fact]
        [Trait("Category", "Integration")]
        public void TestReportAndPackageIsCreatedRunningN5Archive()
        {
            // Establish/clear needed paths:

            string workingDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testDataDirectoryPath = Path.Combine(workingDirectoryPath, "TestData");
            string metadataFilePath = Path.Combine(testDataDirectoryPath, "metadata.txt");
            string archiveDirectoryPath = Path.Combine(testDataDirectoryPath, "N5-archive");
            string outputDirectoryPath = Path.Combine(testDataDirectoryPath, "output");

            // Clear needed paths:

            File.Delete(metadataFilePath);

            if (Directory.Exists(outputDirectoryPath))
                Directory.Delete(outputDirectoryPath, true);
            Directory.CreateDirectory(outputDirectoryPath);

            // Run commands and store results:

            Program.Main(new[]
            {
                "-g", metadataFilePath,
                "-p", testDataDirectoryPath
            });

            bool metadataWasGenerated = File.Exists(metadataFilePath);

            Program.Main(new[]
            {
                "-a", archiveDirectoryPath,
                "-t", "noark5",
                "-m", metadataFilePath,
                "-p", testDataDirectoryPath,
                "-o", outputDirectoryPath
            });

            FileSystemInfo[] outputDirectoryItems = new DirectoryInfo(outputDirectoryPath).GetFileSystemInfos();
            bool testReportWasCreated = outputDirectoryItems.Any(item => item.Name.StartsWith("Arkaderapport"));
            bool packageWasCreated = outputDirectoryItems.Any(item => item.Name.StartsWith("Arkadepakke"));

            // Clean up:

            File.Delete(metadataFilePath);
            Directory.Delete(outputDirectoryPath, true);
            ArkadeProcessingArea.Destroy();

            // Control results:

            metadataWasGenerated.Should().BeTrue();
            testReportWasCreated.Should().BeTrue();
            packageWasCreated.Should().BeTrue();
        }
    }
}
