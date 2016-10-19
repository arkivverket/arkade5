using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml
{
    public class FieldProcessRunner
    {
        private readonly ProcessFactory _processFactory;
        private readonly Dictionary<string, List<IAddmlProcess>> _processCache;

        public FieldProcessRunner(ProcessFactory processFactory)
        {
            _processFactory = processFactory;
            _processCache = _processFactory.GetFieldProcesses();
        }

        public void RunProcesses(Field field)
        {
            List<IAddmlProcess> processes = _processFactory.GetProcesses(field.Definition.Key(), _processCache);

            foreach (IAddmlProcess process in processes)
            {
                process.Run(field);
            }
        }
    }
}
