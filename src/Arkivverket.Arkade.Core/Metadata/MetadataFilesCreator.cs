using Arkivverket.Arkade.Core.Base;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Metadata
{
    public class MetadataFilesCreator
    {
        private readonly DiasMetsCreator _diasMetsCreator;
        private readonly DiasPremisCreator _diasPremisCreator;
        private readonly LogCreator _logCreator;
        private readonly EacCpfCreator _eacCpfCreator;
        private readonly EadCreator _eadCreator;

        public MetadataFilesCreator(DiasMetsCreator diasMetsCreator, DiasPremisCreator diasPremisCreator,
            EadCreator eadCreator, EacCpfCreator eacCpfCreator, LogCreator logCreator)
        {
            _diasMetsCreator = diasMetsCreator;
            _diasPremisCreator = diasPremisCreator;
            _logCreator = logCreator;
            _eadCreator = eadCreator;
            _eacCpfCreator = eacCpfCreator;
        }

        public void Create(Archive archive, ArchiveMetadata metadata)
        {
            _diasPremisCreator.CreateAndSaveFile(archive, metadata);
            _logCreator.CreateAndSaveFile(archive, metadata);
            // EAD is not included in v1.0
            _eadCreator.CreateAndSaveFile(archive, metadata);
            // EAC-CPF is not included in v1.0
            _eacCpfCreator.CreateAndSaveFile(archive, metadata);

            AddXsdFiles(archive.WorkingDirectory);

            // Generate mets-file last for it to describe all other package content
            _diasMetsCreator.CreateAndSaveFile(archive, metadata);
        }

        private static void AddXsdFiles(WorkingDirectory workingDirectory)
        {
            workingDirectory.Root().AddFileFromResources(DiasMetsXsdResource, DiasMetsXsdFileName);

            workingDirectory.AdministrativeMetadata()
                .AddFileFromResources(DiasPremisXsdResource, DiasPremisXsdFileName);

            workingDirectory.DescriptiveMetadata().AddFileFromResources(EadXsdResource, EadXsdFileName);
            workingDirectory.DescriptiveMetadata().AddFileFromResources(EacCpfXsdResource, EacCpfXsdFileName);
        }
    }
}