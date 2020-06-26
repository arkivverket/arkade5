using System;
using System.IO;
using System.Reflection;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.Core.Testing.Noark5;
using Arkivverket.Arkade.Core.Util;
using Serilog;

namespace Arkivverket.Arkade.CLI
{
    internal static class CommandLineRunner
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);
        private static readonly Core.Base.Arkade Arkade;

        static CommandLineRunner()
        {
            Arkade = new Core.Base.Arkade();

            Log.Information($"ARKADE 5 v{ArkadeVersion.Current} \n--------------------------------\n");

            if (Arkade.Version().UpdateIsAvailable())
            {
                Log.Warning("    The current Arkade 5 version is outdated!");
                Log.Information($"Arkade 5 v{Arkade.Version().GetLatest()} is available.");
            }

            Log.Information(
                "Download the latest Arkade 5 version from: https://github.com/arkivverket/arkade5/releases/latest");
            Log.Information(
                "See version history and release notes at: https://github.com/arkivverket/arkade5/releases \n");
        }

        public static void Run(ProcessOptions options)
        {
            try
            {
                TestSession testSession =
                    CreateTestSession(options.Archive, options.ArchiveType, options.TestListFile);

                Test(options.OutputDirectory, testSession);

                Pack(options.MetadataFile, options.InformationPackageType, options.OutputDirectory, testSession);
            }
            finally
            {
                ArkadeProcessingArea.CleanUp();
            }
        }

        public static void Run(TestOptions options)
        {
            try
            {
                TestSession testSession =
                    CreateTestSession(options.Archive, options.ArchiveType, options.TestListFile);

                Test(options.OutputDirectory, testSession);
            }
            finally
            {
                ArkadeProcessingArea.CleanUp();
            }
        }

        public static void Run(PackOptions options)
        {
            try
            {
                TestSession testSession = CreateTestSession(options.Archive, options.ArchiveType);

                Pack(options.MetadataFile, options.InformationPackageType, options.OutputDirectory, testSession);
            }
            finally
            {
                ArkadeProcessingArea.CleanUp();
            }
        }

        public static void Run(GenerateOptions options)
        {
            if (options.GenerateMetadataExample)
            {
                const string metadataFileName = ArkadeConstants.MetadataFileName;
                new MetadataExampleGenerator().Generate(metadataFileName);
                Log.Information(metadataFileName + " was created");
            }

            if (options.GenerateNoark5TestList)
            {
                const string noark5TestListFileName = ArkadeConstants.Noark5TestListFileName;
                Noark5TestListGenerator.Generate(noark5TestListFileName);
                Log.Information(noark5TestListFileName + " was created");
            }
        }

        private static void Test(string outputDirectory, TestSession testSession)
        {
            Arkade.RunTests(testSession);
            SaveTestReport(testSession, outputDirectory);
        }
        
        private static void Pack(string metadataFile, string packageType, string outputDirectory,
            TestSession testSession)
        {
            ArchiveMetadata archiveMetadata = MetadataLoader.Load(metadataFile);

            archiveMetadata.PackageType = InformationPackageCreator.ParsePackageType(packageType);

            testSession.ArchiveMetadata = archiveMetadata;
            testSession.ArchiveMetadata.Id = $"UUID:{testSession.Archive.Uuid}";

            Arkade.CreatePackage(testSession, outputDirectory);
        }

        private static ArchiveType GetArchiveType(string archiveTypeString)
        {
            if (!Enum.TryParse(archiveTypeString, true, out ArchiveType archiveType))
            {
                Log.Error("Unknown archive type");
                throw new ArgumentException("unknown archive type");
            }

            if (archiveType == ArchiveType.Noark4)
            {
                Log.Error("Archive type Noark 4 is currently not supported");
                throw new ArgumentException("unsupported archive type");
            }

            return archiveType;
        }

        private static TestSession CreateTestSession(string archive, string archiveTypeString,
            string testListFilePath = null)
        {
            var fileInfo = new FileInfo(archive);
            Log.Information($"Processing archive: {fileInfo.FullName}");

            ArchiveType archiveType = GetArchiveType(archiveTypeString);

            TestSession testSession;
            if (File.Exists(archive))
            {
                Log.Debug("File exists");
                testSession = Arkade.CreateTestSession(ArchiveFile.Read(archive, archiveType));
            }
            else if (Directory.Exists(archive))
            {
                Log.Debug("Directory exists");
                testSession = Arkade.CreateTestSession(ArchiveDirectory.Read(archive, archiveType));
            }
            else
            {
                throw new ArgumentException("Invalid archive path: " + archive);
            }

            if (archiveType == ArchiveType.Noark5)
            {
                testSession.TestsToRun = File.Exists(testListFilePath)
                    ? Noark5TestListReader.GetUserSelectedTestIds(testListFilePath)
                    : Noark5TestProvider.GetAllTestIds();
            }

            return testSession;
        }

        private static void SaveTestReport(TestSession testSession, string outputDirectory)
        {
            var packageTestReport = new FileInfo(Path.Combine(
                testSession.GetReportDirectory().FullName, "report.html"
            ));
            Arkade.SaveReport(testSession, packageTestReport);

            var standaloneTestReport = new FileInfo(Path.Combine(
                outputDirectory, string.Format(OutputStrings.TestReportFileName, testSession.Archive.Uuid)
            ));
            Arkade.SaveReport(testSession, standaloneTestReport);
        }
    }
}
