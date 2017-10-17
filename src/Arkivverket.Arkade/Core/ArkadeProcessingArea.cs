using System;
using System.IO;
using Arkivverket.Arkade.Properties;
using Arkivverket.Arkade.Util;
using Serilog;

namespace Arkivverket.Arkade.Core
{
    public static class ArkadeProcessingArea
    {
        public static readonly DirectoryInfo Location;

        public static DirectoryInfo RootDirectory;
        public static DirectoryInfo WorkDirectory;
        public static DirectoryInfo LogsDirectory;

        static ArkadeProcessingArea()
        {
            string locationSetting = GetLocationSetting();

            if (String.IsNullOrEmpty(locationSetting))
                SetupTemporaryLogsDirectory();
            else
            {
                Location = new DirectoryInfo(locationSetting);
                if (Location.Exists)
                    SetupDirectories();
            }
        }

        public static string GetLocationSetting()
        {
            return Settings.Default.ArkadeProcessingAreaLocation;
        }

        public static void SetLocationSetting(string location)
        {
            Settings.Default.ArkadeProcessingAreaLocation = location;
            Settings.Default.Save();
        }

        public static bool HasValidLocation()
        {
            return Location != null && Location.Exists;
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
