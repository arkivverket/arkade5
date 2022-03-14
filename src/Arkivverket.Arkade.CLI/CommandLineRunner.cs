using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing.Noark5;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.Core.Util.ArchiveFormatValidation;
using Serilog;

namespace Arkivverket.Arkade.CLI
{
    internal static class CommandLineRunner
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);
        private static readonly Core.Base.Arkade Arkade;

        private static readonly IStatusEventHandler StatusEventHandler;

        private static bool _testRunHasFailed;

        static CommandLineRunner()
        {
            Arkade = new Core.Base.Arkade();
            StatusEventHandler = Arkade.StatusEventHandler;

            StatusEventHandler.TestProgressUpdatedEvent += OnTestProgressUpdatedEvent;
            StatusEventHandler.OperationMessageEvent += OnOperationMessageEvent;
            StatusEventHandler.SiardValidationFinishedEvent += OnSiardValidationFinishedEvent;

            Log.Information($"\n" +
                            $"********************************************************************************\n" +
                            $"* ARKADE 5 CLI v{ArkadeVersion.Current}                                                          *\n" +
                            $"* Copyright © 2017 Arkivverket                                                 *\n" +
                            $"* Licensed under the GNU Affero General Public License (GNU AGPL), Version 3.0 *\n" +
                            $"********************************************************************************\n");

            Log.Information(GetThirdPartySoftwareInfo());

            if (Arkade.Version().UpdateIsAvailable())
            {
                Log.Warning("    The current Arkade 5 CLI version is outdated!");
                Log.Information($"Arkade 5 CLI v{Arkade.Version().GetLatest().ToString(3)} is available.");
            }

            Log.Information(
                "Download new releases, see release notes and version history at: " + ArkadeConstants.ArkadeWebSiteUrl + "\n");
        }

        private static void OnTestProgressUpdatedEvent(object sender, TestProgressEventArgs eventArgs)
        {
            if (eventArgs.HasFailed)
            {
                Log.Error(eventArgs.FailMessage);
                _testRunHasFailed = true;
            }
            else
                Console.WriteLine(eventArgs.TestProgress);
        }

        private static void OnOperationMessageEvent(object sender, OperationMessageEventArgs e)
        {
            Log.Debug(e.Message);
        }

        private static void OnSiardValidationFinishedEvent(object sender, SiardValidationEventArgs eventArgs)
        {
            List<string> errorsAndWarnings = eventArgs.Errors.Where(e => e != null).ToList();

            if (!errorsAndWarnings.Any())
                return;

            errorsAndWarnings.Where(e => !e.StartsWith("WARN")).ToList().ForEach(Log.Error);

            errorsAndWarnings.Where(e =>
                    e.StartsWith("WARN") && !ArkadeConstants.SuppressedDbptkWarningMessages.Contains(e)).ToList()
                .ForEach(Log.Warning);
        }

        private static string GetThirdPartySoftwareInfo()
        {
            var info = new StringBuilder();

            info.AppendLine("\n----------------------------- THIRD PARTY SOFTWARE -----------------------------\n");
            info.AppendLine("-- Siegfried --");
            info.AppendLine("PURPOSE: identify document file format.");
            info.AppendLine("Copyright © 2019 Richard Lehane");
            info.AppendLine("Available from: https://www.itforarchivists.com/siegfried/");
            info.AppendLine("Licensed under the Apache License, Version 2.0");
            info.AppendLine();
            info.AppendLine("-- iText 7 --");
            info.AppendLine("PURPOSE: generate PDF documents");
            info.AppendLine("Copyright © 2021 iText Group nv (HQ Belgium), Inc. All rights reserved.");
            info.AppendLine("Available from: https://itextpdf.com/");
            info.AppendLine("Licensed under the GNU Affero General Public License (GNU AGPL), Version 3.0");
            info.AppendLine("\n--------------------------------------------------------------------------------\n");

            return info.ToString();
        }

        public static void Run(ProcessOptions options)
        {
            try
            {
                string command = GetRunningCommand(options.GetType().Name);

                TestSession testSession = CreateTestSession(options.Archive, options.ArchiveType, command,
                    options.OutputLanguage, options.TestSelectionFile, options.PerformFileFormatAnalysis);

                bool testSuccess = Test(options.OutputDirectory, testSession, createStandAloneTestReport: false);

                bool packSuccess = Pack(options.MetadataFile, options.InformationPackageType, options.OutputDirectory, testSession);

                LogFinishedStatus(command, RanWithoutErrors(testSession) && testSuccess && packSuccess);
            }
            catch (SiardArchiveReaderException siardEx)
            {
                Log.Error(siardEx.Message);
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
                    options.OutputLanguage, options.TestSelectionFile);

                bool testSuccess = Test(options.OutputDirectory, testSession);

                LogFinishedStatus(command, RanWithoutErrors(testSession) && testSuccess);
            }
            catch (SiardArchiveReaderException siardEx)
            {
                Log.Error(siardEx.Message);
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
                    options.OutputLanguage, performFileFormatAnalysis: options.PerformFileFormatAnalysis);

                LogFinishedStatus(command, Pack(options.MetadataFile, options.InformationPackageType, options.OutputDirectory, testSession));
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
                string metadataFileName = Path.Combine(options.OutputDirectory, OutputFileNames.MetadataFile);
                new MetadataExampleGenerator().Generate(metadataFileName);
                Log.Information(metadataFileName + " was created");
            }

            if (options.GenerateNoark5TestSelectionFile)
            {
                string noark5TestSelectionFileName = Path.Combine(options.OutputDirectory, OutputFileNames.Noark5TestSelectionFile);
                SupportedLanguage language = GetSupportedLanguage(options.OutputLanguage);
                Noark5TestSelectionFileGenerator.Generate(noark5TestSelectionFileName, language);
                Log.Information(noark5TestSelectionFileName + " was created");
            }

            LogFinishedStatus(command);
        }

        public static void Run(AnalyseOptions options)
        {
            string command = GetRunningCommand(options.GetType().Name);

            var analysisDirectory = new DirectoryInfo(options.FormatCheckTarget);

            Log.Information($"{{{command.TrimEnd('e')}ing}} format of all content in {analysisDirectory}");
            string outputFileName = options.OutputFileName ?? string.Format(
                OutputFileNames.FileFormatInfoFile,
                analysisDirectory.Name.TrimEnd(Path.GetInvalidFileNameChars())
            );

            SupportedLanguage language = GetSupportedLanguage(options.OutputLanguage);

            Arkade.GenerateFileFormatInfoFiles(analysisDirectory, options.OutputDirectory, outputFileName, language);
            
            LogFinishedStatus(command);
        }

        public static void Run(ValidateOptions options)
        {
            string command = GetRunningCommand(options.GetType().Name);

            FileSystemInfo item = File.GetAttributes(options.Item).HasFlag(FileAttributes.Directory)
                ? new DirectoryInfo(options.Item)
                : new FileInfo(options.Item);

            var archiveFormat = options.Format.GetValueByDescription<ArchiveFormat>();

            Log.Information($"{{{command.TrimEnd('e')}ing}} the format of {item} as {archiveFormat.GetDescription()}");

            ArchiveFormatValidationReport validationReport = Arkade.ValidateArchiveFormat(item, archiveFormat, SupportedLanguage.en).Result;

            Log.Information(validationReport.ToString());

            LogFinishedStatus(command);
        }

        private static bool Test(string outputDirectory, TestSession testSession, bool createStandAloneTestReport = true)
        {
            if (!testSession.IsTestableArchive(out _))
                return false;

            try
            {
                Arkade.RunTests(testSession);
            }
            catch (Exception e)
            {
                StatusEventHandler.RaiseEventTestProgressUpdated("", true, e.Message);
                return false;
            }

            if (_testRunHasFailed)
                return false;

            SaveTestReport(testSession, outputDirectory, createStandAloneTestReport);
            return true;
        }

        private static bool Pack(string metadataFile, string packageType, string outputDirectory,
            TestSession testSession)
        {
            ArchiveMetadata archiveMetadata = MetadataLoader.Load(metadataFile);

            archiveMetadata.PackageType = InformationPackageCreator.ParsePackageType(packageType);

            testSession.ArchiveMetadata = archiveMetadata;
            testSession.ArchiveMetadata.Id = $"UUID:{testSession.Archive.Uuid}";

            Arkade.CreatePackage(testSession, outputDirectory);

            return true;
        }

        private static ArchiveType GetArchiveType(string archiveTypeString, string archive)
        {
            if (string.IsNullOrWhiteSpace(archiveTypeString))
            {
                ArchiveType? detectedArchiveType = Arkade.DetectArchiveType(archive);

                if (detectedArchiveType == null)
                {
                    string errorMessage =
                        $"Arkade could not detect archive type of {archive}. " +
                        "Please check the structure- or info-file. To attempt a forced run, explicitly specify type with parameter \"-t\"";
                    throw new ArgumentException(errorMessage);
                }
                Log.Information($"Arkade determined {archive} to be of type {detectedArchiveType}");
                return (ArchiveType) detectedArchiveType;
            }

            if (!Enum.TryParse(archiveTypeString, true, out ArchiveType selectedArchiveType))
            {
                Log.Error("Unknown archive type");
                throw new ArgumentException("unknown archive type");
            }

            return selectedArchiveType;
        }

        private static SupportedLanguage GetSupportedLanguage(string chosenLanguage)
        {
            if (!Enum.TryParse(chosenLanguage, out SupportedLanguage language))
                throw new ArgumentException("Language \"" + chosenLanguage + "\" is not supported");
         
            return language;
        }

        private static TestSession CreateTestSession(string archive, string archiveTypeString,
            string command, string selectedOutputLanguage, string testSelectionFilePath = null,
            bool performFileFormatAnalysis = false)
        {
            var fileInfo = new FileInfo(archive);
            Log.Information($"{{{command}ing}} archive: {fileInfo.FullName}");

            ArchiveType archiveType = GetArchiveType(archiveTypeString, archive);

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
                testSession.TestsToRun = File.Exists(testSelectionFilePath)
                    ? Noark5TestSelectionFileReader.GetUserSelectedTestIds(testSelectionFilePath)
                    : Noark5TestProvider.GetAllTestIds();

                if (testSession.TestsToRun.Count == 0)
                    throw new ArgumentException($"No tests selected in {testSelectionFilePath}");
            }

            selectedOutputLanguage ??= Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            if (!Enum.TryParse(selectedOutputLanguage, out SupportedLanguage outputLanguage))
                outputLanguage = SupportedLanguage.en;
            testSession.OutputLanguage = outputLanguage;

            testSession.GenerateFileFormatInfo = performFileFormatAnalysis;

            return testSession;
        }

        private static void SaveTestReport(TestSession testSession, string outputDirectory,
            bool createStandAloneTestReport)
        {
            DirectoryInfo packageTestReportDirectory = testSession.Archive.GetTestReportDirectory();

            if (createStandAloneTestReport)
            {
                string testReportDirectoryName = string.Format(OutputFileNames.StandaloneTestReportDirectory, testSession.Archive.Uuid);
                packageTestReportDirectory = new DirectoryInfo(Path.Combine(outputDirectory, testReportDirectoryName));
                packageTestReportDirectory.Create();
            }

            Arkade.SaveReport(testSession, packageTestReportDirectory, createStandAloneTestReport);

            if (createStandAloneTestReport)
                Log.Information($"Test reports generated at: {packageTestReportDirectory.FullName}");
        }

        private static void LogFinishedStatus(string command, bool withoutErrors = true)
        {
            if (withoutErrors)
                Log.Information($"Arkade 5 CLI {ArkadeVersion.Current} {{{command}}} successfully finished.");
            else
                Log.Warning($"Arkade 5 CLI {ArkadeVersion.Current} {{{command}}} finished with errors.");
        }

        private static string GetRunningCommand(string optionType)
        {
            int optionsStartIndex = optionType.Length - "options".Length;
            return optionType.Remove(optionsStartIndex).ToLower();
        }

        private static bool RanWithoutErrors(TestSession testSession)
        {
            if (!testSession.IsTestableArchive(out string disqualifyingCause))
            {
                Log.Error("Archive is not testable: " + disqualifyingCause);
                return false;
            }
            return true;
        }
    }
}
