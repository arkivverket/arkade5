using System;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Util;
using CommandLine;
using Serilog;

namespace Arkivverket.Arkade.Cli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ArkadeProcessingArea.SetupTemporaryLogsDirectory();

            ConfigureLogging(); // Configured with temporary log directory

            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(RunOptionsAndReturnExitCode)
                .WithNotParsed(HandleParseError);
        }

        private static void RunOptionsAndReturnExitCode(CommandLineOptions options)
        {
            ArkadeProcessingArea.Establish(options.ProcessingArea); // Removes temporary log directory

            ConfigureLogging(); // Re-configured with log directory within processing area

            if (ValidArgumentsForMetadataCreation(options))
            {
                new MetadataExampleGenerator().Generate(options.GenerateMetadataExample);
            }
            else
            {
                if (ValidArgumentsForTesting(options))
                {
                    new CommandLineRunner().Run(options);
                }
                else
                {
                    Console.WriteLine(options.GetUsage());
                }
            }
        }

        private static void HandleParseError(IEnumerable<Error> errors)
        {
            foreach (Error error in errors)
                Log.Error(error.ToString());
        }

        private static bool ValidArgumentsForMetadataCreation(CommandLineOptions options)
        {
            return !string.IsNullOrWhiteSpace(options.GenerateMetadataExample);
        }

        private static bool ValidArgumentsForTesting(CommandLineOptions options)
        {
            return !string.IsNullOrWhiteSpace(options.Archive)
                   && !string.IsNullOrWhiteSpace(options.ArchiveType)
                   && (
                       !string.IsNullOrWhiteSpace(options.Skip) && options.Skip.Equals("packing")
                       || !string.IsNullOrWhiteSpace(options.MetadataFile)
                   )
                   && !string.IsNullOrWhiteSpace(options.ProcessingArea)
                   && !string.IsNullOrWhiteSpace(options.OutputDirectory);
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
    }
}