using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Identify
{
    public interface ITestSessionBuilder
    {
        TestSession NewSessionFromTarFile(string archiveFileName, string metadataFileName);
    }
}