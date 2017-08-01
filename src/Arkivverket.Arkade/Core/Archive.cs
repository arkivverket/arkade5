using System.IO;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core
{
    public class Archive
    {
        public Uuid Uuid { get; }
        public WorkingDirectory WorkingDirectory { get; }
        public ArchiveType ArchiveType { get; }
        private DirectoryInfo DocumentsDirectory { get; set; }

        public Archive(ArchiveType archiveType, Uuid uuid, WorkingDirectory workingDirectory)
        {
            ArchiveType = archiveType;
            Uuid = uuid;
            WorkingDirectory = workingDirectory;
        }

        public string GetContentDescriptionFileName()
        {
            return WorkingDirectory.Content().WithFile(ArkadeConstants.ArkivstrukturXmlFileName).FullName;
        }

        public string GetStructureDescriptionFileName()
        {
            return WorkingDirectory.AdministrativeMetadata().WithFile(ArkadeConstants.AddmlXmlFileName).FullName;
        }

        public string GetContentDescriptionXmlSchemaFileName()
        {
            return WorkingDirectory.Content().WithFile(ArkadeConstants.ArkivstrukturXsdFileName).FullName;
        }

        public string GetStructureDescriptionXmlSchemaFileName()
        {
            return WorkingDirectory.Content().WithFile(ArkadeConstants.AddmlXsdFileName).FullName;
        }

        public string GetMetadataCatalogXmlSchemaFileName()
        {
            return WorkingDirectory.Content().WithFile(ArkadeConstants.MetadatakatalogXsdFileName).FullName;
        }

        public bool HasStructureDescriptionXmlSchema()
        {
            return File.Exists(GetStructureDescriptionXmlSchemaFileName());
        }

        public bool HasContentDescriptionXmlSchema()
        {
            return File.Exists(GetContentDescriptionXmlSchemaFileName());
        }

        public bool HasMetadataCatalogXmlSchema()
        {
            return File.Exists(GetMetadataCatalogXmlSchemaFileName());
        }

        public FileInfo GetInformationPackageFileName()
        {
            return WorkingDirectory.Root().WithFile(Uuid + ".tar");
        }

        public FileInfo GetInfoXmlFileName()
        {
            return WorkingDirectory.Root().WithFile(Uuid + ".xml");
        }

        public DirectoryInfo GetDocumentsDirectory()
        {
            // Looks (once) for a document-directory with one of the names defined in DocumentDirectoryNames.
            // If an actual directory is found, the field DocumentsDirectory is assigned with it and returned.
            // If none is found, the field DocumentsDirectory is assigned with a new, non-existing directory
            // given the name of the first element of DocumentDirectoryNames (should be the most common name).
            // The method will always return a DirectoryInfo object in which can be checked by it's Exists-field.

            if (DocumentsDirectory == null)
            {
                int index = ArkadeConstants.DocumentDirectoryNames.Length - 1;

                do DocumentsDirectory = WorkingDirectory.Content().WithSubDirectory(
                        ArkadeConstants.DocumentDirectoryNames[index--]
                    ).DirectoryInfo();

                while (index >= 0 && !DocumentsDirectory.Exists);
            }

            return DocumentsDirectory;
        }
    }

    public enum ArchiveType
    {
        Noark3,
        Noark4,
        Noark5,
        Fagsystem,
    }
}