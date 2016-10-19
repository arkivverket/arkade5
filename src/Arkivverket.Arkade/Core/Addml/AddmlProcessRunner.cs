using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Addml.Processes;
using Serilog;

namespace Arkivverket.Arkade.Core.Addml
{
    // TODO: Split into FlatFileProcessRunner, RecordProcessRunner and FieldProcessRunner
    public class AddmlProcessRunner
    {
        private readonly ProcessTypeMapping _processTypeMapping = new ProcessTypeMapping();

        private Dictionary<string, List<IAddmlProcess>> _processes = new Dictionary<string, List<IAddmlProcess>>();

        public void RunProcesses(FlatFile file)
        {
            List<IAddmlProcess> processes = GetProcesses(file);

            IEnumerable<IAddmlFileProcess> addmlFileProcesses =
                processes.Where(p => p.GetType().GetInterfaces().Contains(typeof(IAddmlFileProcess)))
                    .Select(p => p as IAddmlFileProcess);

            foreach (IAddmlFileProcess addmlFileProcess in addmlFileProcesses)
            {
                addmlFileProcess.Run(file);
            }
        }

        public void RunProcesses(Record record)
        {
        }

        public void RunProcesses(Field field)
        {
            List<IAddmlProcess> processes = GetProcesses(field);

            IEnumerable<IAddmlFieldProcess> addmlFieldProcesses = processes
                .Where(p => p.GetType().GetInterfaces()
                    .Contains(typeof(IAddmlFieldProcess)))
                .Select(p => p as IAddmlFieldProcess);

            foreach (IAddmlFieldProcess p in addmlFieldProcesses)
            {
                p.Run(field);
            }
        }

        private List<IAddmlProcess> GetProcesses(HasProcesses hasProcesses)
        {
            string name = hasProcesses.GetName();
            if (!_processes.ContainsKey(name))
            {
                List<IAddmlProcess> processes = new List<IAddmlProcess>();

                foreach (string processName in hasProcesses.GetProcesses())
                {
                    Type type = _processTypeMapping.GetType(processName);
                    if (type != null)
                    {
                        IAddmlProcess process = (IAddmlProcess)Activator.CreateInstance(type);
                        processes.Add(process);
                    }
                    else
                    {
                        Log.Logger.Warning("No process with name " + processName);
                    }
                }

                _processes.Add(name, processes);
            }

            return _processes[name];
        }


        public TestSuite GetTestSuite()
        {
            TestSuite testSuite = new TestSuite();

            foreach (KeyValuePair<string, List<IAddmlProcess>> entry in _processes)
            {
                foreach (IAddmlProcess addmlProcess in entry.Value)
                {
                    testSuite.AddTestRun(addmlProcess.GetTestRun());
                }
            }

            return testSuite;
        }

        public void EndOfFile()
        {
            foreach (KeyValuePair<string, List<IAddmlProcess>> entry in _processes)
            {
                foreach (IAddmlProcess addmlProcess in entry.Value)
                {
                    addmlProcess.EndOfFile();
                }
            }

        }
    }
}