using System;
using System.IO;
using Arkivverket.Arkade.Util;
using Serilog;

namespace Arkivverket.Arkade.Core
{
    public static class ArkadeProcessingArea
    {
        public static DirectoryInfo Location;

        public static DirectoryInfo RootDirectory;
        public static DirectoryInfo WorkDirectory;
        public static DirectoryInfo LogsDirectory;

        public static void Establish(string locationPath)
        {
            try
            {
                SetupLocation(locationPath);
                SetupDirectories();
            }
            catch
            {
                SetupTemporaryLogsDirectory();
            }
        }

        public static void CleanUp()
        {
            WorkDirectory?.Delete(true);
        }

        private static void SetupLocation(string locationPath)
        {
            var location = new DirectoryInfo(locationPath);

            if (!location.Exists)
                throw new IOException("Non existing path");

            Location = location;
        }

        private static void SetupDirectories()
        {
            RootDirectory = CreateDirectory(
                Path.Combine(Location.FullName, ArkadeConstants.DirectoryNameArkadeProcessingAreaRoot)
            );

            WorkDirectory = CreateDirectory(
                Path.Combine(RootDirectory.FullName, ArkadeConstants.DirectoryNameArkadeProcessingAreaWork)
            );

            LogsDirectory = CreateDirectory(
                Path.Combine(RootDirectory.FullName, ArkadeConstants.DirectoryNameArkadeProcessingAreaLogs)
            );

            // TODO: Remove any temporary logs
        }

        private static void SetupTemporaryLogsDirectory()
        {
            string temporaryLogsDirectoryPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ArkadeConstants.DirectoryNameTemporaryLogsLocation
            );

            LogsDirectory = new DirectoryInfo(temporaryLogsDirectoryPath);
        }


        private static DirectoryInfo CreateDirectory(string directoryPath)
        {
            var directory = new DirectoryInfo(directoryPath);

            directory.Create();

            Log.Information("Arkade processing area directory created: " + directory.FullName);

            return directory;
        }
    }
}
