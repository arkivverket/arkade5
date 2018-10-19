using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Util
{
    public class SystemInfo
    {
        public static long GetAvailableDiskSpaceInBytes()
        {
            DirectoryInfo arkadeDirectory = ArkadeProcessingArea.RootDirectory;
            string drive = Path.GetPathRoot(arkadeDirectory.FullName);

            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.Name == drive)
                {
                    return d.AvailableFreeSpace;
                }
            }
            return -1;
        }

        public static long GetTotalDiskSpaceInBytes()
        {
            DirectoryInfo arkadeDirectory = ArkadeProcessingArea.RootDirectory;
            string drive = Path.GetPathRoot(arkadeDirectory.FullName);

            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.Name == drive)
                {
                    return d.TotalSize;
                }
            }
            return -1;
        }

        public static string GetDotNetClrVersion()
        {
            return Environment.Version.ToString();
        }
    }
}