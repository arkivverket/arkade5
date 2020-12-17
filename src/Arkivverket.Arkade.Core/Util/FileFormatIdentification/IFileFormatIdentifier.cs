using System.Collections.Generic;
using System.IO;

namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public interface IFileFormatIdentifier
    {
        IEnumerable<IFileFormatInfo> IdentifyFormat(DirectoryInfo directory);

        IFileFormatInfo IdentifyFormat(FileInfo file);

        IFileFormatInfo IdentifyFormat(KeyValuePair<string, Stream> filePathAndStream);
    }
}
