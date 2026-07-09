using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
using static Arkivverket.Arkade.Core.Base.PackageType;
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

        public void Create(Archive archive)
        {
            OutputDiasPackage outputDiasPackage = archive.OutputDiasPackage;

            List<IMetadataCreator> metadataCreators = outputDiasPackage.PackageType switch
            {
                SubmissionInformationPackage => [diasPremisCreator, logCreator],
                ArchivalInformationPackage => [diasPremisCreator, logCreator, eadCreator, eacCpfCreator],
                _ => throw new ArgumentOutOfRangeException(nameof(outputDiasPackage), outputDiasPackage.PackageType, null)
            };

            foreach (IMetadataCreator metadataCreator in metadataCreators)
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