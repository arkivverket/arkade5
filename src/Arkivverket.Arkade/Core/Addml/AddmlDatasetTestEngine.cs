using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml
{
    public class AddmlDatasetTestEngine : ITestEngine
    {
        private readonly AddmlProcessRunner _addmlProcessRunner;
        private readonly FlatFileReaderFactory _flatFileReaderFactory;

        public AddmlDatasetTestEngine(FlatFileReaderFactory flatFileReaderFactory, AddmlProcessRunner addmlProcessRunner)
        {
            _flatFileReaderFactory = flatFileReaderFactory;
            _addmlProcessRunner = addmlProcessRunner;
        }

        public event EventHandler<TestFinishedEventArgs> TestFinished;
        public event EventHandler<TestStartedEventArgs> TestStarted;

        protected virtual void OnTestFinished(TestFinishedEventArgs e)
        {
            var handler = TestFinished;
            handler?.Invoke(this, e);
        }

        protected virtual void OnTestStarted(TestStartedEventArgs e)
        {
            var handler = TestStarted;
            handler?.Invoke(this, e);
        }

        public TestSuite RunTestsOnArchive(TestSession testSession)
        {
            AddmlDefinition addmlDefinition = testSession.AddmlDefinition;

            _addmlProcessRunner.Init(addmlDefinition);

            List<FlatFile> flatFiles = addmlDefinition.GetFlatFiles();

            foreach (FlatFile file in flatFiles)
            {
                string testName = "ADDML-prosesser på filen: " + file.GetName();
                OnTestStarted(new TestStartedEventArgs(testName));
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
                
                OnTestFinished(new TestFinishedEventArgs(new TestRun(testName, TestType.Content)));
            }

            return _addmlProcessRunner.GetTestSuite();
        }
    }
}