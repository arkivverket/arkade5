using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Metadata
{
    public class MetadataFilesCreator
    {
        private readonly DiasMetsCreator _diasMetsCreator;
        private readonly DiasPremisCreator _diasPremisCreator;


        public MetadataFilesCreator(DiasMetsCreator diasMetsCreator, DiasPremisCreator diasPremisCreator)
        {
            _diasMetsCreator = diasMetsCreator;
            _diasPremisCreator = diasPremisCreator;
        }

        public void Create(Archive archive, ArchiveMetadata metadata)
        {
            _diasMetsCreator.CreateAndSaveFile(archive, metadata);
            _diasPremisCreator.CreateAndSaveFile(archive, metadata);
        }

    }
}
