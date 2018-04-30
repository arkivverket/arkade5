using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Processes;
using Arkivverket.Arkade.Core.Addml.Processes.Hardcoded;
using Arkivverket.Arkade.Core.Addml.Processes.Internal;

namespace Arkivverket.Arkade.Core.Addml
{
    public class AddmlProcessRunner
    {
        private FieldProcessRunner _fieldProcessRunner;
        private FileProcessRunner _fileProcessRunner;
        private ProcessManager _processManager;
        private RecordProcessRunner _recordProcessRunner;

        public void Init(AddmlDefinition addmlDefinition)
        {
            _processManager = new ProcessManager(addmlDefinition);
            _fileProcessRunner = new FileProcessRunner(_processManager);
            _recordProcessRunner = new RecordProcessRunner(_processManager);
            _fieldProcessRunner = new FieldProcessRunner(_processManager);
        }

        public void RunProcesses(FlatFile file)
        {
            _fileProcessRunner.RunProcesses(file);
            _processManager.GetProcessInstanceByName(ControlChecksum.Name).Run(file);
        }

        public void RunProcesses(FlatFile file, Record record)
        {
            _fileProcessRunner.RunProcesses(file, record);
            _recordProcessRunner.RunProcesses(record);
        }

        public void RunProcesses(FlatFile file, Field field)
        {
            _fileProcessRunner.RunProcesses(file, field);
            _fieldProcessRunner.RunProcesses(field);
        }

        public TestSuite GetTestSuite()
        {
            var testSuite = new TestSuite();

            foreach (IAddmlProcess addmlProcess in _processManager.GetAllProcesses())
            {
                // ControlForeignKey needs to access results of CollectPrimaryKey process
                // Consider moving this to ProcessManager.InstantiateProcesses()
                if (addmlProcess.GetType() == typeof(ControlForeignKey))
                {
                    LoadCollectedPrimaryKeysIntoControlForeignKeyProcess(addmlProcess);
                }

                // CollectPrimaryKey is internal and should not be part of the test report
                if (addmlProcess.GetType() != typeof(CollectPrimaryKey))
                    testSuite.AddTestRun(addmlProcess.GetTestRun());
            }

            return testSuite;
        }

        private void LoadCollectedPrimaryKeysIntoControlForeignKeyProcess(IAddmlProcess addmlProcess)
        {
            CollectPrimaryKey collectPrimaryKeyProcess = (CollectPrimaryKey) _processManager.GetProcessInstanceByName(CollectPrimaryKey.Name);

            ((ControlForeignKey) addmlProcess).GetCollectedPrimaryKeys(collectPrimaryKeyProcess);
        }

        public void EndOfFile(FlatFile file)
        {
            _fileProcessRunner.EndOfFile(file);
            _recordProcessRunner.EndOfFile(file);
            _fieldProcessRunner.EndOfFile(file);
        }
        
    }
}