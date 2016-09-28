using Arkivverket.Arkade.Core;
using Serilog;

namespace Arkivverket.Arkade.Identify
{
    public class TestSessionBuilder : ITestSessionBuilder
    {
        private readonly IArchiveExtractor _archiveExtractor;
        private readonly IArchiveIdentifier _archiveIdentifier;

        public TestSessionBuilder(IArchiveExtractor archiveExtractor, IArchiveIdentifier archiveIdentifier)
        {
            _archiveExtractor = archiveExtractor;
            _archiveIdentifier = archiveIdentifier;
        }

        public TestSession NewSessionFromTarFile(string archiveFileName, string metadataFileName)
        {
            Log.Information($"Building new TestSession with [archiveFileName: {archiveFileName}] [metadataFileName: {metadataFileName}");

            Archive archive = _archiveExtractor.Extract(archiveFileName);
            archive.ArchiveType = _archiveIdentifier.Identify(metadataFileName);

            return new TestSession(archive);
        }
    }
}