using System;
using System.Text;
using System.Windows;
using Arkivverket.Arkade.Util;
using Serilog;

namespace Arkivverket.Arkade.UI.Util
{
    public class ExceptionMessageBox
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<ExceptionMessageBox>();

        public static void Show(Exception e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Use Ctrl+C to copy this error message and send it to arkade5@arkivverket.no");
            sb.AppendLine("");
            sb.AppendLine("System information:");
            sb.AppendLine("Arkade version: " + ArkadeVersion.Version);
            sb.AppendLine("OS: " + SystemInfo.GetOsName());
            sb.AppendLine("OS Version: " + SystemInfo.GetOsVersion());
            sb.AppendLine(".NET Framework: " + SystemInfo.GetDotNetFrameworkVersion());
            sb.AppendLine(".NET CLR: " + SystemInfo.GetDotNetClrVersion());

            ulong totalPhysicalMemoryInBytes = SystemInfo.GetTotalPhysicalMemoryInBytes();
            sb.AppendLine("Total physical memory: " + totalPhysicalMemoryInBytes + " (" + (totalPhysicalMemoryInBytes / 1024/1024) + " MB)");
            ulong availablePhysicalMemoryInBytes = SystemInfo.GetAvailablePhysicalMemoryInBytes();
            sb.AppendLine("Available physical memory: " + availablePhysicalMemoryInBytes + " (" + (availablePhysicalMemoryInBytes / 1024 / 1024) + " MB)");

            ulong totalVirtualMemoryInBytes = SystemInfo.GetTotalVirtualMemoryInBytes();
            sb.AppendLine("Total virtual memory: " + totalVirtualMemoryInBytes + " (" + (totalVirtualMemoryInBytes / 1024 / 1024) + " MB)");
            ulong availableVirtualMemoryInBytes = SystemInfo.GetAvailableVirtualMemoryInBytes();
            sb.AppendLine("Available virtual memory: " + availableVirtualMemoryInBytes + " (" + (availableVirtualMemoryInBytes / 1024 / 1024) + " MB)");

            long totalSpaceInBytes = SystemInfo.GetTotalDiskSpaceInBytes();
            sb.AppendLine("Total disk space: " + totalSpaceInBytes + " (" + (totalSpaceInBytes / 1024 / 1024) + " MB)");
            long freeDiskSpaceInBytes = SystemInfo.GetAvailableDiskSpaceInBytes();
            sb.AppendLine("Available disk space: " + freeDiskSpaceInBytes + " (" + (freeDiskSpaceInBytes/1024/1024) + " MB)");

            sb.AppendLine("");
            sb.AppendLine("Stack trace:");
            sb.AppendLine(e.ToString());
            string errorMessage = sb.ToString();

            Log.Error(errorMessage);

            MessageBox.Show(errorMessage, "Unexpected error!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}