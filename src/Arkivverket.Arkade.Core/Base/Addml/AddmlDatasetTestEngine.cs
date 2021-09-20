using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes.Hardcoded;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    public class AddmlDatasetTestEngine : ITestEngine
    {
        private readonly AddmlProcessRunner _addmlProcessRunner;
        private readonly FlatFileReaderFactory _flatFileReaderFactory;
        private readonly IStatusEventHandler _statusEventHandler;
        private readonly List<TestResult> _testResultsFailedRecordsList = new List<TestResult>();
        private const int MaxNumberOfSingleReportedFieldDelimiterErrors = 25;
        private readonly ITestProgressReporter _testProgressReporter;

        public AddmlDatasetTestEngine(FlatFileReaderFactory flatFileReaderFactory, AddmlProcessRunner addmlProcessRunner,
            IStatusEventHandler statusEventHandler, ITestProgressReporter testProgressReporter)
        {
            _flatFileReaderFactory = flatFileReaderFactory;
            _addmlProcessRunner = addmlProcessRunner;
            _statusEventHandler = statusEventHandler;
            _testProgressReporter = testProgressReporter;
        }

        public TestSuite RunTestsOnArchive(TestSession testSession, ApiClient? apiClient = null)
        {
            _testProgressReporter.Begin(testSession.Archive.ArchiveType);

            AddmlDefinition addmlDefinition = testSession.AddmlDefinition;

            _addmlProcessRunner.Init(addmlDefinition);

            List<FlatFile> flatFiles = addmlDefinition.GetFlatFiles();
            
            foreach (FlatFile file in flatFiles)
            {
                string testName = string.Format(Messages.RunningAddmlProcessesOnFile, file.GetName());

                var recordIdx = 1;

                _statusEventHandler.RaiseEventFileProcessingStarted(
                    new FileProcessingStatusEventArgs(testName, file.GetName())
                );

                _addmlProcessRunner.RunProcesses(file);

                IRecordEnumerator recordEnumerator =
                    _flatFileReaderFactory.GetRecordEnumerator(testSession.Archive, file);

                int numberOfRecordsWithFieldDelimiterError = 0;

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
                        numberOfRecordsWithFieldDelimiterError++;

                        if (numberOfRecordsWithFieldDelimiterError <= MaxNumberOfSingleReportedFieldDelimiterErrors)
                        {
                            _testResultsFailedRecordsList.Add(new TestResult(ResultType.Error,
                                new AddmlLocation(file.GetName(), exception.RecordName, ""),
                                string.Format(AddmlMessages.FailedRecordErrorMessage, exception.Message, exception.RecordData))
                            );

                            _statusEventHandler.RaiseEventOperationMessage(
                                string.Format(AddmlMessages.FailedRecordErrorIdentifier, file.GetName(), recordIdx, numberOfRecordsWithFieldDelimiterError),
                                string.Format(AddmlMessages.FailedRecordErrorMessage, exception.Message, exception.RecordData),
                                OperationMessageStatus.Error
                            );
                        }
                        else
                        {
                            _statusEventHandler.RaiseEventOperationMessage(
                                string.Format(AddmlMessages.NumberOfRecordsWithFieldDelimiterErrorIdentifier, file.GetName()),
                                string.Format(AddmlMessages.TotalAmountText, numberOfRecordsWithFieldDelimiterError),
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

                if (numberOfRecordsWithFieldDelimiterError > 0)
                {
                    _testResultsFailedRecordsList.Add(new TestResult(ResultType.ErrorGroup, new Location(file.GetName()),
                        string.Format(AddmlMessages.TotalAmountOfRecordsWithFieldDelimiterErrorResultText, numberOfRecordsWithFieldDelimiterError),
                        numberOfRecordsWithFieldDelimiterError)
                    );
                }

                _addmlProcessRunner.EndOfFile(file);

                _statusEventHandler.RaiseEventFileProcessingFinished(
                    new FileProcessingStatusEventArgs(testName, file.GetName(), true)
                );
            }

            TestSuite testSuite = _addmlProcessRunner.GetTestSuite();

            testSuite.AddTestRun(new AH_02_ControlExtraOrMissingFiles(addmlDefinition, testSession.Archive).GetTestRun());
            testSuite.AddTestRun(new AH_03_ControlRecordAndFieldDelimiters(_testResultsFailedRecordsList).GetTestRun());

            _testProgressReporter.Finish();

            return testSuite;
        }
    }
}
