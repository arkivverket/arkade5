using System;
using System.Collections.Generic;
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
            string directory = @"C:\Dropbox (Arkitektum AS)\Arkade5 - Testdata\Komplette uttrekk\Noark5-alice-liten\";
            string archiveFileName = directory + @"ee07a0f9-4564-43ab-b80d-ac975ba12aed.tar";
            string metadataFileName = directory + @"info.xml";
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
                TestSessionFactory testSessionBuilder = container.Resolve<TestSessionFactory>();

                TestSession testSession = testSessionBuilder.NewSessionFromTarFile(archiveFileName, metadataFileName);
                Console.WriteLine($"Reading from archive: {archiveFileName}");
                Console.WriteLine($"Uuid: {testSession.Archive.Uuid}");
                Console.WriteLine($"WorkingDirectory: {testSession.Archive.WorkingDirectory}");
                Console.WriteLine($"ArchiveType: {testSession.Archive.ArchiveType}");


                TestEngine testEngine = container.Resolve<TestEngine>();
                TestSuite testSuite = testEngine.RunTestsOnArchive(testSession);
                foreach (TestRun testRun in testSuite.TestRuns)
                {
                    Console.WriteLine($"Test: {testRun.TestName}, duration={testRun.TestDuration}, success={testRun.IsSuccess()}");
                }

            }

        }
    }
}
