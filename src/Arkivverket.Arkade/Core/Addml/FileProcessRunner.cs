using System;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml
{
    public class FileProcessRunner
    {
        private readonly ProcessManager _processManager;
        private readonly Dictionary<string, List<IAddmlProcess>> _processCache;

        public FileProcessRunner(ProcessManager processManager)
        {
            _processManager = processManager;
            _processCache = _processManager.GetFileProcesses();
        }

        /// <summary>
        /// Run the processes defined at File level for the specified File.
        /// </summary>
        /// <param name="file">the file to run the processes on</param>
        public void RunProcesses(FlatFile file)
        {
            foreach (IAddmlProcess process in GetProcessesForFile(file))
            {
                process.Run(file);
            }
        }
        /// <summary>
        /// Run the processes defined at File level for the specified Record.
        /// </summary>
        /// <param name="file">the file containing process definitions</param>
        /// <param name="record">the Record to run the processes on</param>
        public void RunProcesses(FlatFile file, Record record)
        {
            foreach (IAddmlProcess process in GetProcessesForFile(file))
            {
                process.Run(record);
            }
        }

        private List<IAddmlProcess> GetProcessesForFile(FlatFile file)
        {
            return _processManager.GetProcesses(file.Definition.Key(), _processCache);
        }

    }
}