using System;
using ICSharpCode.SharpZipLib.Tar;
using System.Linq;

namespace Arkivverket.Arkade.Core.Util
{
    public static class TarEntryExtensions
    {
        public static bool IsNoark5DocumentsEntry(this TarEntry tarEntry, string archiveRootDirectoryName)
        {
            string entryName = tarEntry.Name.Replace('\\', '/');

            // archiveRootDirectoryName is the tar's actual internal root directory; null means the
            // tar has no single root and its entry paths start at the package level
            return ArkadeConstants.DocumentDirectoryNames.Any(documentDirectoryName =>
                entryName.StartsWith(archiveRootDirectoryName == null
                    ? $"content/{documentDirectoryName}"
                    : $"{archiveRootDirectoryName}/content/{documentDirectoryName}"));
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
