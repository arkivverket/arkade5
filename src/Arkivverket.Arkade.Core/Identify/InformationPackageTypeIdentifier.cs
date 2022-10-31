using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.ExternalModels.SubmissionDescription;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Identify
{
    public class InformationPackageTypeIdentifier
    {
        public PackageType? IdentifyPackageTypeFromArchiveFile(string archiveFileName)
        {
            string infoFilePath = archiveFileName.Replace(Path.GetExtension(archiveFileName), ".xml");

            if (!File.Exists(infoFilePath))
            {
                return null;
            }

            var infoFile = SerializeUtil.DeserializeFromFile<mets>(infoFilePath);

            return (infoFile.TYPE) switch
            {
                metsTypeTYPE.AIP => PackageType.ArchivalInformationPackage,
                metsTypeTYPE.SIP => PackageType.SubmissionInformationPackage,
                _ => null,
            };
        }
    }
}
