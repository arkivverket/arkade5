using System;
using System.IO;
using System.Text;
using System.Windows;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Util;
using Serilog;

namespace Arkivverket.Arkade.UI.Util
{
    public class DetailedExceptionMessage
    {
        private readonly string _errorMessage;
        private static readonly ILogger Log = Serilog.Log.ForContext<DetailedExceptionMessage>();

        public DetailedExceptionMessage(Exception exception)
        {
            _errorMessage = Build(exception);

            Log.Error(_errorMessage);
        }

        public void ShowMessageBox()
        {
            string messageBoxErrorMessage = new StringBuilder()
                .AppendLine("Use Ctrl+C to copy this error message and send it to arkade5@arkivverket.no")
                .AppendLine("")
                .Append(_errorMessage)
                .ToString();

            MessageBox.Show(messageBoxErrorMessage, "Unexpected error!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public string WriteToFile()
        {
            string timeStamp = DateTime.Now.ToString("ddMMyyHHmmss");

            string fileName = Path.Combine(
                ArkadeProcessingArea.GetLogsDirectory().FullName,
                string.Format(Resources.UI.DetailedErrorMessageFileName, timeStamp)
            );

            try
            {
                File.WriteAllText(fileName, _errorMessage);

                return fileName;
            }
            catch (Exception)
            {
                ShowMessageBox();

                return string.Empty;
            }
        }

        private static string Build(Exception e)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("System information:");
            sb.AppendLine("Arkade version: " + ArkadeVersion.Version);
            sb.AppendLine("OS: " + SystemInfo.GetOsName());
            sb.AppendLine("OS Version: " + SystemInfo.GetOsVersion());
            sb.AppendLine(".NET Framework: " + SystemInfo.GetDotNetFrameworkVersion());
            sb.AppendLine(".NET CLR: " + SystemInfo.GetDotNetClrVersion());

            ulong totalPhysicalMemoryInBytes = SystemInfo.GetTotalPhysicalMemoryInBytes();
            sb.AppendLine("Total physical memory: " + totalPhysicalMemoryInBytes + " (" +
                          (totalPhysicalMemoryInBytes / 1024 / 1024) + " MB)");
            ulong availablePhysicalMemoryInBytes = SystemInfo.GetAvailablePhysicalMemoryInBytes();
            sb.AppendLine("Available physical memory: " + availablePhysicalMemoryInBytes + " (" +
                          (availablePhysicalMemoryInBytes / 1024 / 1024) + " MB)");

            ulong totalVirtualMemoryInBytes = SystemInfo.GetTotalVirtualMemoryInBytes();
            sb.AppendLine("Total virtual memory: " + totalVirtualMemoryInBytes + " (" +
                          (totalVirtualMemoryInBytes / 1024 / 1024) + " MB)");
            ulong availableVirtualMemoryInBytes = SystemInfo.GetAvailableVirtualMemoryInBytes();
            sb.AppendLine("Available virtual memory: " + availableVirtualMemoryInBytes + " (" +
                          (availableVirtualMemoryInBytes / 1024 / 1024) + " MB)");

            long totalSpaceInBytes = SystemInfo.GetTotalDiskSpaceInBytes();
            sb.AppendLine("Total disk space: " + totalSpaceInBytes + " (" + (totalSpaceInBytes / 1024 / 1024) + " MB)");
            long freeDiskSpaceInBytes = SystemInfo.GetAvailableDiskSpaceInBytes();
            sb.AppendLine("Available disk space: " + freeDiskSpaceInBytes + " (" + (freeDiskSpaceInBytes / 1024 / 1024) + " MB)");

            sb.AppendLine("");
            sb.AppendLine("Stack trace:");
            sb.AppendLine(e.ToString());

            string errorMessage = sb.ToString();
            return errorMessage;
        }
    }
}
