using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Identify;
using Arkivverket.Arkade.Util;
using Autofac;

namespace Arkivverket.Arkade.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide file name for archive extraction (.tar).");
                return;
            }

            var builder = new ContainerBuilder();
            builder.RegisterModule(new ArkadeAutofacModule());
            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                ArchiveExtractionReader archiveExtractionReader = container.Resolve<ArchiveExtractionReader>();


                ArchiveExtraction archiveExtraction = archiveExtractionReader.ReadFromFile(args[0]);
                Console.WriteLine($"Reading from archive: {args[0]}");
                Console.WriteLine($"Uuid: {archiveExtraction.Uuid}");
                Console.WriteLine($"WorkingDirectory: {archiveExtraction.WorkingDirectory}");
            }

            


        }
    }
}
