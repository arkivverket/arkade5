using System;
using System.IO;
using Arkivverket.Arkade.Core;
using Microsoft.Win32;

namespace Arkivverket.Arkade.Util
{
    public class SystemInfo
    {
        public static ulong GetTotalPhysicalMemoryInBytes()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
        }

        public static ulong GetTotalVirtualMemoryInBytes()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().TotalVirtualMemory;
        }

        public static ulong GetAvailablePhysicalMemoryInBytes()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory;
        }

        public static ulong GetAvailableVirtualMemoryInBytes()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().AvailableVirtualMemory;
        }

        public static string GetOsName()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().OSFullName;
        }

        public static string GetOsPlatform()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().OSPlatform;
        }

        public static string GetOsVersion()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().OSVersion;
        }

        public static long GetAvailableDiskSpaceInBytes()
        {
            DirectoryInfo arkadeDirectory = ArkadeConstants.GetArkadeDirectory();
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
            DirectoryInfo arkadeDirectory = ArkadeConstants.GetArkadeDirectory();
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

        public static int GetDotNetFrameworkVersion()
        {
            using (
                RegistryKey ndpKey =
                    RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
                        .OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                object value = ndpKey.GetValue("Release");
                return value == null ? -1 : Convert.ToInt32(value);
            }
        }

        public static string GetDotNetClrVersion()
        {
            return Environment.Version.ToString();
        }
    }
}