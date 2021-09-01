using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Testing.Siard;

namespace Arkivverket.Arkade.Core.Base.Siard
{
    public class SiardTestEngine : ITestEngine
    {
        private readonly IStatusEventHandler _statusEventHandler;

        public SiardTestEngine(IStatusEventHandler statusEventHandler)
        {
            _statusEventHandler = statusEventHandler;
        }

        public TestSuite RunTestsOnArchive(TestSession testSession)
        {
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

            return new TestSuite();
        }
    }
}
