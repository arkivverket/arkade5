using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public class SiegfriedFileFormatIdentifier : IFileFormatIdentifier
    {
        public Dictionary<FileInfo, FileFormat> IdentifyFormat(IEnumerable<FileInfo> files)
        {
            string siegfriedFormatInfo = AskSiegfried(files);

            JObject json = JObject.Parse(siegfriedFormatInfo);

            var filesWithFormat = new Dictionary<FileInfo, FileFormat>();

            foreach (JToken file in json["files"])
            {
                JToken firstMatch = file["matches"][0];

                var puId = (string) firstMatch["id"];
                var name = (string) firstMatch["format"];
                var version = (string) firstMatch["version"];
                var mimeType = (string) firstMatch["mime"];

                var fileInfo = new FileInfo((string) file["filename"]);
                var fileFormat = new FileFormat(puId: puId, name: name, version: version, mimeType: mimeType);

                filesWithFormat.Add(fileInfo, fileFormat);
            }

            return filesWithFormat;
        }

        private static string AskSiegfried(IEnumerable<FileInfo> files)
        {
            string[] fullFileNames = files.Select(f => f.FullName).ToArray();

            string fileNameArguments = string.Join(" ", fullFileNames);

            string siegfriedDirectory = Path.Combine("Bundled", "Siegfried");
            string siegfriedExecutable = Path.Combine(siegfriedDirectory, "siegfried.exe");

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = siegfriedExecutable,
                    Arguments = $"-home {siegfriedDirectory} -json {fileNameArguments}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();

            string result = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            return result;
        }
    }
}
