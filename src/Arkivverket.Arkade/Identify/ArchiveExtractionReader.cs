using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Identify
{
    public class ArchiveExtractionReader
    {
        const string TemporaryDirectory = "c:\\temp";

        private readonly ICompressionUtility _compressionUtility;

        public ArchiveExtractionReader(ICompressionUtility compressionUtility)
        {
            _compressionUtility = compressionUtility;
        }

        public ArchiveExtraction ReadFromFile(string filePath)
        {
            _compressionUtility.ExtractFolderFromArchive(filePath, TemporaryDirectory);
            return new ArchiveExtraction();
        }

        public ArchiveExtraction ReadFromDirectory(string directoryPath)
        {

            return new ArchiveExtraction();
        }


    }
}
