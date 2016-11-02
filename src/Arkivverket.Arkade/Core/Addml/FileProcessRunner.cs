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

        public void RunProcesses(FlatFile file)
        {
            List<IAddmlProcess> processes = _processManager.GetProcesses(file.Definition.Key(), _processCache);

            foreach (IAddmlProcess process in processes)
            {
                process.Run(file);
            }
        }
    }
}