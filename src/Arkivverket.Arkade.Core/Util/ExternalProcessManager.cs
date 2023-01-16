using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Serilog;

namespace Arkivverket.Arkade.Core.Util
{
    /// <summary>
    /// <para>Manages processes which are spawned from Arkade.</para>
    /// </summary>
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
            if (!Processes.ContainsValue(process))
                return;

            int processId = process.Id;
            process.Close();
            Processes.Remove(processId);
        }

        public static int Close(int processId)
        {
            if (!Processes.ContainsKey(processId))
                return -1;

            if (Processes[processId] == default)
            {
                Processes?.Remove(processId);
                return 1;
            }

            Processes[processId].Close();
            Processes?.Remove(processId);
            return 0;
        }

        public static void Terminate(string processName)
        {
            Process process = Processes.FirstOrDefault(p => p.Value.ProcessName.Contains(processName)).Value;
            if (process == default)
                return;

            int processId = process.Id;
            process.Kill();
            process.Dispose();
            Processes.Remove(processId);
        }

        public static void TerminateAll()
        {
            if (Processes.Count == 0)
                return;

            foreach ((int _, Process process) in Processes)
            {
                if (process == default) continue;
                process.Kill();
                process.Dispose();
            }
            Processes.Clear();

            Log.Information("Cleanup complete");
        }

        public static bool HasActiveProcess(string processName)
        {
            Process process = Processes.Values.FirstOrDefault(p => !p.HasExited && p.ProcessName.Contains(processName));
            return process != default;
        }
    }
}
