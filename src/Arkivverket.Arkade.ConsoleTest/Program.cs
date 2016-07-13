using System;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.Util;
using Autofac;
using Serilog;

namespace Arkivverket.Arkade.ConsoleTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide file name for archive extraction (.tar) and the corresponding metadata file (info.xml)");
                return;
            }

            var builder = new ContainerBuilder();
            builder.RegisterModule(new ArkadeAutofacModule());
            var container = builder.Build();

            Log.Logger = new LoggerConfiguration()
                             .MinimumLevel.Debug()
                             .WriteTo.ColoredConsole(outputTemplate: "{Timestamp:yyyy-MM-ddTHH:mm:ss.fff} {SourceContext} [{Level}] {Message}{NewLine}{Exception}")
                             .CreateLogger();

            using (container.BeginLifetimeScope())
            {
                ArchiveExtractionReader archiveExtractionReader = container.Resolve<ArchiveExtractionReader>();

                ArchiveExtraction archiveExtraction = archiveExtractionReader.ReadFromFile(args[0], args[1]);
                Console.WriteLine($"Reading from archive: {args[0]}");
                Console.WriteLine($"Uuid: {archiveExtraction.Uuid}");
                Console.WriteLine($"WorkingDirectory: {archiveExtraction.WorkingDirectory}");
                Console.WriteLine($"ArchiveType: {archiveExtraction.ArchiveType}");
            }

        }
    }
}
