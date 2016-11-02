using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml
{
    public class RecordProcessRunner
    {
        private readonly ProcessManager _processManager;
        private readonly Dictionary<string, List<IAddmlProcess>> _processCache;

        public RecordProcessRunner(ProcessManager processManager)
        {
            _processManager = processManager;
            _processCache = processManager.GetRecordProcesses();
        }

        public void RunProcesses(Record record)
        {
            List<IAddmlProcess> processes = _processManager.GetProcesses(record.Definition.Key(), _processCache);

            foreach (IAddmlProcess process in processes)
            {
                process.Run(record);
            }
        }
    }
}
