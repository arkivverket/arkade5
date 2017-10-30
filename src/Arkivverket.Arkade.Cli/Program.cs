using System;
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

            var options = new CommandLineOptions();
            try
            {
                Parser.Default.ParseArgumentsStrict(args, options);

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
            catch (Exception e)
            {
                Log.Error(e, "An error occured: {exceptionMessage}", e.Message);
            }
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