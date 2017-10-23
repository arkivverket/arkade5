using System;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core
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

        [Fact]
        public void ProcessingAreaIsEstablished()
        {
            ArkadeProcessingArea.Establish(_locationPath);

            ArkadeProcessingArea.Location.FullName.Should().Be(_locationPath);
            ArkadeProcessingArea.RootDirectory.FullName.Should().Be(_locationPath + "\\Arkade");
            ArkadeProcessingArea.WorkDirectory.FullName.Should().Be(_locationPath + "\\Arkade\\work");
            ArkadeProcessingArea.LogsDirectory.FullName.Should().Be(_locationPath + "\\Arkade\\logs");
        }

        [Fact]
        public void ProcessingAreaIsEstablishedWithInvalidLocation()
        {
            string nonExistingLocation = Path.Combine(Environment.CurrentDirectory, "TestData", "NonExistingDirectory");

            ArkadeProcessingArea.Establish(nonExistingLocation);

            ProcessingAreaIsSetupWithTemporaryLogsDirectoryOnly();
        }

        [Fact]
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

        public void Dispose()
        {
            _location.Delete(true);
        }
    }
}
