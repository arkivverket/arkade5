using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Util
{
    public static class SystemInfo
    {
        public static long GetAvailableDiskSpaceInBytes()
        {
            return GetAvailableDiskSpaceInBytes(ArkadeProcessingArea.RootDirectory.FullName);
        }

        public static long GetTotalDiskSpaceInBytes()
        {
            return GetTotalDiskSpaceInBytes(ArkadeProcessingArea.RootDirectory.FullName);
        }

        public static long GetAvailableDiskSpaceInBytes(string directory)
        {
            return GetDirectoryDriveInfo(directory).AvailableFreeSpace;
        }

        public static long GetTotalDiskSpaceInBytes(string directory)
        {
            return GetDirectoryDriveInfo(directory).TotalSize;
        }

        public static string GetDotNetClrVersion()
        {
            return Environment.Version.ToString();
        }

        private static DriveInfo GetDirectoryDriveInfo(string directory)
        {
            string fullyQualifiedPath = Path.GetFullPath(directory);

            // TODO: Use below line when on .NET Standard 2.1 (reducing IO)
            //string fullyQualifiedPath = Path.IsPathFullyQualified(directory) ? directory : Path.GetFullPath(directory);

            string directoryPathRoot = Path.GetPathRoot(fullyQualifiedPath);

            return new DriveInfo(directoryPathRoot);
        }
    }
}
