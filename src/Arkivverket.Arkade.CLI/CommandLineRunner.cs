using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Arkivverket.Arkade.CLI.Options;
using Arkivverket.Arkade.CLI.Utils;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Metadata;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing.Noark5;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.Core.Util.ArchiveFormatValidation;
using Arkivverket.Arkade.Core.Util.FileFormatIdentification;
using Org.BouncyCastle.Tls;
using Serilog;

namespace Arkivverket.Arkade.CLI
{
    internal static class CommandLineRunner
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);
        private static readonly Core.Base.Arkade Arkade;

        private static readonly IStatusEventHandler StatusEventHandler;

        private static bool _testRunHasFailed;

        private static FormatAnalysisProgressPresenter _formatAnalysisProgressPresenter;

        static CommandLineRunner()
        {
            Arkade = new Core.Base.Arkade();
            StatusEventHandler = Arkade.StatusEventHandler;

            StatusEventHandler.TestProgressUpdatedEvent += OnTestProgressUpdatedEvent;
            StatusEventHandler.OperationMessageEvent += OnOperationMessageEvent;
            StatusEventHandler.SiardValidationFinishedEvent += OnSiardValidationFinishedEvent;
            StatusEventHandler.FormatAnalysisStartedEvent += OnFormatAnalysisStartedEvent;
            StatusEventHandler.FormatAnalysisProgressUpdatedEvent += OnFormatAnalysisProgressUpdatedEvent;
            StatusEventHandler.FormatAnalysisFinishedEvent += OnFormatAnalysisFinishedEvent;
            StatusEventHandler.TargetSizeCalculatorFinishedEvent += OnTargetSizeCalculatorFinishedEvent;

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

        private static void OnFormatAnalysisStartedEvent(object sender, FormatAnalysisProgressEventArgs eventArgs)
        {
            _formatAnalysisProgressPresenter = new FormatAnalysisProgressPresenter();
        }

        private static void OnTargetSizeCalculatorFinishedEvent(object sender, TargetSizeCalculatorEventArgs eventArgs)
        {
            _formatAnalysisProgressPresenter.SetTotalAmountOfFiles(eventArgs.TargetSize);
        }

        private static void OnFormatAnalysisProgressUpdatedEvent(object sender, FormatAnalysisProgressEventArgs eventArgs)
        {
            if (Console.IsOutputRedirected)
                return;

            _formatAnalysisProgressPresenter?.UpdateAndDisplayProgress(eventArgs.FileSize);
        }

        private static void OnFormatAnalysisFinishedEvent(object sender, FormatAnalysisProgressEventArgs eventArgs)
        {
            if (Console.IsOutputRedirected)
                return;

            _formatAnalysisProgressPresenter.DisplayFinished();
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

                ArchiveProcessing archiveProcessing = LoadArchiveInput(options.Archive, options.ArchiveType, command);

                TestSession testSession = CreateTestSession(archiveProcessing.Archive, options.OutputLanguage, options.TestSelectionFile);

                bool testSuccess = Test(options.OutputDirectory, options.TestResultDisplayLimit, testSession,
                    createStandAloneTestReport: false);

                InputDiasPackage inputDiasPackage = archiveProcessing.InputDiasPackage;

                bool packSuccess = Pack(options.MetadataFile, options.InformationPackageType, archiveProcessing.Archive, options.OutputDirectory, SupportedLanguage.en, options.PerformFileFormatAnalysis);

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
                DoCleanUp();
            }
        }

        public static void Run(TestOptions options)
        {
            try
            {
                string command = GetRunningCommand(options.GetType().Name);

                Archive archive = LoadArchiveInput(options.Archive, options.ArchiveType, command).Archive;

                TestSession testSession = CreateTestSession(archive, options.OutputLanguage, options.TestSelectionFile);

                bool testSuccess = Test(options.OutputDirectory, options.TestResultDisplayLimit, testSession);

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
                DoCleanUp();
            }
        }

        public static void Run(PackOptions options)
        {
            try
            {
                string command = GetRunningCommand(options.GetType().Name);

                ArchiveProcessing archiveProcessing = LoadArchiveInput(options.Archive, options.ArchiveType, command);

                InputDiasPackage inputDiasPackage = archiveProcessing.InputDiasPackage;
                
                LogFinishedStatus(command, Pack(options.MetadataFile, options.InformationPackageType, archiveProcessing.Archive, options.OutputDirectory, SupportedLanguage.en, options.PerformFileFormatAnalysis));
            }
            finally
            {
                DoCleanUp();
            }
        }

        public static void Run(GenerateOptions options)
        {
            string command = GetRunningCommand(options.GetType().Name);

            if (options.GenerateMetadataExampleFile)
            {
                string metadataExampleFileName = Path.Combine(options.OutputDirectory,
                    options.MetadataExampleFileName ?? OutputFileNames.MetadataExampleFile);
                Arkade.GenerateMetadataExampleFile(metadataExampleFileName);
                Log.Information(metadataExampleFileName + " was created");
            }

            if (options.GenerateNoark5TestSelectionFile)
            {
                string noark5TestSelectionFileName = Path.Combine(options.OutputDirectory,
                    options.Noark5TestSelectionFileName ?? OutputFileNames.Noark5TestSelectionFile);
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

            Log.Information($"{{{command.TrimEnd('e')}ing}} format of all content in {analysisDirectory.FullName}");
            string outputFileName = options.FormatAnalysisResultFileName ?? string.Format(
                OutputFileNames.FileFormatInfoFile,
                analysisDirectory.Name.TrimEnd(Path.GetInvalidFileNameChars())
            );

            SupportedLanguage language = GetSupportedLanguage(options.OutputLanguage);

            IEnumerable<IFileFormatInfo> fileFormatInfos = Arkade.
                AnalyseFileFormats(analysisDirectory.FullName, FileFormatScanMode.Directory);
            Arkade.GenerateFileFormatInfoFiles(fileFormatInfos, analysisDirectory.FullName,
                Path.Combine(options.OutputDirectory, outputFileName), language);
            
            LogFinishedStatus(command);
        }

        public static void Run(ValidateOptions options)
        {
            string command = GetRunningCommand(options.GetType().Name);

            FileSystemInfo item = File.GetAttributes(options.Item).HasFlag(FileAttributes.Directory)
                ? new DirectoryInfo(options.Item)
                : new FileInfo(options.Item);

            var archiveFormat = options.Format.ToUpper().GetValueByDescription<ArchiveFormat>();

            Log.Information($"{{{command.TrimEnd('e')}ing}} the format of {item} as {archiveFormat.GetDescription()}");

            ArchiveFormatValidationReport validationReport = Arkade.ValidateArchiveFormatAsync(
                item, archiveFormat, options.OutputDirectory, SupportedLanguage.en).Result;

            Log.Information(validationReport.ToString());

            LogFinishedStatus(command);
        }

        internal static void Dispose()
        {
            Arkade.Dispose();
        }

        private static bool Test(string outputDirectory, int testResultDisplayLimit, TestSession testSession,
            bool createStandAloneTestReport = true)
        {
            if (!TestSession.IsTestableArchive(testSession.Archive, testSession.AddmlDefinition, out _))
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

            Uuid diasPackageId = Uuid.Random(); // Noko må gjerast ...

            SaveTestReport(testSession, outputDirectory, createStandAloneTestReport, testResultDisplayLimit, diasPackageId);
            return true;
        }

        private static bool Pack(string metadataFile, string packageType, Archive archive, string outputDirectory, SupportedLanguage outputLanguage, bool generateFileFormatInfo)
        {
            ArchiveProcessing archiveProcessing = null; // TODO: Provide

            var workingDirectory = new WorkingDirectory(archiveProcessing.ProcessingDirectory);
            
            var outputDiasPackage = new OutputDiasPackage(InformationPackageCreator.ParsePackageType(packageType), archive, MetadataLoader.Load(metadataFile));

            Arkade.CreatePackage(outputDiasPackage, outputDirectory); //, workingDirectory);

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

        private static ArchiveProcessing LoadArchiveInput(string archiveSourcePath, string archiveTypeString, string command)
        {
            FileSystemInfo archiveSource = File.Exists(archiveSourcePath) ? new FileInfo(archiveSourcePath)
                : Directory.Exists(archiveSourcePath) ? new DirectoryInfo(archiveSourcePath)
                : throw new ArgumentException("Invalid archive path: " + archiveSourcePath);


            ArchiveType archiveType = GetArchiveType(archiveTypeString, archiveSourcePath);

            Log.Information($"{{{command}ing}} {archiveType} archive from source: {archiveSource.FullName}");

            return archiveSource switch
            {
                DirectoryInfo or FileInfo { Extension: ".siard" }
                    => new ArchiveProcessing(Arkade.LoadArchiveExtraction(archiveSource, archiveType)),

                FileInfo { Extension: ".tar" } tarFile
                    => new ArchiveProcessing(Arkade.LoadDiasPackage(tarFile, archiveType)),

                _ => throw new ArgumentException("Unsupported archive input or input + archive type combination")
            };
        }

        private static TestSession CreateTestSession(Archive archive, string selectedOutputLanguage, string testSelectionFilePath = null)
        {
            Log.Information("Creating test session");

            TestSession testSession = Arkade.CreateTestSession(archive);
            
            if (archive.ArchiveType == ArchiveType.Noark5)
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

            return testSession;
        }

        private static void SaveTestReport(TestSession testSession, string outputDirectory,
            bool createStandAloneTestReport, int testResultDisplayLimit, Uuid diasPackageId = null)
        {
            DirectoryInfo packageTestReportDirectory = testSession.Archive.GetTestReportDirectory();

            if (createStandAloneTestReport)
            {
                string testReportDirectoryName = string.Format(OutputFileNames.StandaloneTestReportDirectory, diasPackageId); // NB! UUID-writeout (test results)
                packageTestReportDirectory = new DirectoryInfo(Path.Combine(outputDirectory, testReportDirectoryName));
                packageTestReportDirectory.Create();
            }

            Arkade.SaveReport(testSession, packageTestReportDirectory, createStandAloneTestReport, testResultDisplayLimit);

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
            if (!TestSession.IsTestableArchive(testSession.Archive, testSession.AddmlDefinition, out string disqualifyingCause))
            {
                Log.Error("Archive is not testable: " + disqualifyingCause);
                return false;
            }
            return true;
        }

        private static void DoCleanUp()
        {
            Log.Debug("Deleting temporary files");
            
            if (!ArkadeProcessingArea.CleanUp())
            {
                ExceptionMessages.Culture = CultureInfo.CreateSpecificCulture(SupportedLanguage.en.ToString());
                Log.Error($"\n{ExceptionMessages.ProcessAreaCleanUpFailed}", ArkadeProcessingArea.WorkDirectory.FullName);
            }
            else
            {
                Log.Debug("Temporary files deleted");
            }
        }
    }
}
