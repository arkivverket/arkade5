using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Identify
{
    public interface ITestSessionFactory
    {
        TestSession NewSessionFromArchiveDirectory(ArchiveDirectory archive);

        TestSession NewSessionFromArchiveFile(ArchiveFile archive);

    }
}