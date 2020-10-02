using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using CsvHelper;
using Serilog;

namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public class SiegfriedFileFormatIdentifier : IFileFormatIdentifier
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

        public IEnumerable<SiegfriedFileInfo> IdentifyFormat(DirectoryInfo directory)
        {
            Process siegfriedProcess = SetupSiegfriedProcess();

            Log.Information($"Starting document file format analysis.");

            IEnumerable<string> siegfriedResult = RunProcessOnDirectory(siegfriedProcess, directory);

            Log.Information($"Document file format analysis completed.");

            return GetSiegfriedFileInfoObjects(siegfriedResult);
        }

        private static Process SetupSiegfriedProcess()
        {
            string executableFileName = GetOSSpecificExecutableFileName();

            string bundleDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bundled");
            string siegfriedDirectory = Path.Combine(bundleDirectory, "Siegfried");
            string siegfriedExecutable = Path.Combine(siegfriedDirectory, executableFileName);
            string argumentsExceptInputDirectory = $"-home \"{siegfriedDirectory}\" -multi 256 -csv -log e,w -coe ";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = siegfriedExecutable,
                    Arguments = argumentsExceptInputDirectory,
                    StandardOutputEncoding = Encoding.UTF8,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            return process;
        }

        private static IEnumerable<string> RunProcessOnDirectory(Process process, FileSystemInfo directory)
        {
            process.StartInfo.Arguments += $"\"{directory.FullName}\"";

            var results = new List<string>();
            var errors = new List<string>();

            process.OutputDataReceived += (sender, args) => results.Add(args.Data);
            process.ErrorDataReceived += (sender, args) => errors.Add(args.Data);

            try
            {
                process.Start();
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
                throw new SystemException("Document file format analysis could not to be executed, process is skipped. Details can be found in arkade-tmp/logs/");
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            if (errors.Any())
                errors.ForEach(Log.Debug);

            return results;
        }

        private static IEnumerable<SiegfriedFileInfo> GetSiegfriedFileInfoObjects(IEnumerable<string> formatInfoSet)
        {
            var siegfriedFileInfoObjects = new List<SiegfriedFileInfo>();

            foreach (string siegfriedFormatResult in formatInfoSet.Skip(1))
            {
                if (siegfriedFormatResult == null)
                    continue;

                using (var stringReader = new StringReader(siegfriedFormatResult))
                using (var csvParser = new CsvParser(stringReader, CultureInfo.InvariantCulture))
                {
                    string[] record = csvParser.Read();

                    var documentFileListElement = new SiegfriedFileInfo
                    (
                        fileName: record[0],
                        errors: record[3],
                        id: record[5],
                        format: record[6],
                        version: record[7]
                    );

                    siegfriedFileInfoObjects.Add(documentFileListElement);
                }
            }

            return siegfriedFileInfoObjects;
        }

        private static string GetOSSpecificExecutableFileName()
        {
            string fileName;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                fileName = ArkadeConstants.SiegfriedWindowsExecutable;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                fileName = ArkadeConstants.SiegfriedLinuxExecutable;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                fileName = ArkadeConstants.SiegfriedMacOSXExecutable;
            else
                throw new SiegfriedFileFormatIdentifierException("Arkade could not identify your OS, format analysis will be skipped");

            return fileName;
        }
    }

    public class SiegfriedFileFormatIdentifierException : Exception
    {
        public SiegfriedFileFormatIdentifierException()
        {
        }

        public SiegfriedFileFormatIdentifierException(string message)
            : base(message)
        {
        }

        public SiegfriedFileFormatIdentifierException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
