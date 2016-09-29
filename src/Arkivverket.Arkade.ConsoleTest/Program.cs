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
            string archiveFileName = @"C:\Dropbox (Arkitektum AS)\Arkade5 - Testdata\Testdata\n5-aclie-in-wonderland-komplett.tar";
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
