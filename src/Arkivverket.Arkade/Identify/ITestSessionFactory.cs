using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Identify
{
    public interface ITestSessionFactory
    {
        TestSession NewSessionFromTarFile(string archiveFileName, string metadataFileName);
    }
}