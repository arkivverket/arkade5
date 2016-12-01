using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Identify
{
    public interface ITestSessionFactory
    {
        TestSession NewSession(ArchiveDirectory archiveDirectory);

        TestSession NewSession(ArchiveFile archiveFile);

    }
}