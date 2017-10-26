using System;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core
{
    public class ArkadeProcessingAreaTest
    {
        private readonly string _locationPath;
        private readonly DirectoryInfo _location;

        public ArkadeProcessingAreaTest()
        {
            _locationPath = Path.Combine(Environment.CurrentDirectory, "TestData", "ProcessingAreaTests");
            _location = new DirectoryInfo(_locationPath);
            _location.Create();
        }

        [Fact]
        public void ProcessingAreaIsEstablished()
        {
            ArkadeProcessingArea.Establish(_locationPath);

            ArkadeProcessingArea.Location.FullName.Should().Be(_locationPath);
            ArkadeProcessingArea.RootDirectory.FullName.Should().Be(_locationPath + "\\Arkade");
            ArkadeProcessingArea.WorkDirectory.FullName.Should().Be(_locationPath + "\\Arkade\\work");
            ArkadeProcessingArea.LogsDirectory.FullName.Should().Be(_locationPath + "\\Arkade\\logs");
        }

        [Fact (Skip = "Failing on buildserver ...")]
        public void ProcessingAreaIsCleanedUp()
        {
            ArkadeProcessingArea.Establish(_locationPath);

            // CREATE SOME WORK-FILES:

            // Create a file in the work-directory:
            new FileInfo(Path.Combine(ArkadeProcessingArea.WorkDirectory.FullName, "someFile.txt")).Create().Close();
            // Create a folder in the work-directory:
            DirectoryInfo workDirectorySubDirectory = ArkadeProcessingArea.WorkDirectory.CreateSubdirectory("someFolder");
            // Create a file in the sub-directory of the work-directory:
            new FileInfo(Path.Combine(workDirectorySubDirectory.FullName, "someOtherFile.txt")).Create().Close();
            // Confirm that the test-files have been created:
            ArkadeProcessingArea.WorkDirectory.GetFiles("*", SearchOption.AllDirectories).Length.Should().Be(2);

            // CREATE SOME LOG-FILES:

            DateTime nowDate = DateTime.Now;
            DateTime oldDate = DateTime.Now.AddDays(-7); // Date one week ago

            string fileNameNewLog = $"arkade-{nowDate.Year}{nowDate.Month}{nowDate.Day}.log";
            string fileNameOldLog = $"arkade-{oldDate.Year}{oldDate.Month}{oldDate.Day}.log";
            string fileNameNewErrorLog = $"arkade-error-{nowDate.Year}{nowDate.Month}{nowDate.Day}235959.log";
            string fileNameOldErrorLog = $"arkade-error-{oldDate.Year}{oldDate.Month}{oldDate.Day}235959.log";

            // Create a new log file in the logs-directory:
            new FileInfo(Path.Combine(ArkadeProcessingArea.LogsDirectory.FullName, fileNameNewLog)).Create().Close();
            // Create an old log file in the logs-directory:
            new FileInfo(Path.Combine(ArkadeProcessingArea.LogsDirectory.FullName, fileNameOldLog)).Create().Close();
            // Create a new error log file in the logs-directory:
            new FileInfo(Path.Combine(ArkadeProcessingArea.LogsDirectory.FullName, fileNameNewErrorLog)).Create().Close();
            // Create an old error log file in the logs-directory:
            new FileInfo(Path.Combine(ArkadeProcessingArea.LogsDirectory.FullName, fileNameOldErrorLog)).Create().Close();
            // Confirm that the test-files have been created:
            ArkadeProcessingArea.LogsDirectory.GetFiles().Length.Should().Be(4);
            
            // RUN CLEANUP AND INSPECT:

            ArkadeProcessingArea.CleanUp();

            // Work-directory are removed:
            ArkadeProcessingArea.WorkDirectory.Exists.Should().BeFalse();
            // Old logs are removed, new logs are kept:
            ArkadeProcessingArea.LogsDirectory.GetFiles().Should().Contain(log => log.Name.Equals(fileNameNewLog));
            ArkadeProcessingArea.LogsDirectory.GetFiles().Should().NotContain(log => log.Name.Equals(fileNameOldLog));
            // Old error logs are removed, new error logs are kept:
            ArkadeProcessingArea.LogsDirectory.GetFiles().Should().Contain(log => log.Name.Equals(fileNameNewErrorLog));
            ArkadeProcessingArea.LogsDirectory.GetFiles().Should().NotContain(log => log.Name.Equals(fileNameOldErrorLog));
        }

        [Fact (Skip="Failing on buildserver ...")]
        public void ProcessingAreaIsEstablishedWithInvalidLocation()
        {
            string nonExistingLocation = Path.Combine(Environment.CurrentDirectory, "TestData", "NonExistingDirectory");

            ArkadeProcessingArea.Establish(nonExistingLocation);

            ProcessingAreaIsSetupWithTemporaryLogsDirectoryOnly();
        }

        [Fact (Skip = "Failing on buildserver ...")]
        public void ProcessingAreaIsEstablishedWithMissingLocation()
        {
            ArkadeProcessingArea.Establish("");

            ProcessingAreaIsSetupWithTemporaryLogsDirectoryOnly();
        }

        private static void ProcessingAreaIsSetupWithTemporaryLogsDirectoryOnly()
        {
            string temporaryLogsDirectoryPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ArkadeConstants.DirectoryNameTemporaryLogsLocation
            );

            ArkadeProcessingArea.LogsDirectory.FullName.Should().Be(temporaryLogsDirectoryPath);

            ArkadeProcessingArea.Location.Should().BeNull();
            ArkadeProcessingArea.RootDirectory.Should().BeNull();
            ArkadeProcessingArea.WorkDirectory.Should().BeNull();
        }

        ~ArkadeProcessingAreaTest()
        {
            _location.Delete(true);
        }
    }
}
