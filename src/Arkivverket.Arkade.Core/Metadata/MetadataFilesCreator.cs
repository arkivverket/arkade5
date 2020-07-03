using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Report;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Metadata
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

        public void Create(Archive archive, ArchiveMetadata metadata, bool generateDocumentFileInfo)
        {
            _diasPremisCreator.CreateAndSaveFile(archive, metadata);
            _logCreator.CreateAndSaveFile(archive, metadata);
            // EAD is not included in v1.0
            _eadCreator.CreateAndSaveFile(archive, metadata);
            // EAC-CPF is not included in v1.0
            _eacCpfCreator.CreateAndSaveFile(archive, metadata);

            CopyXsdFiles(archive.WorkingDirectory);
            
            if (generateDocumentFileInfo)
            {
                string fileLocation = archive.WorkingDirectory.AdministrativeMetadata().DirectoryInfo().FullName;
                DocumentFileListGenerator.Generate(fileLocation, archive);
            }

            // Generate mets-file last for it to describe all other package content
            _diasMetsCreator.CreateAndSaveFile(archive, metadata);
        }

        private static void CopyXsdFiles(WorkingDirectory workingDirectory)
        {
            CopyXsdFile(
                ArkadeConstants.DiasMetsXsdResource,
                ArkadeConstants.DiasMetsXsdFileName,
                workingDirectory.Root()
            );

            CopyXsdFile(
                ArkadeConstants.DiasPremisXsdResource,
                ArkadeConstants.DiasPremisXsdFileName,
                workingDirectory.AdministrativeMetadata()
            );

            CopyXsdFile(
                ArkadeConstants.AddmlXsdResource,
                ArkadeConstants.AddmlXsdFileName,
                workingDirectory.AdministrativeMetadata()
            );
        }

        private static void CopyXsdFile(string xsdResource, string xsdFileName, ArkadeDirectory arkadeDirectory)
        {
            using (Stream xsdResourceStream = ResourceUtil.GetResourceAsStream(xsdResource))
            {
                string targetXsdFileName = arkadeDirectory.WithFile(xsdFileName).FullName;

                using (FileStream targetXsdFileStream = File.Create(targetXsdFileName))
                {
                    xsdResourceStream.CopyTo(targetXsdFileStream);
                }
            }
        }
    }
}