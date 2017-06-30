using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Metadata
{
    public class MetadataFilesCreator
    {
        private readonly DiasMetsCreator _diasMetsCreator;
        private readonly DiasPremisCreator _diasPremisCreator;
        private readonly EacCpfCreator _eacCpfCreator;
        private readonly EadCreator _eadCreator;
        private readonly InfoXmlCreator _infoXmlCreator;

        public MetadataFilesCreator(DiasMetsCreator diasMetsCreator, DiasPremisCreator diasPremisCreator, EadCreator eadCreator,
            EacCpfCreator eacCpfCreator, InfoXmlCreator infoXmlCreator)
        {
            _diasMetsCreator = diasMetsCreator;
            _diasPremisCreator = diasPremisCreator;
            _eadCreator = eadCreator;
            _eacCpfCreator = eacCpfCreator;
            _infoXmlCreator = infoXmlCreator;
        }

        public void Create(Archive archive, ArchiveMetadata metadata)
        {
            _diasMetsCreator.CreateAndSaveFile(archive, metadata);
            _diasPremisCreator.CreateAndSaveFile(archive, metadata);
            // EAD is not included in v1.0
            _eadCreator.CreateAndSaveFile(archive, metadata);
            // EAC-CPF is not included in v1.0
            _eacCpfCreator.CreateAndSaveFile(archive, metadata);
            CopyDiasMetsXsdToRootDirectory(archive.WorkingDirectory);
            //_infoXmlCreator.CreateAndSaveFile(archive, metadata);
        }

        private void CopyDiasMetsXsdToRootDirectory(WorkingDirectory workingDirectory)
        {
            using (Stream xsdAsStream = ResourceUtil.GetResourceAsStream(ArkadeConstants.DiasMetsXsdResource))
            {
                string targetFile = workingDirectory.Root().WithFile(ArkadeConstants.DiasMetsXsdFileName).FullName;
                using (FileStream fileStream = File.Create(targetFile))
                {
                    xsdAsStream.CopyTo(fileStream);
                }
            }
        }
    }
}