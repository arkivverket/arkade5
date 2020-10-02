using System.IO;

namespace Arkivverket.Arkade.Core.Util
{
    public static class PathUtil
    {
        public static string GetRelativePath(FileSystemInfo file, FileSystemInfo startDirectory)
        {
            if (startDirectory == null)
                return file.FullName;
            
            string startDirectoryFullName = startDirectory.FullName;

            if (!startDirectoryFullName.EndsWith(Path.DirectorySeparatorChar.ToString()))
                startDirectoryFullName += Path.DirectorySeparatorChar;

            int relativePathStartIndex = startDirectoryFullName.Length;

            return file.FullName.Substring(relativePathStartIndex);
        }
    }
}
