using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Metadata
{
    public class MetadataFilesCreator
    {
        private readonly DiasMetsCreator _diasMetsCreator;
        private readonly DiasPremisCreator _diasPremisCreator;
        private readonly LogCreator _logCreator;
        private readonly EacCpfCreator _eacCpfCreator;
        private readonly EadCreator _eadCreator;

        public MetadataFilesCreator(DiasMetsCreator diasMetsCreator, DiasPremisCreator diasPremisCreator, EadCreator eadCreator,
            EacCpfCreator eacCpfCreator, LogCreator logCreator)
        {
            _diasMetsCreator = diasMetsCreator;
            _diasPremisCreator = diasPremisCreator;
            _logCreator = logCreator;
            _eadCreator = eadCreator;
            _eacCpfCreator = eacCpfCreator;
        }

        public void Create(Archive archive, ArchiveMetadata metadata, PackageType packageType)
        {
            _diasPremisCreator.CreateAndSaveFile(archive, metadata);
            _logCreator.CreateAndSaveFile(archive, metadata);
            // EAD is not included in v1.0
            _eadCreator.CreateAndSaveFile(archive, metadata);
            // EAC-CPF is not included in v1.0
            _eacCpfCreator.CreateAndSaveFile(archive, metadata);
            CopyDiasMetsXsdToRootDirectory(archive.WorkingDirectory);

            // Generate mets-file last for it to describe all other package content
            _diasMetsCreator.CreateAndSaveFile(archive, metadata, packageType);
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