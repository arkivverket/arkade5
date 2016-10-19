using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Addml
{
    public class AddmlProcessRunner
    {
        private readonly FieldProcessRunner _fieldProcessRunner;
        private readonly FileProcessRunner _fileProcessRunner;
        private readonly ProcessFactory _processFactory;
        private readonly RecordProcessRunner _recordProcessRunner;

        public AddmlProcessRunner(AddmlDefinition addmlDefinition)
        {
            _processFactory = new ProcessFactory(addmlDefinition);
            _fileProcessRunner = new FileProcessRunner(_processFactory);
            _recordProcessRunner = new RecordProcessRunner(_processFactory);
            _fieldProcessRunner = new FieldProcessRunner(_processFactory);
        }

        public void RunProcesses(FlatFile file)
        {
            _fileProcessRunner.RunProcesses(file);
        }

        public void RunProcesses(Record record)
        {
            _recordProcessRunner.RunProcesses(record);
        }

        public void RunProcesses(Field field)
        {
            _fieldProcessRunner.RunProcesses(field);
        }

        public TestSuite GetTestSuite()
        {
            var testSuite = new TestSuite();

            foreach (IAddmlProcess addmlProcess in _processFactory.GetAllProcesses())
            {
                testSuite.AddTestRun(addmlProcess.GetTestRun());
            }

            return testSuite;
        }

        public void EndOfFile()
        {
            foreach (IAddmlProcess addmlProcess in _processFactory.GetAllProcesses())
            {
                addmlProcess.EndOfFile();
            }
        }
    }
}