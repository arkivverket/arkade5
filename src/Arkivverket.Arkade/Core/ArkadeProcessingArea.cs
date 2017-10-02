using System;
using System.IO;
using Arkivverket.Arkade.Util;
using Serilog;

namespace Arkivverket.Arkade.Core
{
    public static class ArkadeProcessingArea
    {
        private static readonly DirectoryInfo RootDirectory;
        private static readonly DirectoryInfo WorkDirectory;
        private static readonly DirectoryInfo LogsDirectory;

        static ArkadeProcessingArea()
        {
            string rootDirectoryPath = Path.Combine(
                GetRootDirectoryLocationPath(), ArkadeConstants.DirectoryNameArkadeProcessingAreaRoot
            );
            string workDirectoryPath = Path.Combine(
                rootDirectoryPath, ArkadeConstants.DirectoryNameArkadeProcessingAreaWork
            );
            string logsDirectoryPath = Path.Combine(
                rootDirectoryPath, ArkadeConstants.DirectoryNameArkadeProcessingAreaLogs
            );

            RootDirectory = new DirectoryInfo(rootDirectoryPath);
            WorkDirectory = new DirectoryInfo(workDirectoryPath);
            LogsDirectory = new DirectoryInfo(logsDirectoryPath);
        }

        public static DirectoryInfo GetRootDirectory()
        {
            return RootDirectory.Exists ? RootDirectory : CreateArkadeProcessingAreaDirectory(RootDirectory);
        }

        public static DirectoryInfo GetWorkDirectory()
        {
            return WorkDirectory.Exists ? WorkDirectory : CreateArkadeProcessingAreaDirectory(WorkDirectory);
        }

        public static DirectoryInfo GetLogsDirectory()
        {
            return LogsDirectory.Exists ? LogsDirectory : CreateArkadeProcessingAreaDirectory(LogsDirectory);
        }

        private static string GetRootDirectoryLocationPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        private static DirectoryInfo CreateArkadeProcessingAreaDirectory(DirectoryInfo directory)
        {
            directory.Create();
            Log.Information("Arkade processing area directory created: " + LogsDirectory.FullName);
            return directory;
        }
    }
}
