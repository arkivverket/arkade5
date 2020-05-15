using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Base.Addml.Processes.Hardcoded;
using Arkivverket.Arkade.Core.Base.Addml.Processes.Internal;
using Arkivverket.Arkade.Core.Logging;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    public class AddmlProcessRunner
    {
        private FieldProcessRunner _fieldProcessRunner;
        private FileProcessRunner _fileProcessRunner;
        private ProcessManager _processManager;
        private RecordProcessRunner _recordProcessRunner;
        private readonly IStatusEventHandler _statusEventHandler;

        public AddmlProcessRunner(IStatusEventHandler statusEventHandler)
        {
            _statusEventHandler = statusEventHandler;
        }

        public void Init(AddmlDefinition addmlDefinition)
        {
            _processManager = new ProcessManager(addmlDefinition, _statusEventHandler);
            _fileProcessRunner = new FileProcessRunner(_processManager);
            _recordProcessRunner = new RecordProcessRunner(_processManager);
            _fieldProcessRunner = new FieldProcessRunner(_processManager);
        }

        public void RunProcesses(FlatFile file)
        {
            _fileProcessRunner.RunProcesses(file);
            _processManager.GetProcessInstanceByName(AH_01_ControlChecksum.Name).Run(file);
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
                if (addmlProcess.GetType() == typeof(A_16_ControlForeignKey))
                {
                    LoadCollectedPrimaryKeysIntoControlForeignKeyProcess(addmlProcess);
                }

                // CollectPrimaryKey is internal and should not be part of the test report
                if (addmlProcess.GetType() != typeof(AI_01_CollectPrimaryKey))
                    testSuite.AddTestRun(addmlProcess.GetTestRun());
            }

            return testSuite;
        }

        private void LoadCollectedPrimaryKeysIntoControlForeignKeyProcess(IAddmlProcess addmlProcess)
        {
            AI_01_CollectPrimaryKey collectPrimaryKeyProcess = (AI_01_CollectPrimaryKey) _processManager.GetProcessInstanceByName(AI_01_CollectPrimaryKey.Name);

            ((A_16_ControlForeignKey) addmlProcess).GetCollectedPrimaryKeys(collectPrimaryKeyProcess);
        }

        public void EndOfFile(FlatFile file)
        {
            _fileProcessRunner.EndOfFile(file);
            _recordProcessRunner.EndOfFile(file);
            _fieldProcessRunner.EndOfFile(file);
        }
        
    }
}