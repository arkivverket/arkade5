using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Metadata
{
    public class MetadataFilesCreator
    {
        private readonly DiasMetsCreator _diasMetsCreator;

        public MetadataFilesCreator(DiasMetsCreator diasMetsCreator)
        {
            _diasMetsCreator = diasMetsCreator;
        }

        public void Create(Archive archive, ArchiveMetadata metadata)
        {
            _diasMetsCreator.CreateAndSaveFile(archive, metadata);
        }

    }
}
