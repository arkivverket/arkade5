using System;
using System.IO;
using System.Reflection;
using Arkivverket.Arkade.Core;
using Newtonsoft.Json;
using Serilog;

namespace Arkivverket.Arkade.Cli
{
    internal class CommandLineRunner
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public void Run(string archivePath, string incomingArchiveType, string metadataFile)
        {
            ArkadeProcessingArea.Establish(@"c:\dev\tmp\arkadecli\processingarea");

            var arkade = new Core.Arkade();

            var fileInfo = new FileInfo(archivePath);
            Log.Information($"Processing archive: {fileInfo.FullName}");

            if (!Enum.TryParse(incomingArchiveType, true, out ArchiveType archiveType))
            {
                Log.Error("Unknown archive type");
                throw new ArgumentException("unknown archive type");
            }

            TestSession testSession;
            if (File.Exists(archivePath))
            {
                Log.Debug("File exists");
                testSession = arkade.RunTests(ArchiveFile.Read(archivePath, archiveType));
            }
            else if (Directory.Exists(archivePath))
            {
                Log.Debug("Directory exists");
                testSession = arkade.RunTests(ArchiveDirectory.Read(archivePath, archiveType));
            }
            else
            {
                throw new ArgumentException("Invalid archive path");
            }

            if (testSession != null)
            {
                string metadataJsonFile = @"c:\dev\tmp\arkadecli\metadata.json";
                var archiveMetadata = JsonConvert.DeserializeObject<ArchiveMetadata>(File.ReadAllText(metadataJsonFile));

                testSession.ArchiveMetadata = archiveMetadata;

                arkade.CreatePackage(testSession, PackageType.SubmissionInformationPackage, @"c:\dev\tmp\arkadecli");
            }

            ArkadeProcessingArea.CleanUp();
        }
    }
}