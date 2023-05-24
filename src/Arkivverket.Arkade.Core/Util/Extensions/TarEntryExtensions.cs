using System;
using ICSharpCode.SharpZipLib.Tar;
using System.Linq;
using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Util
{
    public static class TarEntryExtensions
    {
        public static bool IsNoark5DocumentsEntry(this TarEntry tarEntry, Uuid uuid)
        {
            string entryName = tarEntry.Name.Replace('\\', '/');

            return ArkadeConstants.DocumentDirectoryNames.Any(documentDirectoryName =>
                entryName.StartsWith($"{uuid}/content/{documentDirectoryName}"));
        }

        public static string GetRelativePathForNoark5DocumentEntry(this TarEntry tarEntry)
        {
            string entryName = tarEntry.Name.Replace('\\', '/');

            foreach (string documentDirectoryName in ArkadeConstants.DocumentDirectoryNames)
            {
                int endIndex = entryName.IndexOf(documentDirectoryName, StringComparison.InvariantCultureIgnoreCase);

                if (endIndex > 0)
                    return entryName.Remove(0, endIndex);
            }

            return entryName;
        }
    }
}
