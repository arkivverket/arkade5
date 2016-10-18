using System;

namespace Arkivverket.Arkade.Core.Addml
{
    public class FlatFileReaderFactory
    {
        public IFlatFileReader GetReader(Archive archive, FlatFile flatFile)
        {
            if (archive.ArchiveType == ArchiveType.Noark3 || archive.ArchiveType == ArchiveType.Fagsystem)
                return new FlatFileReader(flatFile);
            else if (archive.ArchiveType == ArchiveType.Noark4)
                return new Noark4FileReader();
            else
                throw new ArgumentException($"Cannot instantiate file reader for archive. Unsupported archive type: {archive.ArchiveType}");
        }
    }
}
