using System;
using System.IO;
using System.Reflection;
using Arkivverket.Arkade.Core.Base;
using Newtonsoft.Json;
using RestSharp.Extensions;
using Serilog;

namespace Arkivverket.Arkade.CLI
{
    internal class CommandLineRunner
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public void Run(CommandLineOptions options)
        {
            try
            {
                var arkade = new Core.Base.Arkade();

                var fileInfo = new FileInfo(options.Archive);
                Log.Information($"Processing archive: {fileInfo.FullName}");

                if (!Enum.TryParse(options.ArchiveType, true, out ArchiveType archiveType))
                {
                    Log.Error("Unknown archive type");
                    throw new ArgumentException("unknown archive type");
                }

                TestSession testSession = CreateTestSession(options, arkade, archiveType);

                if (!TestingIsSkipped(options))
                {
                    arkade.RunTests(testSession);
                    SaveTestReport(arkade, testSession, options);
                }

                if (!PackingIsSkipped(options))
                {
                    var archiveMetadata = JsonConvert.DeserializeObject<ArchiveMetadata>(File.ReadAllText(options.MetadataFile));

                    archiveMetadata.PackageType = options.InformationPackageType != null &&
                                                  options.InformationPackageType.Equals("AIP")
                        ? PackageType.ArchivalInformationPackage
                        : PackageType.SubmissionInformationPackage;

                    testSession.ArchiveMetadata = archiveMetadata;
                    testSession.ArchiveMetadata.Id = $"UUID:{testSession.Archive.Uuid}";

                    arkade.CreatePackage(testSession, options.OutputDirectory);
                }
            }
            finally
            {
                ArkadeProcessingArea.CleanUp();
            }
        }

        private static bool TestingIsSkipped(CommandLineOptions options)
        {
            return options.Skip.HasValue() && options.Skip.Equals("testing");
        }

        private static bool PackingIsSkipped(CommandLineOptions options)
        {
            return options.Skip.HasValue() && options.Skip.Equals("packing");
        }

        private static TestSession CreateTestSession(CommandLineOptions options, Core.Base.Arkade arkade, ArchiveType archiveType)
        {
            TestSession testSession;
            if (File.Exists(options.Archive))
            {
                Log.Debug("File exists");
                testSession = arkade.CreateTestSession(ArchiveFile.Read(options.Archive, archiveType));
            }
            else if (Directory.Exists(options.Archive))
            {
                Log.Debug("Directory exists");
                testSession = arkade.CreateTestSession(ArchiveDirectory.Read(options.Archive, archiveType));
            }
            else
            {
                throw new ArgumentException("Invalid archive path: " + options.Archive);
            }
            return testSession;
        }

        private static void SaveTestReport(Core.Base.Arkade arkade, TestSession testSession, CommandLineOptions options)
        {
            var packageTestReport = new FileInfo(Path.Combine(
                testSession.GetReportDirectory().FullName, "report.html"
            ));
            arkade.SaveReport(testSession, packageTestReport);

            var standaloneTestReport = new FileInfo(Path.Combine(
                options.OutputDirectory, string.Format(OutputStrings.TestReportFileName, testSession.Archive.Uuid)
            ));
            arkade.SaveReport(testSession, standaloneTestReport);
        }
    }
}