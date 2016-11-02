using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml
{
    public class FieldProcessRunner
    {
        private readonly ProcessManager _processManager;
        private readonly Dictionary<string, List<IAddmlProcess>> _processCache;

        public FieldProcessRunner(ProcessManager processManager)
        {
            _processManager = processManager;
            _processCache = _processManager.GetFieldProcesses();
        }

        public void RunProcesses(Field field)
        {
            List<IAddmlProcess> processes = _processManager.GetProcesses(field.Definition.Key(), _processCache);

            foreach (IAddmlProcess process in processes)
            {
                process.Run(field);
            }
        }
    }
}
