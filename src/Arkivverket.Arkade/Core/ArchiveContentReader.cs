using System;
using System.IO;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core
{
    public class ArchiveContentReader : IArchiveContentReader
    {
        public Stream GetContentAsStream(Archive archive)
        {
            string fileName = archive.GetContentDescriptionFileName();
            return GetFileAsStream(fileName);
        }

        public Stream GetStructureContentAsStream(Archive archive)
        {
            string fileName = archive.GetStructureDescriptionFileName();
            return GetFileAsStream(fileName);
        }
        public Stream GetContentDescriptionXmlSchemaAsStream(Archive archive)
        {
            string fileName = archive.GetContentDescriptionXmlSchemaFileName();
            return GetFileAsStream(fileName);
        }

        public Stream GetStructureDescriptionXmlSchemaAsStream(Archive archive)
        {
            string fileName = archive.GetStructureDescriptionXmlSchemaFileName();
            return GetFileAsStream(fileName);
        }
        public Stream GetMetadataCatalogXmlSchemaAsStream(Archive archive)
        {
            string fileName = archive.GetMetadataCatalogXmlSchemaFileName();
            return GetFileAsStream(fileName);
        }

        private static Stream GetFileAsStream(string fileName)
        {
            var fileInfo = new FileInfo(fileName);

            try
            {
                return fileInfo.OpenRead();
            }
            catch (Exception e)
            {
                if(fileName.Contains(ArkadeConstants.GetArkadeWorkDirectory().Name))
                    fileName = fileInfo.Name;

                string message = string.Format(Messages.FileNotFoundMessage, fileName);
                throw new ArkadeException(message, e);
            }
        }
    }
}