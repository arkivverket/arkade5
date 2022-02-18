using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    public class FileProcessRunner
    {
        private readonly ProcessManager _processManager;
        private readonly Dictionary<IAddmlIndex, List<IAddmlProcess>> _processCache;

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

        public void RunProcesses(FlatFile file, Field field, long recordNumber)
        {
            foreach (IAddmlProcess process in GetProcessesForFile(file))
            {
                process.Run(field, recordNumber);
            }
        }

        private List<IAddmlProcess> GetProcessesForFile(FlatFile file)
        {
            return _processManager.GetProcesses(file.Definition.GetIndex(), _processCache);
        }

        public void EndOfFile(FlatFile file)
        {
            foreach (IAddmlProcess process in GetProcessesForFile(file))
            {
                process.EndOfFile();
            }
        }
    }
}