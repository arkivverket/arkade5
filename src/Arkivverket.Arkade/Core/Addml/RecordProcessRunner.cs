using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Addml
{
    public class RecordProcessRunner
    {
        private readonly Dictionary<IAddmlIndex, List<IAddmlProcess>> _processCache;
        private readonly ProcessManager _processManager;

        public RecordProcessRunner(ProcessManager processManager)
        {
            _processManager = processManager;
            _processCache = processManager.GetRecordProcesses();
        }

        public void RunProcesses(Record record)
        {
            List<IAddmlProcess> processes = _processManager.GetProcesses(record.Definition.GetIndex(), _processCache);
            foreach (IAddmlProcess process in processes)
            {
                process.Run(record);
            }
        }

        public void EndOfFile(FlatFile file)
        {
            foreach (AddmlRecordDefinition recordDefinition in file.Definition.AddmlRecordDefinitions)
            {
                List<IAddmlProcess> processes = _processManager.GetProcesses(recordDefinition.GetIndex(), _processCache);
                foreach (IAddmlProcess process in processes)
                {
                    process.EndOfFile();
                }
            }
        }
    }
}