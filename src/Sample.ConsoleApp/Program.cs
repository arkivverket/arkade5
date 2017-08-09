using System.IO;
using System.Reflection;
using Arkivverket.Arkade.Core;
using Serilog;

namespace Sample.ConsoleApp
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.ColoredConsole(
                    outputTemplate: "{Timestamp:yyyy-MM-ddTHH:mm:ss.fff} {SourceContext} [{Level}] {Message}{NewLine}{Exception}")
                .CreateLogger();

            string archiveFileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "noark5-testuttrekk.tar");

            var arkade = new Arkade();
            TestSession session = arkade.RunTests(ArchiveFile.Read(archiveFileName, ArchiveType.Noark5));

            foreach (TestRun testRun in session.TestSuite.TestRuns)
            {
                Log.Information($"Test name: {testRun.TestName}, duration={testRun.TestDuration}, success={testRun.IsSuccess()}");
            }
        }
    }
}