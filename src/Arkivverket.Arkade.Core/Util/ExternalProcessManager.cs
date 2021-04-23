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

        public static void AddProcess(Process process)
        {
            Processes.Add(process.Id, process);
        }

        public static void CloseProcess(Process process)
        {
            int processId = process.Id;
            process.Close();
            Processes.Remove(processId);
        }

        public static void TerminateAllProcesses()
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
