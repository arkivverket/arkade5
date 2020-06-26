using System.Collections.Generic;
using System.IO;

namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    public interface IFileFormatIdentifier
    {
        IEnumerable<SiegfriedFileInfo> IdentifyFormat(DirectoryInfo directory);
    }
}
