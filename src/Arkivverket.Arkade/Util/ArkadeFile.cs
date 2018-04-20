using System;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Util
{
    public class ArkadeFile
    {
        public string Name { get; }
        public string FullName { get; }
        public bool Exists { get; }

        public ArkadeFile(FileSystemInfo fileInfo)
        {
            Name = fileInfo.Name;
            FullName = fileInfo.FullName;
            Exists = fileInfo.Exists;
        }

        public Stream AsStream()
        {
            try
            {
                return File.OpenRead(FullName);
            }
            catch (Exception e)
            {
                string message = string.Format(Messages.FileNotFoundMessage, FullName);
                throw new ArkadeException(message, e);
            }
        }
    }
}
