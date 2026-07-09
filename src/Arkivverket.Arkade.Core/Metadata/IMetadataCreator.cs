using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Metadata;

public interface IMetadataCreator
{
    public void CreateAndSaveFile(OutputDiasPackage outputDiasPackage);
}
