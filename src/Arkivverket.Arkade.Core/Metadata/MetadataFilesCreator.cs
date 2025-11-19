using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Metadata
{
    public class MetadataFilesCreator(
        DiasMetsCreator diasMetsCreator,
        DiasPremisCreator diasPremisCreator,
        EadCreator eadCreator,
        EacCpfCreator eacCpfCreator,
        LogCreator logCreator)
    {
        private readonly List<IMetadataCreator> _metadataCreators = [diasPremisCreator, logCreator, eadCreator, eacCpfCreator];

        public void Create(Archive archive)
        {
            OutputDiasPackage outputDiasPackage = archive.OutputDiasPackage;

            outputDiasPackage.WorkingDirectory.CreateAllFolders(); // Experimental!

            foreach (IMetadataCreator metadataCreator in _metadataCreators)
                metadataCreator.CreateAndSaveFile(outputDiasPackage);
            
            AddXsdFiles(outputDiasPackage.WorkingDirectory);

            // Generate mets-file last for it to describe all other package content
            diasMetsCreator.CreateAndSaveFile(archive);
        }

        private static void AddXsdFiles(DiasPackageWorkingDirectory diasPackageWorkingDirectory)
        {
            diasPackageWorkingDirectory.Root().AddFileFromResources(DiasMetsXsdResource, DiasMetsXsdFileName);

            diasPackageWorkingDirectory.AdministrativeMetadata()
                .AddFileFromResources(DiasPremisXsdResource, DiasPremisXsdFileName);

        }
    }
}