using System.IO;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class ArchiveContentMemoryStreamReader : IArchiveContentReader
    {
        private readonly Stream _stream;

        public ArchiveContentMemoryStreamReader(Stream stream)
        {
            _stream = stream;
        }

        public Stream GetContentAsStream(ArchiveExtraction archiveExtraction)
        {
            return _stream;
        }
    }
}
