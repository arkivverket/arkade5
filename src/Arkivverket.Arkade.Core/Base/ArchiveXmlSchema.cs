using System.IO;

namespace Arkivverket.Arkade.Core.Base
{
    public abstract class ArchiveXmlSchema
    {
        public string FileName => GetFileName();
        protected abstract string GetFileName();
        public abstract Stream AsStream();
    }
}
