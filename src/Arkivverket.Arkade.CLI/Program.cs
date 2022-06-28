using System;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Util;
using Arkivverket.Arkade.Core.Util.ArchiveFormatValidation;
using CommandLine;
using Serilog;
using Serilog.Events;
using static System.String;

namespace Arkivverket.Arkade.CLI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (!Console.IsOutputRedirected)
                Console.CursorVisible = false;
            Console.CancelKeyPress += OnProcessCanceled;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            ArkadeProcessingArea.SetupTemporaryLogsDirectory();

            ConfigureLogging(); // Configured with temporary log directory

            bool hasRun = false;

            ParseArguments(args)
                .WithParsed<ProcessOptions>(RunProcessOptions)
                .WithParsed<TestOptions>(RunTestOptions)
                .WithParsed<PackOptions>(RunPackOptions)
                .WithParsed<GenerateOptions>(RunGenerateOptions)
                .WithParsed<AnalyseOptions>(RunAnalyseOptions)
                .WithParsed<ValidateOptions>(options => RunValidateOptions(options, out hasRun))
                .WithNotParsed(LogParseErrors);

            if (!Console.IsOutputRedirected)
                Console.CursorVisible = true;
            
            if (hasRun)
                CommandLineRunner.Dispose();
        }

        public static ParserResult<object> ParseArguments(IEnumerable<string> args)
        {
            return Parser.Default.ParseArguments<TestOptions, PackOptions, ProcessOptions, GenerateOptions, AnalyseOptions, ValidateOptions>(args);
        }

        private static void RunProcessOptions(ProcessOptions processOptions)
        {
            if (!ReadyToRun(processOptions.OutputDirectory, processOptions.ProcessingArea,
                metadataFilePath: processOptions.MetadataFile,
                testSelectionFilePath: processOptions.TestSelectionFile,
                languageForOutputFiles: processOptions.OutputLanguage))
                return;

            CommandLineRunner.Run(processOptions);
        }

        private static void RunTestOptions(TestOptions testOptions)
        {
            if (!ReadyToRun(testOptions.OutputDirectory, testOptions.ProcessingArea,
                testSelectionFilePath: testOptions.TestSelectionFile,
                languageForOutputFiles: testOptions.OutputLanguage))
                return;

            CommandLineRunner.Run(testOptions);
        }

        private static void RunPackOptions(PackOptions packOptions)
        {
            if (!ReadyToRun(packOptions.OutputDirectory, packOptions.ProcessingArea,
                metadataFilePath: packOptions.MetadataFile,
                languageForOutputFiles: packOptions.OutputLanguage))
                return;

            CommandLineRunner.Run(packOptions);
        }

        private static void RunGenerateOptions(GenerateOptions generateOptions)
        {
            if (!ReadyToRun(generateOptions))
                return;

            CommandLineRunner.Run(generateOptions);
        }

        private static void RunAnalyseOptions(AnalyseOptions analyseOptions)
        {
            if (!ReadyToRun(analyseOptions))
                return;

            CommandLineRunner.Run(analyseOptions);
        }

        private static void RunValidateOptions(ValidateOptions validateOptions, out bool canRun)
        {
            canRun = ReadyToRun(validateOptions);
            if (!canRun)
                return;

            CommandLineRunner.Run(validateOptions);
        }

        private static bool ReadyToRun(OutputOptions outputOptions)
        {
            return DirectoryArgsExists(outputOptions.OutputDirectory) &&
                SelectedOutputLanguageIsValid(outputOptions.OutputLanguage);
        }

        private static bool ReadyToRun(AnalyseOptions analyseOptions)
        {
            return DirectoryArgsExists(analyseOptions.OutputDirectory,
                documentFileDirectoryPath: analyseOptions.FormatCheckTarget) &&
                SelectedOutputLanguageIsValid(analyseOptions.OutputLanguage);
        }

        private static bool ReadyToRun(ValidateOptions validateOptions)
        {
            if (!(File.Exists(validateOptions.Item) || Directory.Exists(validateOptions.Item)))
            {
                Log.Error($@"Item [{validateOptions.Item}] was not found");
                return false;
            }

            if (!validateOptions.Format.ToUpper().TryParseFromDescription(out ArchiveFormat archiveFormat))
            {
                Log.Error($@"Format [{validateOptions.Format}] was not recognized");
                return false;
            }

            if (Directory.Exists(validateOptions.Item))
            {
                if (IsNullOrEmpty(validateOptions.OutputDirectory))
                {
                    Log.Error(@"The -o/--output-directory argument is required when -i/--item is a directory.");
                    return false;
                }

                return DirectoryArgsExists(validateOptions.OutputDirectory);
            }

            return true;
        }

        private static bool ReadyToRun(string outputDirectoryPath, string processingAreaPath = null,
            string documentFileDirectoryPath = null, string metadataFilePath = null, string testSelectionFilePath = null,
            string languageForOutputFiles = null)
        {
            if (!(DirectoryArgsExists(outputDirectoryPath, processingAreaPath, documentFileDirectoryPath) &&
                  FileArgsExists(metadataFilePath, testSelectionFilePath) &&
                  SelectedOutputLanguageIsValid(languageForOutputFiles)))
                return false;

            ArkadeProcessingArea.Establish(processingAreaPath); // Removes temporary log directory

            ConfigureLogging(); // Re-configured with log directory within processing area

            return true;
        }

        private static bool DirectoryArgsExists(string outputDirectoryPath, string processingAreaPath = null, string documentFileDirectoryPath = null)
        {
            if (!Directory.Exists(outputDirectoryPath))
            {
                Log.Error(new DirectoryNotFoundException(), $"Could not find output directory: '{outputDirectoryPath}'.");
                return false;
            }

            if (!new DirectoryInfo(outputDirectoryPath).HasWritePermission())
            {
                Log.Error(new IOException(), $"Arkade does not have write permissions to specified location: '{outputDirectoryPath}'");
                return false;
            }

            if (processingAreaPath != null && !Directory.Exists(processingAreaPath))
            {
                Log.Error(new DirectoryNotFoundException(), $"Could not find processing area: '{processingAreaPath}'.");
                return false;
            }

            if (documentFileDirectoryPath != null && !Directory.Exists(documentFileDirectoryPath))
            {
                Log.Error(new DirectoryNotFoundException(), $"Could not find document file directory: '{processingAreaPath}'.");
                return false;
            }
            return true;
        }

        private static bool FileArgsExists(string metadataFilePath = null, string testSelectionFilePath = null)
        {
            if (metadataFilePath != null && !File.Exists(metadataFilePath))
            {
                Log.Error(new FileNotFoundException(), $"Could not find metadata file: '{metadataFilePath}'.");
                return false;
            }

            if (testSelectionFilePath != null && !File.Exists(testSelectionFilePath))
            {
                Log.Error(new FileNotFoundException(), $"Could not find test selection file: '{testSelectionFilePath}'.");
                return false;
            }

            return true;
        }

        private static bool SelectedOutputLanguageIsValid(string selectedLanguageForOutputFiles)
        {
            if (selectedLanguageForOutputFiles == null || Enum.TryParse(selectedLanguageForOutputFiles, out SupportedLanguage _))
                return true;
            Log.Error(new ArgumentOutOfRangeException(), $"'{selectedLanguageForOutputFiles}' is not a valid value for output language.");
            return false;
        }

        private static void ConfigureLogging()
        {
            string systemLogFilePath = Path.Combine(
                ArkadeProcessingArea.LogsDirectory.ToString(),
                ArkadeConstants.SystemLogFileNameFormat
            );

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: OutputStrings.SystemLogOutputTemplateForConsole, restrictedToMinimumLevel:LogEventLevel.Information)
                .WriteTo.File(systemLogFilePath, rollingInterval: RollingInterval.Day,
                    outputTemplate: OutputStrings.SystemLogOutputTemplateForFile)
                .CreateLogger();
        }

        private static void LogParseErrors(IEnumerable<Error> errors)
        {
            foreach (Error error in errors)
            {
                if (error.Tag != ErrorType.HelpRequestedError && error.Tag != ErrorType.HelpVerbRequestedError && error.Tag != ErrorType.VersionRequestedError)
                    Log.Error(error.ToString());
            }
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            ExternalProcessManager.TerminateAll();
        }

        private static void OnProcessCanceled(object sender, EventArgs e)
        {
            Log.Logger.Information("Exiting Arkade due to external CTRL-C or CTRL-BREAK");
            Environment.Exit(0);
        }
    }
}
