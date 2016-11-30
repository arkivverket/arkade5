using System;
using System.IO;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Core
{
    public class ArchiveContentReader : IArchiveContentReader
    {
        public Stream GetContentAsStream(Archive archive)
        {
            try
            {
                return File.OpenRead(archive.GetContentDescriptionFileName());
            }
            catch (Exception e)
            {
                string message = string.Format(Messages.FileNotFoundMessage, archive.GetContentDescriptionFileName());
                throw new ArkadeException(message, e);
            }
        }

        public Stream GetStructureContentAsStream(Archive archive)
        {
            try
            {
                return File.OpenRead(archive.GetStructureDescriptionFileName());
            }
            catch (Exception e)
            {
                string message = string.Format(Messages.FileNotFoundMessage, archive.GetStructureDescriptionFileName());
                throw new ArkadeException(message, e);
            }
        }
    }
}