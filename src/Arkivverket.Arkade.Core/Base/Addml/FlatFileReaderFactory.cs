using System;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Archives;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    public class FlatFileReaderFactory
    {
        public IRecordEnumerator GetRecordEnumerator(Archive archive, FlatFile flatFile)
        {
            if (!Enum.IsDefined(typeof(ArchiveType), archive.ArchiveType))
            {
                throw new ArgumentException(
                    $"Cannot instantiate file reader for archive. Unsupported archive type: {archive.ArchiveType}");
            }

            var fileInfo = flatFile?.Definition?.FileInfo;
            if (fileInfo == null)
            {
                throw new ArgumentException("flatFile.Definition.FileInfo cannot be null");
            }
            if (!fileInfo.Exists)
            {
                return null;
            }

            switch (archive)
            {
                case Noark3Archive:
                case SpecializedSystemArchive:
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
                default:
                    {
                        throw new ArgumentException("Unsupported archive: " + archive.GetType().Name);
                    }
            }
        }
    }
}
