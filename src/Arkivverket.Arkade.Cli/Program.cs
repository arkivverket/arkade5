using System;
using System.IO;
using Arkivverket.Arkade.Core;
using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json;
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

                if (!string.IsNullOrWhiteSpace(options.GenerateMetadataExample))
                    new MetadataExampleGenerator().Generate(options.GenerateMetadataExample);
                else
                {
                    if (ValidArgumentsForTesting(options))
                        new CommandLineRunner().Run(options.Archive, options.ArchiveType, options.MetadataFile);
                    else
                        Console.WriteLine(options.GetUsage());
                }
                    
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured: {exceptionMessage}", e.Message);
            }

            
        }

        private static bool ValidArgumentsForTesting(CommandLineOptions options)
        {
            return !string.IsNullOrWhiteSpace(options.Archive)
                   && !string.IsNullOrWhiteSpace(options.ArchiveType)
                   && !string.IsNullOrWhiteSpace(options.MetadataFile);
        }
    }
}