using System;
using System.IO;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Base
{
    public class ArchiveXmlFile
    {
        public string Name { get; }
        public string FullName { get; }

        public ArchiveXmlFile(FileSystemInfo fileInfo)
        {
            Name = fileInfo.Name;
            FullName = fileInfo.FullName;
        }

        public Stream AsStream()
        {
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
