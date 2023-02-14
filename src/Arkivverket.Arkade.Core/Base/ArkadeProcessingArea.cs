using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Arkivverket.Arkade.Core.Util;
using Serilog;

namespace Arkivverket.Arkade.Core.Base
{
    public static class ArkadeProcessingArea
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public static DirectoryInfo Location;

        public static DirectoryInfo RootDirectory;
        public static DirectoryInfo WorkDirectory;
        public static DirectoryInfo LogsDirectory;

        /// <summary>
        /// Establish processing area for Arkade in the given location.
        /// Throws ArgumentException on invalid location.
        /// </summary>
        /// <param name="locationPath">Path to an existing and accessible location
        /// for the processing area (root directory) to be created.</param>
        public static void Establish(string locationPath)
        {
            try
            {
                SetupLocation(locationPath);
                SetupDirectories();
            }
            catch(Exception e)
            {
                SetupTemporaryLogsDirectory();
                throw new ArgumentException("Unable to establish processing area in: " + locationPath, e);
            }
        }

        /// <summary>
        /// Clean up temporary files generated from an Arkade session
        /// </summary>
        /// <returns><c>false</c> if any temporary files could not be deleted<br/><c>true</c> otherwise</returns>
        public static bool CleanUp()
        {
            DeleteOldLogs();

            return TryDeleteWorkDirectory();
        }

        public static void Destroy()
        {
            Serilog.Log.CloseAndFlush();

            RootDirectory?.Delete(true);

            RootDirectory?.Refresh();
        }

        public static void SetupTemporaryLogsDirectory()
        {
            string directoryPath = GetTemporaryLogsDirectoryPath();
            const string logMessage = "Temporary system logs directory created: ";

            LogsDirectory = CreateDirectory(directoryPath, logMessage);
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

        private static DirectoryInfo CreateDirectory(string directoryPath, string customLogMessage = null)
        {
            var directory = new DirectoryInfo(directoryPath);

            directory.Create();

            const string defaultLogMessage = "Arkade processing area directory created: ";
            
            Log.Information((customLogMessage ?? defaultLogMessage) + directory.FullName);

            return directory;
        }

        private static bool TryDeleteWorkDirectory()
        {
            try
            {
                if (WorkDirectory != null && WorkDirectory.Exists)
                    WorkDirectory.Delete(true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void DeleteOldLogs()
        {
            foreach (FileInfo logFile in LogsDirectory.GetFiles())
                if (IsOldLog(logFile))
                    logFile.Delete();
        }

        private static bool IsOldLog(FileSystemInfo logFile)
        {
            // Extracts date from filename looking like either arkade-20171024.log, arkade-error-20171024091500.log
            // arkade-20171024_001.log or arkade-error-20171024091500_001.log
            const string dateCaptureRegexPattern = @"^arkade(-error)?-(?<date>\d{8})(\d{6})?(_\d{3})?\.log$";

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
