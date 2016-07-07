using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Util
{
    interface ICompressionDecompressionUtilities
    {
        int ExtractFolderFromArchive(string tarFileName, string targetFolderName);
        int CompressFolderContentToArchiveFile(string tarTargetFileName, string sourceFileFolder);
    }
}
