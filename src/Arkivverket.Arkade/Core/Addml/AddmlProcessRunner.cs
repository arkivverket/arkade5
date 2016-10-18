using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Addml.Processes;

namespace Arkivverket.Arkade.Core.Addml
{
    public class AddmlProcessRunner
    {
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

            if (_fileProcesses.ContainsKey(file.Definition.FileName))
            {
                return _fileProcesses[file.Definition.FileName];
            }
            else
            {
                List<string> fileProcesses = file.GetFileProcesses();
                // instantiate new classes
                // save to dictionary
            }

            return new List<IAddmlProcess>();
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