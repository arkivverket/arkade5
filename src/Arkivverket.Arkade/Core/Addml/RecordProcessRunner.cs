using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml
{
    public class RecordProcessRunner
    {
        private readonly ProcessFactory _processFactory;
        private readonly Dictionary<string, List<IAddmlProcess>> _processCache;

        public RecordProcessRunner(ProcessFactory processFactory)
        {
            _processFactory = processFactory;
            _processCache = processFactory.GetRecordProcesses();
        }

        public void RunProcesses(Record record)
        {
            List<IAddmlProcess> processes = _processFactory.GetProcesses(record.Definition.Key(), _processCache);

            foreach (IAddmlProcess process in processes)
            {
                process.Run(record);
            }
        }
    }
}
