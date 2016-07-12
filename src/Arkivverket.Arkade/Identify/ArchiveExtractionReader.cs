using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Identify
{
    public class ArchiveExtractionReader
    {
        private readonly IArchiveExtractor _archiveExtractor;

        public ArchiveExtractionReader(IArchiveExtractor archiveExtractor)
        {
            _archiveExtractor = archiveExtractor;
        }

        public ArchiveExtraction ReadFromFile(string fileName)
        {
            return _archiveExtractor.Extract(fileName);
        }
    }
}