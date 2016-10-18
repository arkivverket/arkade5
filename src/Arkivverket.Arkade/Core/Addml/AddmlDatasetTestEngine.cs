using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Addml
{
    public class AddmlDatasetTestEngine
    {
        private readonly AddmlProcessRunner _addmlProcessRunner;
        private readonly FlatFileReaderFactory _flatFileReaderFactory;

        public AddmlDatasetTestEngine(FlatFileReaderFactory flatFileReaderFactory, AddmlProcessRunner addmlProcessRunner)
        {
            _flatFileReaderFactory = flatFileReaderFactory;
            _addmlProcessRunner = addmlProcessRunner;
        }

        public TestSuite RunTests(AddmlDefinition addmlDefinition, TestSession testSession)
        {

            List<FlatFile> flatFiles = addmlDefinition.GetFlatFiles();

            foreach (FlatFile file in flatFiles)
            {
                _addmlProcessRunner.RunProcesses(file);

                IFlatFileReader flatFileReader = _flatFileReaderFactory.GetReader(testSession.Archive, file);

                while (flatFileReader.HasMoreRecords())
                {
                    Record record = flatFileReader.GetNextRecord();

                    _addmlProcessRunner.RunProcesses(record);

                    foreach (Field field in record.Fields)
                    {
                        _addmlProcessRunner.RunProcesses(field);
                    }
                }
                _addmlProcessRunner.EndOfFile();
            }

            return _addmlProcessRunner.GetTestSuite();
        }
    }
}