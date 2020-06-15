using System.Collections.Generic;
using System.IO;

namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public interface IFileFormatIdentifier
    {
        Dictionary<FileInfo, FileFormat> IdentifyFormat(IEnumerable<FileInfo> files);
    }
}
