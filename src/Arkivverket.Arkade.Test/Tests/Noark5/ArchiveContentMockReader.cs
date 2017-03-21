using System;
using System.IO;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class ArchiveContentMockReader : IArchiveContentReader
    {
        private readonly Stream _contentStream;
        private readonly Stream _structureStream;
        private readonly Stream _contentSchemaStream;
        private readonly Stream _structureSchemaStream;
        private readonly Stream _metadataCatalogSchemaStream;

        public ArchiveContentMockReader(Stream contentStream, Stream structureStream)
        {
            _contentStream = contentStream;
            _structureStream = structureStream;
        }
        public ArchiveContentMockReader(Stream contentStream, Stream structureStream, Stream contentSchemaStream, Stream structureSchemaStream, Stream metadataCatalogSchemaStream)
        {
            _contentStream = contentStream;
            _structureStream = structureStream;
            _contentSchemaStream = contentSchemaStream;
            _structureSchemaStream = structureSchemaStream;
            _metadataCatalogSchemaStream = metadataCatalogSchemaStream;
        }

        public Stream GetContentAsStream(Archive archiveExtraction)
        {
            return _contentStream;
        }

        public Stream GetStructureContentAsStream(Archive archive)
        {
            return _structureStream;
        }

        public Stream GetContentDescriptionXmlSchemaAsStream(Archive archive)
        {
            return _contentSchemaStream;
        }

        public Stream GetStructureDescriptionXmlSchemaAsStream(Archive archive)
        {
            return _structureSchemaStream;
        }

        public Stream GetMetadataCatalogXmlSchemaAsStream(Archive archive)
        {
            return _metadataCatalogSchemaStream;
        }
    }
}
