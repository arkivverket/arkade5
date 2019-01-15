using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Util
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
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new ArkadeException("The given fileName cannot be null, empty or whitespace");

                return new FileInfo(Path.Combine(directory.FullName, fileName));
            }
            catch (ArgumentException argumentException)
            {
                string pathCombineExceptionMessage = string.Format(
                    ExceptionMessages.PathCombine, directory.FullName, fileName
                );

                throw new ArkadeException($"{pathCombineExceptionMessage}: {argumentException.Message}");
            }
        }

        public ArkadeDirectory WithSubDirectory(string subDirectoryName)
        {
            return new ArkadeDirectory(new DirectoryInfo(Path.Combine(_directoryInfo.FullName, subDirectoryName)));
        }
    }
}
