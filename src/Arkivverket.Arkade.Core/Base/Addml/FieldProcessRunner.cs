using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    public class FieldProcessRunner
    {
        private readonly ProcessManager _processManager;
        private readonly Dictionary<IAddmlIndex, List<IAddmlProcess>> _processCache;

        public FieldProcessRunner(ProcessManager processManager)
        {
            _processManager = processManager;
            _processCache = _processManager.GetFieldProcesses();
        }

        public void RunProcesses(Field field)
        {
            List<IAddmlProcess> processes = _processManager.GetProcesses(field.Definition.GetIndex(), _processCache);
            foreach (IAddmlProcess process in processes)
            {
                process.Run(field);
            }
        }

        public void EndOfFile(FlatFile file)
        {
            foreach (AddmlRecordDefinition recordDefinition in file.Definition.AddmlRecordDefinitions)
            {
                foreach (AddmlFieldDefinition fieldDefinition in recordDefinition.AddmlFieldDefinitions)
                {
                    List<IAddmlProcess> processes = _processManager.GetProcesses(fieldDefinition.GetIndex(), _processCache);
                    foreach (IAddmlProcess process in processes)
                    {
                        process.EndOfFile();
                    }
                }
            }
        }
    }
}
