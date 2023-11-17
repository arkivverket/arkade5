using System.IO;

namespace Arkivverket.Arkade.Core.Util
{
    public static class DirectoryInfoExtensions
    {
        public static void CopyTo(this DirectoryInfo sourceDirectory, string directoryCopyLocationPath, bool overwrite)
        {
            var directoryCopy = new DirectoryInfo(Path.Combine(directoryCopyLocationPath, sourceDirectory.Name));

            if (overwrite && directoryCopy.Exists)
                directoryCopy.Delete(true);

            directoryCopy.Create();

            foreach (FileInfo file in sourceDirectory.GetFiles())
                file.CopyTo(Path.Combine(directoryCopy.FullName, file.Name));

            foreach (DirectoryInfo directory in sourceDirectory.GetDirectories())
                directory.CopyTo(directoryCopy.FullName, overwrite);
        }

        public static bool HasWritePermission(this DirectoryInfo directory)
        {
            var dummyFile = new FileInfo(Path.Combine(directory.FullName, "dummy.txt"));

            try
            {
                FileStream tempStream = dummyFile.Create();
                tempStream.Dispose();
                dummyFile.Delete();
                return true;
            }
            catch
            {
                if (dummyFile.Exists) dummyFile.Delete();

                return false;
            }
        }
    }
}
