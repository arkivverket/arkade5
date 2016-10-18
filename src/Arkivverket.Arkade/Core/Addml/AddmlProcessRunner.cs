using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Addml.Processes;

namespace Arkivverket.Arkade.Core.Addml
{
    public class AddmlProcessRunner
    {
        private readonly ProcessTypeMapping _processTypeMapping = new ProcessTypeMapping();

        private Dictionary<string, List<IAddmlProcess>> _fileProcesses = new Dictionary<string, List<IAddmlProcess>>();

        public void RunProcesses(FlatFile file)
        {
            List<IAddmlProcess> processes = GetProcesses(file);

            IEnumerable<IAddmlFileProcess> addmlFileProcesses = processes.Where(p => p.GetType().GetInterfaces().Contains(typeof(IAddmlFileProcess))).Select(p => p as IAddmlFileProcess);

            foreach (IAddmlFileProcess addmlFileProcess in addmlFileProcesses)
            {
                addmlFileProcess.Run(file);
            }
        }

        private List<IAddmlProcess> GetProcesses(FlatFile file)
        {
            if (!_fileProcesses.ContainsKey(file.Definition.FileName))
            {
                List<IAddmlProcess> processes = new List<IAddmlProcess>();

                // instantiate new classes
                List<string> fileProcesses = file.GetFileProcesses();
                foreach (string fileProcess in fileProcesses)
                {
                    Type type = _processTypeMapping.GetType(fileProcess);
                    IAddmlProcess process = (IAddmlProcess) Activator.CreateInstance(type);
                    processes.Add(process);
                }

                // save to dictionary
                _fileProcesses.Add(file.Definition.FileName, processes);
            }

            return _fileProcesses[file.Definition.FileName];
        }

        public void RunProcesses(Record record)
        {

        }

        public void RunProcesses(Field field)
        {
        }

        public TestSuite GetTestSuite()
        {
            return new TestSuite();
        }

        public void EndOfFile()
        {

        }
    }
}