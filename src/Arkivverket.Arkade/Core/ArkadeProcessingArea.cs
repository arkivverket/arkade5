using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Arkivverket.Arkade.Util;
using Serilog;

namespace Arkivverket.Arkade.Core
{
    public static class ArkadeProcessingArea
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public static DirectoryInfo Location;

        public static DirectoryInfo RootDirectory;
        public static DirectoryInfo WorkDirectory;
        public static DirectoryInfo LogsDirectory;

        /// <summary>
        /// Establish processing directory for Arkade. 
        /// Throws IOException on non-existing paths. 
        /// </summary>
        /// <param name="locationPath">Path to root processing area - must exist!</param>
        public static void Establish(string locationPath)
        {
            try
            {
                SetupLocation(locationPath);
                SetupDirectories();
            }
            finally
            {
                SetupTemporaryLogsDirectory();
            }
        }

        public static void CleanUp()
        {
            WorkDirectory?.Delete(true);

            DeleteOldLogs();
        }

        public static void Destroy()
        {
            Serilog.Log.CloseAndFlush();

            RootDirectory?.Delete(true);

            RootDirectory?.Refresh();
        }

        private static void SetupLocation(string locationPath)
        {
            var location = new DirectoryInfo(locationPath);

            if (!location.Exists)
                throw new IOException("Non existing path: " + locationPath);

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

            // Deletes any temporary logs:
            if (Directory.Exists(GetTemporaryLogsDirectoryPath()))
                Directory.Delete(GetTemporaryLogsDirectoryPath(), true);
        }

        private static void SetupTemporaryLogsDirectory()
        {
            string directoryPath = GetTemporaryLogsDirectoryPath();
            const string logMessage = "Temporary system logs directory created: ";

            LogsDirectory = CreateDirectory(directoryPath, logMessage);
        }

        private static DirectoryInfo CreateDirectory(string directoryPath, string customLogMessage = null)
        {
            var directory = new DirectoryInfo(directoryPath);

            directory.Create();

            const string defaultLogMessage = "Arkade processing area directory created: ";
            
            Log.Information((customLogMessage ?? defaultLogMessage) + directory.FullName);

            return directory;
        }

        private static void DeleteOldLogs()
        {
            foreach (FileInfo logFile in LogsDirectory.GetFiles())
                if (IsOldLog(logFile))
                    logFile.Delete();
        }

        private static bool IsOldLog(FileSystemInfo logFile)
        {
            // Extracts date from either arkade-20171024.log or arkade-error-20171024091500.log
            const string dateCaptureRegexPattern = @"^arkade(-error)?-(?<date>\d{8})(\d{6})?\.log$";

            string dateString = Regex.Match(logFile.Name, dateCaptureRegexPattern).Groups["date"].Value;

            DateTime logDate = DateTime.ParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture);

            return logDate.AddDays(7) < DateTime.Now; // The log is more than 7 days old
        }

        private static string GetTemporaryLogsDirectoryPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ArkadeConstants.DirectoryNameTemporaryLogsLocation
            );
        }
    }
}
