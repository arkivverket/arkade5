using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Logging
{
    public interface ITestProgressReporter
    {
        bool IsRunning { get; }

        void Begin(ArchiveType archiveType);
        void ReportTestProgress(int testProgressValue);
        void Finish(bool hasFailed = false);
    }
}