using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Util
{
    public class ArchivePart
    {
        public string SystemId { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format(Noark5Messages.ArchivePartSystemId, SystemId, Name);
        }
    }
}
