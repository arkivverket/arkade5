using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Arkivverket.Arkade.Core.Logging;
using Serilog;

namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public class SiegfriedProcessRunner
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

        private static IStatusEventHandler _statusEventHandler;

        private static readonly List<string> _supportedArchiveFilePronomCodes = new()
        {
            "x-fmt/263", //.zip
            "x-fmt/265", //.tar
            "fmt/289", "fmt/1355", "fmt/1281", //.warc
            "fmt/161", "fmt/995", "fmt/1196", "fmt/1777" // .siard
        };

        public SiegfriedProcessRunner(IStatusEventHandler statusEventHandler)
        {
            _statusEventHandler = statusEventHandler;
        }

        internal IEnumerable<string> Run(Process process)
        {
            var results = new List<string>();
            var errors = new List<string>();
            var headerLineAdded = false;

            process.OutputDataReceived += (_, args) =>
            {
                if (headerLineAdded)
                {
                    HandleReceivedOutputData(args, results);
                }
                else
                {
                    results.Add(args.Data); 
                    headerLineAdded = true;
                }
            };
            process.ErrorDataReceived += (_, args) => HandleReceivedErrorData(args, errors);

            try
            {
                ExternalProcessManager.Start(process);
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

        internal string RunOnFile(Process process)
        {
            return Run(process).Skip(1).First();
        }

        internal string RunOnByteArray(Process process, KeyValuePair<string, IEnumerable<byte>> filePathAndByteContent)
        {
            using var streamFromByteContent = new MemoryStream(filePathAndByteContent.Value.ToArray());
            return RunOnStream(process, streamFromByteContent, filePathAndByteContent.Key);
        }

        internal string RunOnStream(Process process, Stream stream, string fileName)
        {
            var results = new List<string>();
            var errors = new List<string>();

            process.OutputDataReceived += (_, args) => HandleReceivedOutputData(args, results);
            process.ErrorDataReceived += (_, args) => HandleReceivedErrorData(args, errors);

            try
            {
                ExternalProcessManager.Start(process);

                using StreamWriter streamWriter = process.StandardInput;
                stream.CopyTo(streamWriter.BaseStream);
            }
            catch (Exception e)
            {
                ExternalProcessManager.Close(process);
                try
                {
                    process.StartInfo.StandardInputEncoding = Encoding.UTF8;
                    ExternalProcessManager.Start(process);
                    using StreamWriter streamWriter = process.StandardInput;
                    stream.CopyTo(streamWriter.BaseStream);
                }
                catch (Exception)
                {
                    Log.Debug(e.ToString());
                    throw new SiegfriedFileFormatIdentifierException("Document file format analysis could not to be executed, process is skipped. Details can be found in arkade-tmp/logs/");
                }
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            if (errors.Any())
                errors.ForEach(Log.Debug);

            string result = fileName + results.Skip(1).First();

            return result;
        }

        internal Process SetupSiegfriedProcess(FileFormatScanMode scanMode, string analysisTargetFullName)
        {
            string executableFileName = GetOsSpecificExecutableFileName();

            string thirdPartySoftwareDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                ArkadeConstants.DirectoryNameThirdPartySoftware);
            string siegfriedDirectory = Path.Combine(thirdPartySoftwareDirectory, ArkadeConstants.DirectoryNameSiegfried);
            string siegfriedExecutable = Path.Combine(siegfriedDirectory, executableFileName);
            string arguments = $"-home \"{siegfriedDirectory}\" -csv -log e,w -coe {BuildSiegfriedArgument(scanMode, analysisTargetFullName)}";

            Process process = new() 
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = siegfriedExecutable,
                    Arguments = arguments,
                    StandardOutputEncoding = Encoding.UTF8,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = scanMode is FileFormatScanMode.Stream,
                }
            };
            return process;
        }

        private static string GetOsSpecificExecutableFileName()
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

        private static string BuildSiegfriedArgument(FileFormatScanMode scanMode, string analysisTargetFullName)
        {
            
            string siegfriedArgument = scanMode switch
            {
                FileFormatScanMode.Directory => "-multi 256 ",
                FileFormatScanMode.File => "",
                FileFormatScanMode.Stream => "-",
                FileFormatScanMode.Archive => "-z",
                _ => throw new SiegfriedFileFormatIdentifierException(
                    $"Siegfried scan mode {{{nameof(scanMode)}}} is not implemented")
            };

            if (scanMode is FileFormatScanMode.Stream)
                return siegfriedArgument;

            string rootDirPostfix = analysisTargetFullName.Equals(Path.GetPathRoot(analysisTargetFullName))
                ? Path.DirectorySeparatorChar.ToString()
                : string.Empty;

            return $"{siegfriedArgument} \"{analysisTargetFullName}{rootDirPostfix}\"";
        }

        private static void HandleReceivedOutputData(DataReceivedEventArgs eventArgs, ICollection<string> results)
        {
            if (eventArgs.Data == null) return;

            results.Add(eventArgs.Data);
            IFileFormatInfo siegfriedFileInfo = SiegfriedFileInfo.CreateFromString(eventArgs.Data);

            if (_supportedArchiveFilePronomCodes.Contains(siegfriedFileInfo.Id)) 
                return;
            
            if (long.TryParse(siegfriedFileInfo.ByteSize, out long result))
                _statusEventHandler.RaiseEventFormatAnalysisProgressUpdated(result);
            
        }

        private static void HandleReceivedErrorData(DataReceivedEventArgs eventArgs, ICollection<string> errors)
        {
            errors.Add(eventArgs.Data);
        }
    }
}
