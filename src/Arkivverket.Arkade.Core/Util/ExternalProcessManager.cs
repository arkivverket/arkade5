using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Serilog;

namespace Arkivverket.Arkade.Core.Util
{
    public static class ExternalProcessManager
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);
        private static readonly Dictionary<int, Process> Processes = new();

        public static void Start(Process process)
        {
            process.Start();
            Processes.Add(process.Id, process);
        }

        public static void Close(Process process)
        {
            int processId = process.Id;
            process.Close();
            Processes.Remove(processId);
        }

        public static void TerminateAll()
        {
            if (Processes.Count == 0)
                return;

            foreach ((int _, Process process) in Processes)
            {
                process.Kill();
                process.Dispose();
            }
            Processes.Clear();

            Log.Information("Cleanup complete");
        }
    }
}
