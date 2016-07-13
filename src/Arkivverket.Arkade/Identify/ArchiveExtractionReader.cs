using Arkivverket.Arkade.Core;
using Serilog;

namespace Arkivverket.Arkade.Identify
{
    public class ArchiveExtractionReader
    {
        private readonly IArchiveExtractor _archiveExtractor;

        public ArchiveExtractionReader(IArchiveExtractor archiveExtractor)
        {
            _archiveExtractor = archiveExtractor;
        }

        public ArchiveExtraction ReadFromFile(string archiveFileName)
        {
            Log.Information("Reading archive from file: " + archiveFileName);

            return _archiveExtractor.Extract(archiveFileName);
        }
    }
}