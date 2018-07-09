using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Identify
{
    public interface ITestSessionFactory
    {
        TestSession NewSession(ArchiveDirectory archiveDirectory);

        TestSession NewSession(ArchiveFile archiveFile);

    }
}