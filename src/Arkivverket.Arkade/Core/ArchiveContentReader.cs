using System.IO;

namespace Arkivverket.Arkade.Core
{
    public class ArchiveContentReader : IArchiveContentReader
    {
        public Stream GetContentAsStream(ArchiveExtraction archiveExtraction)
        {

            // TODO: investigate if we can cache the stream instead of creating it from scratch multiple times. When should we call Dispose() on the stream?
            return File.OpenRead(archiveExtraction.GetContentDescriptionFileName());
        }
    }
}