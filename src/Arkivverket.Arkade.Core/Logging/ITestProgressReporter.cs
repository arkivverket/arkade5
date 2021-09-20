using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Logging
{
    public interface ITestProgressReporter
    {
        void Begin(ArchiveType archiveType);
        void ReportTestProgress(int testProgressValue);
        void Finish();
    }
}