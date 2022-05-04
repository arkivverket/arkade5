using System.IO;
using System.Linq;

namespace Arkivverket.Arkade.Core.Util
{
    public static class DirectoryInfoExtensions
    {
        public static void CopyTo(this DirectoryInfo sourceDirectoryInfo, string destinationPath, bool withSubDirectories)
        {
            Directory.CreateDirectory(destinationPath);

            foreach (FileInfo fileInfo in sourceDirectoryInfo.GetFiles())
            {
                fileInfo.CopyTo(Path.Combine(destinationPath, fileInfo.Name));
            }

            if (!withSubDirectories)
                return;
            
            foreach (DirectoryInfo subDirectory in sourceDirectoryInfo.GetDirectories())
            {
                subDirectory.CopyTo(Path.Combine(destinationPath, subDirectory.Name), true);
            }
        }

        public static long GetNumberOfFileInfoObjects(this DirectoryInfo directory, bool recursive = true)
        {
            long numberOfFileInfoObjects = directory.GetFiles().LongLength;

            if (!recursive)
                return numberOfFileInfoObjects;

            numberOfFileInfoObjects = directory.GetDirectories()
                .Aggregate(numberOfFileInfoObjects, (i, info) => i + info.GetNumberOfFileInfoObjects());

            return numberOfFileInfoObjects;
        }
    }
}
