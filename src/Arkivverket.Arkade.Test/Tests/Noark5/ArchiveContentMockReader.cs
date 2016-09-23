using System.IO;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class ArchiveContentMockReader : IArchiveContentReader
    {
        private readonly Stream _stream;

        public ArchiveContentMockReader(Stream stream)
        {
            _stream = stream;
        }

        public Stream GetContentAsStream(Archive archiveExtraction)
        {
            return _stream;
        }

        public Stream GetStructureContentAsStream(Archive archive)
        {
            return _stream;
        }
    }
}
