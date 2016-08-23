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
            string archiveFileName = @"C:\Dropbox (Arkitektum AS)\Ark_prosjekter\Arkivverket\Arkade 5\Testdata\noark5_testdata-alice-in-wonderland\2a4c611b-fc02-4ad0-8b11-1b9956eaa400.tar";
            string metadataFileName = @"C:\dev\src\arkade\src\Arkivverket.Arkade.Test\TestData\noark5-info.xml";
            if (args.Length != 0)
            {
                archiveFileName = args[0];
                metadataFileName = args[1];
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

                ArchiveExtraction archiveExtraction = archiveExtractionReader.ReadFromFile(archiveFileName, metadataFileName);
                Console.WriteLine($"Reading from archive: {archiveFileName}");
                Console.WriteLine($"Uuid: {archiveExtraction.Uuid}");
                Console.WriteLine($"WorkingDirectory: {archiveExtraction.WorkingDirectory}");
                Console.WriteLine($"ArchiveType: {archiveExtraction.ArchiveType}");


                new TestEngine().LoadArchive(archiveExtraction);
            }

        }
    }
}
