using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.ExternalModels.Info;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Identify
{
    public class ArchiveIdentifier : IArchiveIdentifier
    {

        public ArchiveType Identify(string metadataFileName)
        {
            string metadataXmlAsString = File.ReadAllText(metadataFileName);
            ExternalModels.Info.info metadata = SerializeUtil.DeserializeFromString<ExternalModels.Info.info>(metadataXmlAsString);
            return Identify(metadata);
        }

        private ArchiveType Identify(ExternalModels.Info.info metadata)
        {
            var utrekkType = metadata.uttrekk.type;
            if (utrekkType == type.Noark5)
            {
                return ArchiveType.Noark5;
            }
            if (utrekkType == type.Noark4)
            {
                return ArchiveType.Noark4;
            }
            if (utrekkType == type.Noark3)
            {
                return ArchiveType.Noark3;
            }
            return ArchiveType.Fagsystem;
        }
    }
}
