using System;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Test.Core;

namespace Arkivverket.Arkade.Core.Addml
{
    public class FlatFileReaderFactory
    {
        public IRecordEnumerator GetRecordEnumerator(Archive archive, FlatFile flatFile)
        {
            if (archive.ArchiveType == ArchiveType.Noark3 || archive.ArchiveType == ArchiveType.Fagsystem)
            {
                AddmlFlatFileFormat format = flatFile.Definition.Format;
                switch (format)
                {
                    case AddmlFlatFileFormat.Delimiter:
                        return new DelimiterFileFormatReader(flatFile);
                    case AddmlFlatFileFormat.Fixed:
                        return new FixedFileFormatReader(flatFile);
                    default:
                        throw new ArkadeException("Unkown AddmlFlatFileFormat: " + format);
                }
            }
            else if (archive.ArchiveType == ArchiveType.Noark4)
            {
                return new Noark4FileReader(flatFile);
            }
            else
            {
                throw new ArgumentException(
                    $"Cannot instantiate file reader for archive. Unsupported archive type: {archive.ArchiveType}");
            }
        }
    }
}