using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml;

namespace Arkivverket.Arkade.Core
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
            IFlatFileReader flatFileReader = _flatFileReaderFactory.GetReader(testSession.Archive);

            List<FlatFile> flatFiles = addmlDefinition.GetFlatFiles();

            foreach (FlatFile file in flatFiles)
            {
                _addmlProcessRunner.RunProcesses(file);

                while (flatFileReader.HasMoreRecords())
                {
                    Record record = flatFileReader.GetNextRecord();

                    _addmlProcessRunner.RunProcesses(record);

                    foreach (Field field in record.Fields)
                    {
                        _addmlProcessRunner.RunProcesses(field);
                    }
                }
            }

            return _addmlProcessRunner.GetTestSuite();
        }
    }
}