using System.Reflection;
using Arkivverket.Arkade.Core.Base;
using Serilog;

namespace Arkivverket.Arkade.Core.Logging
{
    public class GuiTestProgressReporter : ITestProgressReporter
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly IStatusEventHandler _statusEventHandler;

        private int _previousProgressPercentage;
        private string _testProgressUnit;

        public GuiTestProgressReporter(IStatusEventHandler statusEventHandler)
        {
            _statusEventHandler = statusEventHandler;
        }

        public void Begin(ArchiveType archiveType)
        {
            _testProgressUnit = GetTestProgressUnit(archiveType);

            Log.Information($"Running {archiveType}-validation: ");
            _statusEventHandler.RaiseEventTestProgressUpdated($"0 {_testProgressUnit}");
            _previousProgressPercentage = 0;
        }

        private string GetTestProgressUnit(ArchiveType archiveType)
        {
            return archiveType switch
            {
                ArchiveType.Noark5 => "%",
                ArchiveType.Fagsystem => "records",
                ArchiveType.Noark3 => "records",
                _ => ""
            };
        }

        public void ReportTestProgress(int testProgressValue)
        {
            if (_previousProgressPercentage != testProgressValue)
            {
                _statusEventHandler.RaiseEventTestProgressUpdated($"{testProgressValue} {_testProgressUnit}");
                _previousProgressPercentage = testProgressValue;
            }
        }

        public void Finish()
        {
            _statusEventHandler.RaiseEventTestProgressUpdated("Done!");
        }
    }
}
