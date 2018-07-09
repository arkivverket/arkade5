using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Base
{
    public class ArkadeProcessingAreaTest : IDisposable
    {
        private readonly string _locationPath;
        private readonly DirectoryInfo _location;

        public ArkadeProcessingAreaTest()
        {
            _locationPath = Path.Combine(Environment.CurrentDirectory, "TestData", "ProcessingAreaTests");
            _location = new DirectoryInfo(_locationPath);
            _location.Create();
        }

        [Fact(Skip = "IO-issues")]
        public void ProcessingAreaIsEstablished()
        {
            ArkadeProcessingArea.Establish(_locationPath);

            ArkadeProcessingArea.Location.FullName.Should().Be(_locationPath);
            ArkadeProcessingArea.RootDirectory.FullName.Should().Be(_locationPath + "\\Arkade");
            ArkadeProcessingArea.WorkDirectory.FullName.Should().Be(_locationPath + "\\Arkade\\work");
            ArkadeProcessingArea.LogsDirectory.FullName.Should().Be(_locationPath + "\\Arkade\\logs");
        }

        [Fact(Skip = "IO-issues")]
        public void ProcessingAreaIsEstablishedWithMissingLocation()
        {
            try
            {
                ArkadeProcessingArea.Establish("");
            }
            catch (Exception exception)
            {
                exception.GetType().Should().Be(typeof(ArgumentException));
                exception.Message.Should().Be("Unable to establish processing area in: " + "");
            }

            ProcessingAreaIsSetupWithTemporaryLogsDirectoryOnly().Should().BeTrue();
        }

        [Fact(Skip = "IO-issues")]
        public void ProcessingAreaIsEstablishedWithInvalidLocation()
        {
            string nonExistingLocation = Path.Combine(Environment.CurrentDirectory, "TestData", "NonExistingDirectory");

            try
            {
                ArkadeProcessingArea.Establish(nonExistingLocation);
            }
            catch (Exception exception)
            {
                exception.GetType().Should().Be(typeof(ArgumentException));
                exception.Message.Should().Be("Unable to establish processing area in: " + nonExistingLocation);

                exception.InnerException?.GetType().Should().Be(typeof(IOException));
                exception.InnerException?.Message.Should().Be("Non existing path: " + nonExistingLocation);
            }

            ProcessingAreaIsSetupWithTemporaryLogsDirectoryOnly().Should().BeTrue();
        }

        [Fact(Skip = "IO-issues")]
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

            string nowDate = DateTime.Now.ToString("yyyyMMdd");
            string oldDate = DateTime.Now.AddDays(-7).ToString("yyyyMMdd"); // Date one week ago

            string fileNameNewLog = $"arkade-{nowDate}.log";
            string fileNameOldLog = $"arkade-{oldDate}.log";
            string fileNameNewErrorLog = $"arkade-error-{nowDate}235959.log";
            string fileNameOldErrorLog = $"arkade-error-{oldDate}235959.log";

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

        [Fact(Skip = "IO-issues")]
        public void ProcessingAreaIsDestroyed()
        {
            ArkadeProcessingArea.Establish(_locationPath);
            ArkadeProcessingArea.RootDirectory.Exists.Should().BeTrue();

            ArkadeProcessingArea.Destroy();
            ArkadeProcessingArea.RootDirectory.Exists.Should().BeFalse();
        }

        private static bool ProcessingAreaIsSetupWithTemporaryLogsDirectoryOnly()
        {
            string temporaryLogsDirectoryPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ArkadeConstants.DirectoryNameTemporaryLogsLocation
            );

            return ArkadeProcessingArea.LogsDirectory.FullName.Equals(temporaryLogsDirectoryPath)
                   && ArkadeProcessingArea.Location == null
                   && ArkadeProcessingArea.RootDirectory == null
                   && ArkadeProcessingArea.WorkDirectory == null;
        }

        public void Dispose()
        {
            ArkadeProcessingArea.Location = null;
            ArkadeProcessingArea.RootDirectory = null;
            ArkadeProcessingArea.WorkDirectory = null;
            ArkadeProcessingArea.LogsDirectory = null;

            _location.Delete(true);
        }
    }
}
