using System.Collections.Generic;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Logging;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core.Addml
{
    public class AddmlDatasetTestEngine : ITestEngine
    {
        private readonly AddmlProcessRunner _addmlProcessRunner;
        private readonly FlatFileReaderFactory _flatFileReaderFactory;
        private readonly IStatusEventHandler _statusEventHandler;
        private readonly List<TestResult> _testResultsFailedRecordsList = new List<TestResult>();
        private int _numberOfRecordsWithFieldDelimiterError;
        private const int MaxNumberOfSingleReportedFieldDelimiterErrors = 99;

        public AddmlDatasetTestEngine(FlatFileReaderFactory flatFileReaderFactory, AddmlProcessRunner addmlProcessRunner,
            IStatusEventHandler statusEventHandler)
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
                string testName = string.Format(Messages.RunningAddmlProcessesOnFile, file.GetName());

                var recordIdx = 0;

                _statusEventHandler.RaiseEventFileProcessingStarted(
                    new FileProcessingStatusEventArgs(testName, file.GetName())
                );

                _addmlProcessRunner.RunProcesses(file);

                IRecordEnumerator recordEnumerator =
                    _flatFileReaderFactory.GetRecordEnumerator(testSession.Archive, file);

                while (recordEnumerator != null && recordEnumerator.MoveNext())
                {
                    try
                    {
                        _statusEventHandler.RaiseEventRecordProcessingStart();
                        Record record = recordEnumerator.Current;
                        _addmlProcessRunner.RunProcesses(file, record);

                        foreach (Field field in record.Fields)
                            _addmlProcessRunner.RunProcesses(file, field);
                    }
                    catch (ArkadeAddmlDelimiterException exception)
                    {
                        _numberOfRecordsWithFieldDelimiterError++;

                        if (_numberOfRecordsWithFieldDelimiterError <= MaxNumberOfSingleReportedFieldDelimiterErrors)
                        {
                            _testResultsFailedRecordsList.Add(new TestResult(ResultType.Error,
                                new AddmlLocation(file.GetName(), exception.RecordName, ""),
                                exception.Message + " Felttekst: " + exception.RecordData)
                            );

                            _statusEventHandler.RaiseEventOperationMessage(
                                $"{AddmlMessages.RecordLengthErrorTestName} i fil {file.GetName()}, post nummer {recordIdx}, feil nummer {_numberOfRecordsWithFieldDelimiterError}",
                                exception.Message + " Felttekst: " + exception.RecordData, OperationMessageStatus.Error
                            );
                        }
                        else
                        {
                            _statusEventHandler.RaiseEventOperationMessage(
                                $"ADDML-poster med feil antall felt i filen {file.GetName()}",
                                $"Totalt antall: {_numberOfRecordsWithFieldDelimiterError}",
                                OperationMessageStatus.Error
                            );
                        }
                    }
                    finally
                    {
                        _statusEventHandler.RaiseEventRecordProcessingStopped();
                    }

                    recordIdx++;
                }

                _testResultsFailedRecordsList.Add(new TestResult(ResultType.Error, new Location(file.GetName()),
                    $"Filens totale antall poster med feil antall felt: {_numberOfRecordsWithFieldDelimiterError}")
                );

                _addmlProcessRunner.EndOfFile(file);

                _statusEventHandler.RaiseEventFileProcessingFinished(
                    new FileProcessingStatusEventArgs(testName, file.GetName(), true)
                );
            }

            TestSuite testSuite = _addmlProcessRunner.GetTestSuite();

            var p = new HardcodedProcessRunner(addmlDefinition, testSession.Archive);
            p.Run().ForEach(t => testSuite.AddTestRun(t));

            var failedRecoredTestRun = new TestRun(AddmlMessages.RecordLengthErrorTestName, TestType.Structure)
            {
                Results = _testResultsFailedRecordsList
            };

            testSuite.AddTestRun(failedRecoredTestRun);

            return testSuite;
        }
    }
}
