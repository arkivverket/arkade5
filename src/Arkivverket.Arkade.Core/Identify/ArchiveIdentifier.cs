using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.ExternalModels.Info;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Identify
{
    public class ArchiveIdentifier : IArchiveIdentifier
    {
        public ArchiveType Identify(string metadataFileName)
        {
            var metadataXmlAsString = File.ReadAllText(metadataFileName);
            var metadata = SerializeUtil.DeserializeFromString<info>(metadataXmlAsString);
            return Identify(metadata);
        }

        private ArchiveType Identify(info metadata)
        {
            var utrekkType = metadata.uttrekk.type;
            if (utrekkType == type.Noark5)
            {
                return ArchiveType.Noark5;
            }
            if (utrekkType == type.Noark3)
            {
                return ArchiveType.Noark3;
            }
            return ArchiveType.Fagsystem;
        }
    }
}