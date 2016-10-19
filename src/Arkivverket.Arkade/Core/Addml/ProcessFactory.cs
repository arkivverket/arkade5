﻿using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Serilog;

namespace Arkivverket.Arkade.Core.Addml
{
    public class ProcessFactory
    {
        private readonly AddmlDefinition _addmlDefinition;
        private readonly Dictionary<string, IAddmlProcess> _processesByName;
        private readonly ProcessTypeMapping _processTypeMapping = new ProcessTypeMapping();

        public ProcessFactory(AddmlDefinition addmlDefinition)
        {
            _addmlDefinition = addmlDefinition;
            _processesByName = InstantiateProcesses();
        }

        private Dictionary<string, IAddmlProcess> InstantiateProcesses()
        {
            var processes = new Dictionary<string, IAddmlProcess>();
            HashSet<string> processNames = GetUniqueProcesses();
            foreach (string processName in processNames)
            {
                Type type = _processTypeMapping.GetType(processName);
                if (type != null)
                {
                    var process = (IAddmlProcess) Activator.CreateInstance(type);
                    processes.Add(processName, process);
                }
                else
                {
                    Log.Logger.Warning("No process with name " + processName);
                }
            }
            return processes;
        }

        private HashSet<string> GetUniqueProcesses()
        {
            var uniqueProcessSet = new HashSet<string>();
            foreach (AddmlFlatFileDefinition flatFileDefinition in _addmlDefinition.AddmlFlatFileDefinitions)
            {
                AddListOfProcessesToSet(uniqueProcessSet, flatFileDefinition.Processes);

                foreach (AddmlRecordDefinition recordDefinition in flatFileDefinition.AddmlRecordDefinitions)
                {
                    AddListOfProcessesToSet(uniqueProcessSet, recordDefinition.Processes);

                    foreach (AddmlFieldDefinition fieldDefinition in recordDefinition.AddmlFieldDefinitions)
                    {
                        AddListOfProcessesToSet(uniqueProcessSet, fieldDefinition.Processes);
                    }
                }
            }
            return uniqueProcessSet;
        }

        private void AddListOfProcessesToSet(HashSet<string> processSet, List<string> processList)
        {
            foreach (string processName in processList)
            {
                processSet.Add(processName);
            }
        }

        public Dictionary<string, List<IAddmlProcess>> GetFileProcesses()
        {
            return GetProcessInstances(_addmlDefinition.GetFileProcessesGroupedByFile());
        }

        public Dictionary<string, List<IAddmlProcess>> GetRecordProcesses()
        {
            return GetProcessInstances(_addmlDefinition.GetRecordProcessesGroupedByRecord());
        }

        public Dictionary<string, List<IAddmlProcess>> GetFieldProcesses()
        {
            return GetProcessInstances(_addmlDefinition.GetFieldProcessesGroupedByField());
        }

        private Dictionary<string, List<IAddmlProcess>> GetProcessInstances(Dictionary<string, List<string>> processNamesGrouped)
        {
            var processInstancesByGroup = new Dictionary<string, List<IAddmlProcess>>();

            foreach (string file in processNamesGrouped.Keys)
            {
                var processesInstances = new List<IAddmlProcess>();
                List<string> processNames = processNamesGrouped[file];
                foreach (string processName in processNames)
                {
                    if (_processesByName.ContainsKey(processName))
                    {
                        processesInstances.Add(_processesByName[processName]);
                    }
                    else
                    {
                        Log.Logger.Warning($"No class found for process [{processName}]");
                    }
                }
                processInstancesByGroup.Add(file, processesInstances);
            }
            return processInstancesByGroup;
        }

        public List<IAddmlProcess> GetAllProcesses()
        {
            return _processesByName.Values.ToList();
        }

        internal List<IAddmlProcess> GetProcesses(string key, Dictionary<string, List<IAddmlProcess>> cachedProcesses)
        {
            if (cachedProcesses.ContainsKey(key))
            {
                return cachedProcesses[key];
            }
            return new List<IAddmlProcess>(0);
        }
    }
}