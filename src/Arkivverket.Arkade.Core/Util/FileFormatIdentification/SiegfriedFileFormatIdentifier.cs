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

        public IEnumerable<IFileFormatInfo> IdentifyFormat(DirectoryInfo directory)
        {
            const SiegfriedScanMode scanMode = SiegfriedScanMode.Directory;

            Process siegfriedProcess = SetupSiegfriedProcess(scanMode);

            IEnumerable<string> siegfriedResult = RunProcessOnDirectory(siegfriedProcess, directory);

            siegfriedProcess.Close();

            return GetSiegfriedFileInfoObjects(siegfriedResult);
        }

        public IFileFormatInfo IdentifyFormat(FileInfo file)
        {
            const SiegfriedScanMode scanMode = SiegfriedScanMode.File;

            Process siegfriedProcess = SetupSiegfriedProcess(scanMode);

            string siegfriedResult = RunProcessOnFile(siegfriedProcess, file);

            siegfriedProcess.Close();

            return GetSiegfriedFileInfoObject(siegfriedResult);
        }

        public IFileFormatInfo IdentifyFormat(KeyValuePair<string, Stream> filePathAndStream)
        {
            const SiegfriedScanMode scanMode = SiegfriedScanMode.Stream;

            Process siegfriedProcess = SetupSiegfriedProcess(scanMode);

            string siegfriedResult = RunProcessOnStream(siegfriedProcess, filePathAndStream);

            siegfriedProcess.Close();

            return GetSiegfriedFileInfoObject(siegfriedResult);
        }

        private static Process SetupSiegfriedProcess(SiegfriedScanMode scanMode)
        {
            string executableFileName = GetOSSpecificExecutableFileName();

            string bundleDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bundled");
            string siegfriedDirectory = Path.Combine(bundleDirectory, "Siegfried");
            string siegfriedExecutable = Path.Combine(siegfriedDirectory, executableFileName);
            string argumentsExceptInputDirectory = $"-home \"{siegfriedDirectory}\" -csv -log e,w -coe " + BuildSiegfriedArgument(scanMode);

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
            var results = new List<string>();
            var errors = new List<string>();

            process.OutputDataReceived += (sender, args) => results.Add(args.Data);
            process.ErrorDataReceived += (sender, args) => errors.Add(args.Data);

            try
            {
                process.StartInfo.Arguments += $"\"{directory.FullName}\"";

                process.Start();
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
                throw new SiegfriedFileFormatIdentifierException("Document file format analysis could not to be executed, process is skipped. Details can be found in arkade-tmp/logs/");
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            if (errors.Any())
                errors.ForEach(Log.Debug);

            return results;
        }

        private static string RunProcessOnFile(Process process, FileSystemInfo file)
        {
            return RunProcessOnDirectory(process, file).Skip(1).First();
        }

        private static string RunProcessOnStream(Process process, KeyValuePair<string, Stream> filePathAndStream)
        {
            var results = new List<string>();
            var errors = new List<string>();

            process.OutputDataReceived += (sender, args) => results.Add(args.Data);
            process.ErrorDataReceived += (sender, args) => errors.Add(args.Data);

            try
            {
                process.StartInfo.RedirectStandardInput = true;

                process.Start();
                StreamWriter streamWriter = process.StandardInput;

                filePathAndStream.Value.CopyTo(streamWriter.BaseStream);
                streamWriter.Close();
                filePathAndStream.Value.Close();
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
                throw new SiegfriedFileFormatIdentifierException("Document file format analysis could not to be executed, process is skipped. Details can be found in arkade-tmp/logs/");
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            if (errors.Any())
                errors.ForEach(Log.Debug);

            string result = filePathAndStream.Key + results.Skip(1).First();

            return result;
        }

        private static IEnumerable<SiegfriedFileInfo> GetSiegfriedFileInfoObjects(IEnumerable<string> formatInfoSet)
        {
            return formatInfoSet.Skip(1).Select(GetSiegfriedFileInfoObject).ToList();
        }

        private static SiegfriedFileInfo GetSiegfriedFileInfoObject(string siegfriedFormatResult)
        {
            if (siegfriedFormatResult == null)
                return null;

            using (var stringReader = new StringReader(siegfriedFormatResult))
            using (var csvParser = new CsvParser(stringReader, CultureInfo.InvariantCulture))
            {
                string[] record = csvParser.Read();

                return new SiegfriedFileInfo
                (
                    fileName: record[0],
                    errors: record[3],
                    id: record[5],
                    format: record[6],
                    version: record[7],
                    mimeType: record[8]
                );
            }
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

        private static string BuildSiegfriedArgument(SiegfriedScanMode scanMode)
        {
            return scanMode switch
            {
                SiegfriedScanMode.Directory => "-multi 256 ",
                SiegfriedScanMode.File => "",
                SiegfriedScanMode.Stream => "-",
                _ => throw new SiegfriedFileFormatIdentifierException(
                    $"Siegfried scan mode {{{nameof(scanMode)}}} is not implemented")
            };
        }
    }

    internal enum SiegfriedScanMode
    {
        Directory,
        File,
        Stream
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
