using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Testing.Siard;

namespace Arkivverket.Arkade.Core.Base.Siard
{
    public class SiardTestEngine : ITestEngine
    {
        private readonly IStatusEventHandler _statusEventHandler;
        private readonly ITestProgressReporter _testProgressReporter;

        public SiardTestEngine(IStatusEventHandler statusEventHandler, ITestProgressReporter testProgressReporter)
        {
            _statusEventHandler = statusEventHandler;
            _testProgressReporter = testProgressReporter;
        }

        public TestSuite RunTestsOnArchive(TestSession testSession)
        {
            _statusEventHandler.RaiseEventOperationMessage(Resources.Messages.ValidatingExtractMessage, null, OperationMessageStatus.Started);
            _testProgressReporter.Begin(ArchiveType.Siard);

            bool validationHasCompleted = RunSiardValidation(testSession);

            _testProgressReporter.Finish(!validationHasCompleted);

            return new TestSuite();
        }

        private bool RunSiardValidation(TestSession testSession)
        {
            FileInfo siardFileInfo = testSession.Archive.WorkingDirectory.Content().DirectoryInfo().GetFiles()
                .First(f => f.Extension.Equals(".siard"));
            string inputFilePath = siardFileInfo.FullName;
            string reportFilePath = Path.Combine(testSession.Archive.WorkingDirectory.RepositoryOperations().ToString(),
                Resources.OutputFileNames.DbptkValidationReportFile);

            _statusEventHandler.RaiseEventOperationMessage(
                Resources.SiardMessages.ValidationMessageIdentifier,
                string.Format(Resources.SiardMessages.ValidationMessage, siardFileInfo.Name),
                OperationMessageStatus.Info);

            (List<string> results, List<string> errors) = SiardValidator.Validate(inputFilePath, reportFilePath);

            List<string> summary = results.Where(r => r != null && r.Contains("number of", StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (summary.Any())
            {
                testSession.TestSummary = new TestSummary
                {
                    NumberOfErrors = GetNumberOfXFromSummary("errors", summary),
                    NumberOfWarnings = GetNumberOfXFromSummary("warnings", summary)
                };
            }

            _statusEventHandler.RaiseEventSiardValidationFinished(errors);

            bool validationRanWithoutRunErrors = errors.All(e => e == null);

            return validationRanWithoutRunErrors;
        }

        private string GetNumberOfXFromSummary(string x, List<string> summary)
        {
            var regex = new Regex(@"(?!\[)\d+(?=\])");

            return regex.Match(summary.First(s => s.Contains(x, StringComparison.InvariantCultureIgnoreCase))).Value;
        }
    }
}
