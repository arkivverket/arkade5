using System;
using System.IO;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Base
{
    public class ArchiveXmlFile
    {
        public string Name { get; }
        public string FullName { get; }
        public bool Exists { get; }

        public ArchiveXmlFile(FileSystemInfo fileInfo)
        {
            Name = fileInfo.Name;
            FullName = fileInfo.FullName;
            Exists = fileInfo.Exists;
        }

        public Stream AsStream()
        {
            if (!Exists)
                throw new ArkadeException(string.Format(ExceptionMessages.FileNotFound, FullName));
            
            try
            {
                return File.OpenRead(FullName);
            }
            catch (Exception e)
            {
                throw new ArkadeException(string.Format(ExceptionMessages.FileNotRead, FullName), e);
            }
        }
    }
}
