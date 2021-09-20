using System.IO;
using System.Linq;
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
            _testProgressReporter.Begin(ArchiveType.Siard);

            FileInfo siardFileInfo = testSession.Archive.WorkingDirectory.Content().DirectoryInfo().GetFiles()
                .First(f => f.Extension.Equals(".siard"));
            string inputFilePath = siardFileInfo.FullName;
            string reportFilePath = Path.Combine(testSession.Archive.WorkingDirectory.RepositoryOperations().ToString(),
                Resources.OutputFileNames.SiardValidationReportFile);

            _statusEventHandler.RaiseEventOperationMessage(
                Resources.SiardMessages.ValidationMessageIdentifier,
                string.Format(Resources.SiardMessages.ValidationMessage, siardFileInfo.Name),
                OperationMessageStatus.Info);

            SiardValidator.Validate(inputFilePath, reportFilePath);

            _testProgressReporter.Finish();

            return new TestSuite();
        }
    }
}
