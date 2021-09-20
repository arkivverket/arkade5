using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Base.Addml.Processes.Hardcoded;
using Arkivverket.Arkade.Core.Base.Addml.Processes.Internal;
using Arkivverket.Arkade.Core.Logging;
using Serilog;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    public class ProcessManager
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<ProcessManager>();

        private static readonly Mutex Mutex = new(false, "ConsoleCursorPosition - 183f9057-3fd1-4d58-a69b-79ed60f43cfc");

        private readonly AddmlDefinition _addmlDefinition;
        private readonly IStatusEventHandler _statusEventHandler;
        private readonly Dictionary<string, IAddmlProcess> _processesByName;
        private readonly ProcessTypeMapping _processTypeMapping = new ProcessTypeMapping();

        public ProcessManager(AddmlDefinition addmlDefinition, IStatusEventHandler statusEventHandler)
        {
            _addmlDefinition = addmlDefinition;
            _statusEventHandler = statusEventHandler;
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
                    Mutex.WaitOne();
                    Log.Warning($"No process with name {processName} in ProcessTypeMapping.");
                    Mutex.ReleaseMutex();

                    _statusEventHandler.RaiseEventOperationMessage(
                        string.Format(Resources.AddmlMessages.UnknownAddmlProcess, processName),
                        string.Format(Resources.AddmlMessages.CouldNotInstatiateUnsupportedAddmlProcess, processName),
                        OperationMessageStatus.Warning
                    );
                }
            }
            return processes;
        }

        private HashSet<string> GetUniqueProcesses()
        {
            var uniqueProcessSet = new HashSet<string>();

            AddDefaultProcesses(uniqueProcessSet);

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

        private void AddDefaultProcesses(HashSet<string> processes)
        {
            processes.Add(AI_01_CollectPrimaryKey.Name);
            processes.Add(AH_01_ControlChecksum.Name);
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
                        Mutex.WaitOne();
                        Log.Warning($"Process [{processName}] is not supported. No class found in process mapping.");
                        Mutex.ReleaseMutex();
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

        public IAddmlProcess GetProcessInstanceByName(string name)
        {
            if (_processesByName.ContainsKey(name))
            {
                return _processesByName[name];
            }
            return null;
        }
    }
}