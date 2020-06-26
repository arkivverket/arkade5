using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using Serilog;

namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public class SiegfriedFileFormatIdentifier : IFileFormatIdentifier
    {
        public IEnumerable<SiegfriedFileInfo> IdentifyFormat(DirectoryInfo directory)
        {
            Process siegfriedProcess = SetupSiegfriedProcess();

            IEnumerable<string> siegfriedResult = RunProcessOnDirectory(siegfriedProcess, directory);

            return GetSiegfriedFileInfoObjects(siegfriedResult);
        }

        private static Process SetupSiegfriedProcess()
        {
            string bundleDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bundled");
            string siegfriedDirectory = Path.Combine(bundleDirectory, "Siegfried");
            string siegfriedExecutable = Path.Combine(siegfriedDirectory, "siegfried.exe");
            string argumentsExceptInputDirectory = $"-home {siegfriedDirectory} -multi 256 -csv -log e,w -coe ";

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

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            if (errors.Any())
                errors.ForEach(Log.Error);

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
    }
}
