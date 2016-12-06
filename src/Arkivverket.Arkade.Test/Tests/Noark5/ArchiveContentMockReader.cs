using System.IO;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class ArchiveContentMockReader : IArchiveContentReader
    {
        private readonly Stream _contentStream;
        private readonly Stream _structureStream;

        public ArchiveContentMockReader(Stream contentStream, Stream structureStream)
        {
            _contentStream = contentStream;
            _structureStream = structureStream;
        }

        public Stream GetContentAsStream(Archive archiveExtraction)
        {
            return _contentStream;
        }

        public Stream GetStructureContentAsStream(Archive archive)
        {
            return _structureStream;
        }
    }
}
