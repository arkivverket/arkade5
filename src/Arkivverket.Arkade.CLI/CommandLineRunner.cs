using System;
using System.IO;
using System.Reflection;
using System.Text;
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

            Log.Information($"\n" +
                            $"***********************\n" +
                            $"* ARKADE 5 CLI v{ArkadeVersion.Current} *\n" +
                            $"***********************\n");

            Log.Information(GetBundledSoftwareInfo());

            if (Arkade.Version().UpdateIsAvailable())
            {
                Log.Warning("    The current Arkade 5 CLI version is outdated!");
                Log.Information($"Arkade 5 CLI v{Arkade.Version().GetLatest().ToString(3)} is available.");
            }

            Log.Information(
                "Download the latest Arkade 5 CLI version from: https://arkade.arkivverket.no/");
            Log.Information(
                "See version history and release notes at: https://arkade.arkivverket.no/ \n");
        }

        private static string GetBundledSoftwareInfo()
        {
            var info = new StringBuilder();

            info.AppendLine("\n-----------------------BUNDLED SOFTWARE-----------------------\n");
            info.AppendLine("-- Siegfried --");
            info.AppendLine("PURPOSE: identify document file format.");
            info.AppendLine("Copyright © 2019 Richard Lehane");
            info.AppendLine("Available from: https://www.itforarchivists.com/siegfried/");
            info.AppendLine("Licensed under the Apache License, Version 2.0");
            info.AppendLine("\n--------------------------------------------------------------\n");

            return info.ToString();
        }

        public static void Run(ProcessOptions options)
        {
            try
            {
                string command = GetRunningCommand(options.GetType().Name);

                TestSession testSession = CreateTestSession(options.Archive, options.ArchiveType, command,
                    options.TestListFile, options.DocumentFileFormatCheck);

                Test(options.OutputDirectory, testSession);

                Pack(options.MetadataFile, options.InformationPackageType, options.OutputDirectory, testSession);

                LogFinishedStatus(command);
            }
            catch (ArgumentException e)
            {
                Log.Error(e.Message);
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
                string command = GetRunningCommand(options.GetType().Name);

                TestSession testSession = CreateTestSession(options.Archive, options.ArchiveType, command,
                    options.TestListFile);

                Test(options.OutputDirectory, testSession);

                LogFinishedStatus(command);
            }
            catch (ArgumentException e)
            {
                Log.Error(e.Message);
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
                string command = GetRunningCommand(options.GetType().Name);

                TestSession testSession = CreateTestSession(options.Archive, options.ArchiveType, command,
                    checkDocumentFileFormat: options.DocumentFileFormatCheck);

                Pack(options.MetadataFile, options.InformationPackageType, options.OutputDirectory, testSession);

                LogFinishedStatus(command);
            }
            finally
            {
                ArkadeProcessingArea.CleanUp();
            }
        }

        public static void Run(GenerateOptions options)
        {
            string command = GetRunningCommand(options.GetType().Name);

            if (options.GenerateMetadataExample)
            {
                string metadataFileName = Path.Combine(options.OutputDirectory, ArkadeConstants.MetadataFileName);
                new MetadataExampleGenerator().Generate(metadataFileName);
                Log.Information(metadataFileName + " was created");
            }

            if (options.GenerateNoark5TestList)
            {
                string noark5TestListFileName = Path.Combine(options.OutputDirectory, ArkadeConstants.Noark5TestListFileName);
                Noark5TestListGenerator.Generate(noark5TestListFileName);
                Log.Information(noark5TestListFileName + " was created");
            }

            LogFinishedStatus(command);
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
            string command, string testListFilePath = null, bool checkDocumentFileFormat = false)
        {
            var fileInfo = new FileInfo(archive);
            Log.Information($"{{{command}ing}} archive: {fileInfo.FullName}");

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

                if (testSession.TestsToRun.Count == 0)
                    throw new ArgumentException($"No tests selected in {testListFilePath}");
            }

            testSession.GenerateDocumentFileInfo = checkDocumentFileFormat;

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
            Log.Information($"Test report generated at: {standaloneTestReport.FullName}");
        }

        private static void LogFinishedStatus(string command)
        {
            Log.Information($"Arkade 5 CLI {ArkadeVersion.Current} {{{command}}} successfully finished.");
        }

        private static string GetRunningCommand(string optionType)
        {
            int optionsStartIndex = optionType.Length - "options".Length;
            return optionType.Remove(optionsStartIndex).ToLower();
        }
    }
}
