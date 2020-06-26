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

        public static ParserResult<object>ParseArguments(IEnumerable<string> args)
        {
            return Parser.Default.ParseArguments<TestOptions, PackOptions, ProcessOptions, GenerateOptions>(args);
        }

        private static void RunProcessOptions(ProcessOptions processOptions)
        {
            PrepareRun(processOptions);

            CommandLineRunner.Run(processOptions);
        }

        private static void RunTestOptions(TestOptions testOptions)
        {
            PrepareRun(testOptions);
            
            CommandLineRunner.Run(testOptions);
        }

        private static void RunPackOptions(PackOptions packOptions)
        {
            PrepareRun(packOptions);

            CommandLineRunner.Run(packOptions);
        }

        private static void RunGenerateOptions(GenerateOptions generateOptions)
        {
            PrepareRun(generateOptions);

            new MetadataExampleGenerator().Generate(ArkadeConstants.MetadataFileName);
        }

        private static void PrepareRun(Options options)
        {
            ArkadeProcessingArea.Establish(options.ProcessingArea); // Removes temporary log directory

            ConfigureLogging(); // Re-configured with log directory within processing area
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
