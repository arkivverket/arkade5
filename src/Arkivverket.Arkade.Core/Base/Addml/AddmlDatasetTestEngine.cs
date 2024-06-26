using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Addml.Processes.Hardcoded;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Testing;

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

        public TestSuite RunTestsOnArchive(TestSession testSession)
        {
            _testProgressReporter.Begin(testSession.Archive.ArchiveType);

            AddmlDefinition addmlDefinition = testSession.AddmlDefinition;

            _addmlProcessRunner.Init(addmlDefinition);

            List<FlatFile> flatFiles = addmlDefinition.GetFlatFiles();

            var useNumberOfRecords = true;
            int? numberOfRecords = 0;

            foreach (FlatFile file in flatFiles)
            {
                if (file.Definition.NumberOfRecords == null)
                {
                    useNumberOfRecords = false;
                    break;
                }
                numberOfRecords += file.Definition.NumberOfRecords;
            }

            long numberOfProcessedRecords = 0;
            double fileCounter = 0;

            foreach (FlatFile file in flatFiles)
            {
                fileCounter++;
                string testName = string.Format(Messages.RunningAddmlProcessesOnFile, file.GetName());

                _statusEventHandler.RaiseEventFileProcessingStarted(
                    new FileProcessingStatusEventArgs(testName, file.GetName())
                );

                _addmlProcessRunner.RunProcesses(file);

                IRecordEnumerator recordEnumerator =
                    _flatFileReaderFactory.GetRecordEnumerator(testSession.Archive, file);

                int numberOfRecordsWithFieldDelimiterError = 0;

                while (recordEnumerator != null)
                {
                    try
                    {
                        if (!recordEnumerator.MoveNext())
                            break;
                        
                        Record record = recordEnumerator.Current;
                        if (record.LineNumber <= record.Definition.HeaderLevel)
                            continue;

                        _statusEventHandler.RaiseEventRecordProcessingStart();
                        _addmlProcessRunner.RunProcesses(file, record);

                        foreach (Field field in record.Fields)
                            _addmlProcessRunner.RunProcesses(file, field, record.LineNumber);
                    }
                    catch (ArkadeAddmlDelimiterException exception)
                    {
                        numberOfRecordsWithFieldDelimiterError++;

                        if (numberOfRecordsWithFieldDelimiterError <= MaxNumberOfSingleReportedFieldDelimiterErrors)
                        {
                            string failedRecordErrorMessage = string.Format(AddmlMessages.FailedRecordErrorMessage, exception.Message, exception.RecordData);

                            _testResultsFailedRecordsList.Add(new TestResult(ResultType.Error,
                                new Location(file.GetName() + (string.IsNullOrWhiteSpace(exception.RecordName)
                                    ? " - " + exception.RecordName
                                    : string.Empty), int.Parse(exception.RecordNumber)),
                                failedRecordErrorMessage)
                            );

                            _statusEventHandler.RaiseEventOperationMessage(
                                string.Format(AddmlMessages.FailedRecordErrorIdentifier, file.GetName(), recordEnumerator.RecordNumber, numberOfRecordsWithFieldDelimiterError),
                                failedRecordErrorMessage,
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

                    numberOfProcessedRecords++;

                    if (useNumberOfRecords)
                        _testProgressReporter.ReportTestProgress((int)(numberOfProcessedRecords / numberOfRecords * 100));
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

                if (!useNumberOfRecords)
                    _testProgressReporter.ReportTestProgress((int)(fileCounter/ flatFiles.Count * 100));
            }

            TestSuite testSuite = _addmlProcessRunner.GetTestSuite();

            testSuite.AddTestRun(new AH_02_ControlExtraOrMissingFiles(addmlDefinition, testSession.Archive).GetTestRun());
            testSuite.AddTestRun(new AH_03_ControlRecordAndFieldDelimiters(_testResultsFailedRecordsList).GetTestRun());

            testSession.TestSummary = new TestSummary((int) fileCounter, numberOfProcessedRecords,
                testSuite.TestRuns.Count(), testSuite.FindNumberOfErrors(), 0);

            _testProgressReporter.Finish();

            return testSuite;
        }
    }
}
