using System;
using System.Collections.Generic;
using CommandLine;
using Serilog;

namespace Arkivverket.Arkade.Cli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.fff} {SourceContext} [{Level}] {Message}{NewLine}{Exception}")
                .CreateLogger();

            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(RunOptionsAndReturnExitCode)
                .WithNotParsed(HandleParseError);
        }

        private static void RunOptionsAndReturnExitCode(CommandLineOptions options)
        {
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
                    Console.WriteLine("Usage ..."); //options.GetUsage());
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
                   && !string.IsNullOrWhiteSpace(options.MetadataFile)
                   && !string.IsNullOrWhiteSpace(options.ProcessingArea)
                   && !string.IsNullOrWhiteSpace(options.PackageOutputDirectory);
        }
    }
}