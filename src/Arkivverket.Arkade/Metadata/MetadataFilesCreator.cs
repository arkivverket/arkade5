using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Metadata
{
    public class MetadataFilesCreator
    {
        private readonly DiasMetsCreator _diasMetsCreator;
        private readonly DiasPremisCreator _diasPremisCreator;
        private readonly EacCpfCreator _eacCpfCreator;
        private readonly EadCreator _eadCreator;


        public MetadataFilesCreator(DiasMetsCreator diasMetsCreator, DiasPremisCreator diasPremisCreator, EadCreator eadCreator,
            EacCpfCreator eacCpfCreator)
        {
            _diasMetsCreator = diasMetsCreator;
            _diasPremisCreator = diasPremisCreator;
            _eadCreator = eadCreator;
            _eacCpfCreator = eacCpfCreator;
        }

        public void Create(Archive archive, ArchiveMetadata metadata)
        {
            _diasMetsCreator.CreateAndSaveFile(archive, metadata);
            _diasPremisCreator.CreateAndSaveFile(archive, metadata);
            _eadCreator.CreateAndSaveFile(archive, metadata);
            _eacCpfCreator.CreateAndSaveFile(archive, metadata);
        }
    }
}