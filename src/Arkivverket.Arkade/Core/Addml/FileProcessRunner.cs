using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml
{
    public class FileProcessRunner
    {
        private readonly ProcessFactory _processFactory;
        private readonly Dictionary<string, List<IAddmlProcess>> _processCache;

        public FileProcessRunner(ProcessFactory processFactory)
        {
            _processFactory = processFactory;
            _processCache = _processFactory.GetFileProcesses();
        }

        public void RunProcesses(FlatFile file)
        {
            List<IAddmlProcess> processes = _processFactory.GetProcesses(file.Definition.Key(), _processCache);

            foreach (IAddmlProcess process in processes)
            {
                process.Run(file);
            }
        }
    }
}