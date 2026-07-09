using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;

namespace Arkivverket.Arkade.Core.Logging
{
    public interface ITestProgressReporter
    {
        void Begin(ArchiveType archiveType);
        void ReportTestProgress(int testProgressValue);
        void Finish(bool hasFailed = false);
    }
}