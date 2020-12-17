using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Arkivverket.Arkade.Core.Base.Siard
{
    public interface ISiardArchiveReader
    {
        Dictionary<string, List<int>> GetLobFolderPathsWithColumnIndexes(string siardArchivePath);

        string GetNamedEntryFromSiardFileStream(Stream siardFileStream, string namedEntry);

        Stream GetNamedEntryStreamFromSiardZipArchive(ZipArchive siardZipArchive, string namedEntry);
    }
}