using System;
using System.IO;
using System.Linq;
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
            string fullyQualifiedDirectoryPath = // TODO: Uncomment below line when on .NET Standard 2.1 (reducing IO)
                /*Path.IsPathFullyQualified(directory) ? directory :*/ Path.GetFullPath(directory);
                
            DriveInfo directoryDrive = DriveInfo.GetDrives()
                .OrderByDescending(drive => drive.Name.Length)
                .First(drive => fullyQualifiedDirectoryPath.StartsWith(drive.Name));

            return directoryDrive;
        }
    }
}
