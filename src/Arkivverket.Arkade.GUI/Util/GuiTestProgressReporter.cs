using System.Reflection;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.GUI.Languages;
using Serilog;

namespace Arkivverket.Arkade.GUI.Util
{
    internal class GuiTestProgressReporter : ITestProgressReporter
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly IStatusEventHandler _statusEventHandler;

        private int _previousProgressPercentage;
        private ArchiveType _archiveType;

        public GuiTestProgressReporter(IStatusEventHandler statusEventHandler)
        {
            _statusEventHandler = statusEventHandler;
        }

        public void Begin(ArchiveType archiveType)
        {
            _archiveType = archiveType;
            Log.Information($"Running {archiveType}-validation: ");

            _statusEventHandler.RaiseEventTestProgressUpdated(archiveType is ArchiveType.Siard
                ? TestRunnerGUI.SiardProgressMessage
                : "0 %");

            _previousProgressPercentage = 0;
        }

        public void ReportTestProgress(int testProgressValue)
        {
            if (_previousProgressPercentage != testProgressValue)
            {
                _statusEventHandler.RaiseEventTestProgressUpdated($"{testProgressValue} %");
                _previousProgressPercentage = testProgressValue;
            }
        }

        public void Finish()
        {
            _statusEventHandler.RaiseEventTestProgressUpdated(_archiveType is ArchiveType.Siard
            ? TestRunnerGUI.MessageCompleted
            : "100 %");
        }
    }
}
