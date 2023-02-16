using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Serilog;

namespace Arkivverket.Arkade.Core.Util
{
    /// <summary>
    /// <para>Manages processes which are spawned from Arkade.</para>
    /// <para>Third party applications (i.e. Siegfried, dbptk) should be managed with <see cref="Start"/> and
    /// <see cref="Close(Process)"/></para>
    /// Processes spawned via third party libraries (i.e greenfield-apps from Codeuctivity's PDF/A validator) should
    /// be managed with <see cref="Add"/> and <see cref="Remove"/>.
    /// </summary>
    public static class ExternalProcessManager
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);
        private static readonly Dictionary<int, Process> Processes = new();

        private static readonly BackgroundWorker ProcessListener = new();

        static ExternalProcessManager()
        {
            ProcessListener.DoWork += ProcessListenerOnDoWork;
        }

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

        public static bool TryClose(Process process)
        {
            try
            {
                int processId = process.Id;

                if (!Processes.ContainsKey(processId))
                    return false;

                if (Processes[processId] == default)
                {
                    Processes?.Remove(processId);
                    return true;
                }

                Processes[processId].Close();
                Processes?.Remove(processId);
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }

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

        /// <summary>
        /// Use when process is spawned from a third party library (I.e veraPDF - greenfield-apps.jar)
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="startTimeAfter"></param>
        public static void Add(string processName, DateTime startTimeAfter)
        {
            ProcessListener.RunWorkerAsync(new object[]{processName, startTimeAfter});
        }

        /// <summary>
        /// Use when process is spawned from a third party library (I.e veraPDF - greenfield-apps.jar)
        /// </summary>
        /// <param name="processName"></param>
        public static void Remove(string processName)
        {
            var process = Processes.Values.FirstOrDefault(p => p.ProcessName.Contains(processName));
            if (process != default)
                Processes.Remove(process.Id);
        }

        private static void ProcessListenerOnDoWork(object sender, DoWorkEventArgs e)
        {
            var arguments = e.Argument as object[];

            var processName = arguments[0] as string;

            if (arguments[1] is not DateTime startTime)
                return;

            Process process = default;

            while (process == default)
            {
                process = Process.GetProcesses().FirstOrDefault(p =>
                    p.ProcessName.Contains(processName) &&
                    p.StartTime.CompareTo(startTime) >= 0);
            }

            Processes.Add(process.Id, process);
        }
    }
}
