using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Util;
using CommandLine;
using Serilog;

namespace Arkivverket.Arkade.CLI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            ArkadeProcessingArea.SetupTemporaryLogsDirectory();

            ConfigureLogging(); // Configured with temporary log directory

            ParseArguments(args)
                .WithParsed<ProcessOptions>(RunProcessOptions)
                .WithParsed<TestOptions>(RunTestOptions)
                .WithParsed<PackOptions>(RunPackOptions)
                .WithParsed<GenerateOptions>(RunGenerateOptions)
                .WithNotParsed(LogParseErrors);
        }

        public static ParserResult<object> ParseArguments(IEnumerable<string> args)
        {
            return Parser.Default.ParseArguments<TestOptions, PackOptions, ProcessOptions, GenerateOptions>(args);
        }

        private static void RunProcessOptions(ProcessOptions processOptions)
        {
            if (!ReadyToRun(processOptions.OutputDirectory, processOptions.ProcessingArea, processOptions.MetadataFile,
                processOptions.TestListFile))
                return;

            CommandLineRunner.Run(processOptions);
        }

        private static void RunTestOptions(TestOptions testOptions)
        {
            if (!ReadyToRun(testOptions.OutputDirectory, testOptions.ProcessingArea,
                testListFilePath: testOptions.TestListFile))
                return;

            CommandLineRunner.Run(testOptions);
        }

        private static void RunPackOptions(PackOptions packOptions)
        {
            if (!ReadyToRun(packOptions.OutputDirectory, packOptions.ProcessingArea, packOptions.MetadataFile))
                return;

            CommandLineRunner.Run(packOptions);
        }

        private static void RunGenerateOptions(GenerateOptions generateOptions)
        {
            if (!ReadyToRun(generateOptions))
                return;

            CommandLineRunner.Run(generateOptions);
        }

        private static bool ReadyToRun(Options options)
        {
            return DirectoryArgsExists(options.OutputDirectory);
        }

        private static bool ReadyToRun(string outputDirectoryPath, string processingAreaPath,
            string metadataFilePath = null, string testListFilePath = null)
        {
            if (!(DirectoryArgsExists(outputDirectoryPath, processingAreaPath) && FileArgsExists(metadataFilePath, testListFilePath)))
                return false;

            ArkadeProcessingArea.Establish(processingAreaPath); // Removes temporary log directory

            ConfigureLogging(); // Re-configured with log directory within processing area

            return true;
        }

        private static bool DirectoryArgsExists(string outputDirectoryPath, string processingAreaPath = null)
        {
            if (!Directory.Exists(outputDirectoryPath))
            {
                Log.Error(new DirectoryNotFoundException(), $"Could not find output directory: '{outputDirectoryPath}'.");
                return false;
            }

            if (processingAreaPath != null && !Directory.Exists(processingAreaPath))
            {
                Log.Error(new DirectoryNotFoundException(), $"Could not find processing area: '{processingAreaPath}'.");
                return false;
            }

            return true;
        }

        private static bool FileArgsExists(string metadataFilePath = null, string testListFilePath = null)
        {
            if (metadataFilePath != null && !File.Exists(metadataFilePath))
            {
                Log.Error(new FileNotFoundException(), $"Could not find metadata file: '{metadataFilePath}'.");
                return false;
            }

            if (testListFilePath != null && !File.Exists(testListFilePath))
            {
                Log.Error(new FileNotFoundException(), $"Could not find test list file: '{testListFilePath}'.");
                return false;
            }

            return true;
        }

        private static void ConfigureLogging()
        {
            string systemLogFilePath = Path.Combine(
                ArkadeProcessingArea.LogsDirectory.ToString(),
                ArkadeConstants.SystemLogFileNameFormat
            );

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(outputTemplate: OutputStrings.SystemLogOutputTemplateForConsole)
                .WriteTo.RollingFile(systemLogFilePath, outputTemplate: OutputStrings.SystemLogOutputTemplateForFile)
                .CreateLogger();
        }

        private static void LogParseErrors(IEnumerable<Error> errors)
        {
            foreach (Error error in errors)
            {
                if (error.Tag != ErrorType.HelpRequestedError && error.Tag != ErrorType.HelpVerbRequestedError)
                    Log.Error(error.ToString());
            }
        }
    }
}
