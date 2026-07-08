using System.IO;

namespace Arkivverket.Arkade.Core.Base
{
    public abstract class ArchiveXmlSchema
    {
        public string Name => GetName();
        protected abstract string GetName();
        public abstract Stream AsStream();
    }
}
