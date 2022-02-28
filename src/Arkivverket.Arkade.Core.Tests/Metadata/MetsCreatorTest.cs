using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Metadata;

namespace Arkivverket.Arkade.Core.Tests.Metadata
{
    public class MetsCreatorTest
    {
        protected readonly ArchiveMetadata ArchiveMetadata;

        public MetsCreatorTest()
        {
            ArchiveMetadata = MetadataExampleCreator.Create(MetadataExamplePurpose.InternalTesting);
        }
    }
}
