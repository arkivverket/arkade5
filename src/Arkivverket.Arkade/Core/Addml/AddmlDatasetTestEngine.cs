using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Logging;

namespace Arkivverket.Arkade.Core.Addml
{
    public class AddmlDatasetTestEngine : ITestEngine
    {
        private readonly AddmlProcessRunner _addmlProcessRunner;
        private readonly IStatusEventHandler _statusEventHandler;
        private readonly FlatFileReaderFactory _flatFileReaderFactory;

        public AddmlDatasetTestEngine(FlatFileReaderFactory flatFileReaderFactory, AddmlProcessRunner addmlProcessRunner, IStatusEventHandler statusEventHandler)
        {
            _flatFileReaderFactory = flatFileReaderFactory;
            _addmlProcessRunner = addmlProcessRunner;
            _statusEventHandler = statusEventHandler;
        }
        
        public TestSuite RunTestsOnArchive(TestSession testSession)
        {
            AddmlDefinition addmlDefinition = testSession.AddmlDefinition;

            _addmlProcessRunner.Init(addmlDefinition);

            List<FlatFile> flatFiles = addmlDefinition.GetFlatFiles();

            foreach (FlatFile file in flatFiles)
            {
                string testName = string.Format(Resources.Messages.RunningAddmlProcessesOnFile, file.GetName());
                _statusEventHandler.RaiseEventFileProcessingStarted(new FileProcessingStatusEventArgs(testName, file.GetName()));

                _addmlProcessRunner.RunProcesses(file);

                IRecordEnumerator recordEnumerator = _flatFileReaderFactory.GetRecordEnumerator(testSession.Archive, file);

                while (recordEnumerator.MoveNext())
                {
                    Record record = recordEnumerator.Current;
                    _statusEventHandler.RaiseEventRecordProcessingStart();
                    _addmlProcessRunner.RunProcesses(file, record);

                    foreach (Field field in record.Fields)
                    {
                        _addmlProcessRunner.RunProcesses(field);
                    }
                    _statusEventHandler.RaiseEventRecordProcessingStopped();

                }
                _addmlProcessRunner.EndOfFile();

                _statusEventHandler.RaiseEventFileProcessingFinished(new FileProcessingStatusEventArgs(testName, file.GetName(), true));
            }

            return _addmlProcessRunner.GetTestSuite();
        }
    }
}