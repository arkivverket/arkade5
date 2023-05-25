using Arkivverket.Arkade.Core.Base;
using ICSharpCode.SharpZipLib.Tar;
using System.IO;
using System.Text;

namespace Arkivverket.Arkade.Core.Util
{
    public class Noark5DocumentFileTarEntryTransferManager
    {
        public static void TransferDocumentFiles(string archiveFileFullPath, Uuid sourceUuid, TarOutputStream tarOutputStream)
        {
            using var tarInputStream = new TarInputStream(File.OpenRead(archiveFileFullPath), Encoding.UTF8);

            while (tarInputStream.GetNextEntry() is { Name: not null } entry)
            {
                if (!entry.IsNoark5DocumentsEntry(sourceUuid))
                    continue;

                tarOutputStream.PutNextEntry(entry);

                tarInputStream.CopyEntryContents(tarOutputStream);

                tarOutputStream.CloseEntry();
            }
        }
    }
}
