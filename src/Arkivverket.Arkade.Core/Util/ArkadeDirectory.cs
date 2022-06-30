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

        public long GetSize()
        {
           return GetSize(_directoryInfo);
        }

        internal void AddFileFromResources(string resourceName, string fileName)
        {
            string targetXsdFileName = WithFile(fileName).FullName;

            using Stream xsdResourceStream = ResourceUtil.GetResourceAsStream(resourceName);
            using FileStream targetXsdFileStream = File.Create(targetXsdFileName);

            xsdResourceStream.CopyTo(targetXsdFileStream);
        }

        private static long GetSize(DirectoryInfo directory)
        {
            long size = 0;

            foreach (FileInfo file in directory.GetFiles())
                size += file.Length;

            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
                size += GetSize(subDirectory);

            return size;
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
