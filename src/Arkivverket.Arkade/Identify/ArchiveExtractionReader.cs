using Arkivverket.Arkade.Core;
using Serilog;

namespace Arkivverket.Arkade.Identify
{
    public class ArchiveExtractionReader
    {
        private readonly IArchiveExtractor _archiveExtractor;
        private readonly IArchiveIdentifier _archiveIdentifier;

        public ArchiveExtractionReader(IArchiveExtractor archiveExtractor, IArchiveIdentifier archiveIdentifier)
        {
            _archiveExtractor = archiveExtractor;
            _archiveIdentifier = archiveIdentifier;
        }

        public Archive ReadFromFile(string archiveFileName, string metadataFileName)
        {
            Log.Information("Reading archive from file: " + archiveFileName);
            Log.Information("Reading archive metadata from file: " + metadataFileName);

            Archive archiveExtraction = _archiveExtractor.Extract(archiveFileName);
            archiveExtraction.ArchiveType = _archiveIdentifier.Identify(metadataFileName);
            return archiveExtraction;
        }
    }
}