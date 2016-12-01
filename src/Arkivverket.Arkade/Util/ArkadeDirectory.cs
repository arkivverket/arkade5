using System.IO;

namespace Arkivverket.Arkade.Util
{
    public class ArkadeDirectory
    {
        private readonly DirectoryInfo _directoryInfo;
        public ArkadeDirectory(DirectoryInfo directoryInfo)
        {
            _directoryInfo = directoryInfo;
        }

        public FileInfo WithFile(string fileName)
        {
            return AppendFileToPath(_directoryInfo, fileName);
        }

        public override string ToString()
        {
            return _directoryInfo.FullName;
        }

        public DirectoryInfo DirectoryInfo()
        {
            return _directoryInfo;
        }

        public void Create()
        {
            _directoryInfo.Create();
        }

        private static FileInfo AppendFileToPath(DirectoryInfo directory, string fileName)
        {
            return new FileInfo(Path.Combine(directory.FullName, fileName));
        }

        public ArkadeDirectory WithSubDirectory(string subDirectoryName)
        {
            return new ArkadeDirectory(new DirectoryInfo(Path.Combine(_directoryInfo.FullName, subDirectoryName)));
        }
    }
}
