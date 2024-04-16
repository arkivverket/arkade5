using System.Collections.Generic;
using System.IO;

namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public interface IFileFormatIdentifier
    {
        IEnumerable<IFileFormatInfo> IdentifyFormats(string target, FileFormatScanMode scanMode);

        IEnumerable<IFileFormatInfo> IdentifyFormats(IEnumerable<KeyValuePair<string, IEnumerable<byte>>> filePathsAndByteContent);

        IFileFormatInfo IdentifyFormat(FileInfo file);

        IFileFormatInfo IdentifyFormat(KeyValuePair<string, IEnumerable<byte>> filePathAndByteContent);
        
        void BroadCastStarted();
        
        void BroadCastFinished();
    }
}
