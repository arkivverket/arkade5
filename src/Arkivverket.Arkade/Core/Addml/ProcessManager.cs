using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Serilog;

namespace Arkivverket.Arkade.Core.Addml
{
    public class ProcessManager
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<ProcessManager>();

        private readonly AddmlDefinition _addmlDefinition;
        private readonly Dictionary<string, IAddmlProcess> _processesByName;
        private readonly ProcessTypeMapping _processTypeMapping = new ProcessTypeMapping();

        public ProcessManager(AddmlDefinition addmlDefinition)
        {
            _addmlDefinition = addmlDefinition;
            _processesByName = InstantiateProcesses();
        }

        private Dictionary<string, IAddmlProcess> InstantiateProcesses()
        {
            var processes = new Dictionary<string, IAddmlProcess>();
            foreach (string processName in GetUniqueProcesses())
            {
                Type type = _processTypeMapping.GetType(processName);
                if (type != null)
                {
                    Log.Debug($"Instantiating process: {processName}");
                    var process = (IAddmlProcess) Activator.CreateInstance(type);
                    processes.Add(processName, process);
                }
                else
                {
                    Log.Warning($"No process with name {processName} in ProcessTypeMapping.");
                }
            }
            return processes;
        }

        private HashSet<string> GetUniqueProcesses()
        {
            var uniqueProcessSet = new HashSet<string>();
            foreach (AddmlFlatFileDefinition flatFileDefinition in _addmlDefinition.AddmlFlatFileDefinitions)
            {
                uniqueProcessSet.UnionWith(flatFileDefinition.Processes);

                foreach (AddmlRecordDefinition recordDefinition in flatFileDefinition.AddmlRecordDefinitions)
                {
                    uniqueProcessSet.UnionWith(recordDefinition.Processes);

                    foreach (AddmlFieldDefinition fieldDefinition in recordDefinition.AddmlFieldDefinitions)
                    {
                        uniqueProcessSet.UnionWith(fieldDefinition.Processes);
                    }
                }
            }
            return uniqueProcessSet;
        }

        public Dictionary<IAddmlIndex, List<IAddmlProcess>> GetFileProcesses()
        {
            return GetProcessInstances(_addmlDefinition.GetFileProcessesGroupedByFile());
        }

        public Dictionary<IAddmlIndex, List<IAddmlProcess>> GetRecordProcesses()
        {
            return GetProcessInstances(_addmlDefinition.GetRecordProcessesGroupedByRecord());
        }

        public Dictionary<IAddmlIndex, List<IAddmlProcess>> GetFieldProcesses()
        {
            return GetProcessInstances(_addmlDefinition.GetFieldProcessesGroupedByField());
        }

        private Dictionary<IAddmlIndex, List<IAddmlProcess>> GetProcessInstances(Dictionary<IAddmlIndex, List<string>> processNamesGrouped)
        {
            var processInstancesByGroup = new Dictionary<IAddmlIndex, List<IAddmlProcess>>();

            foreach (var keyValuePair in processNamesGrouped)
            {
                var processesInstances = new List<IAddmlProcess>();
                List<string> processNames = keyValuePair.Value;
                foreach (string processName in processNames)
                {
                    if (_processesByName.ContainsKey(processName))
                    {
                        processesInstances.Add(_processesByName[processName]);
                    }
                    else
                    {
                        Log.Warning($"Process [{processName}] is not supported. No class found in process mapping.");
                    }
                }
                processInstancesByGroup.Add(keyValuePair.Key, processesInstances);
            }
            return processInstancesByGroup;
        }

        public List<IAddmlProcess> GetAllProcesses()
        {
            return _processesByName.Values.ToList();
        }

        internal List<IAddmlProcess> GetProcesses(IAddmlIndex key, Dictionary<IAddmlIndex, List<IAddmlProcess>> cachedProcesses)
        {
            if (cachedProcesses.ContainsKey(key))
            {
                return cachedProcesses[key];
            }
            return new List<IAddmlProcess>(0);
        }
    }
}