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

        public FileInfo GetStructureDescriptionFile()
        {
            return WorkingDirectory.AdministrativeMetadata().WithFile(ArkadeConstants.AddmlXmlFileName);
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

        public string GetPublicJournalFileName()
        {
            return WorkingDirectory.Content().WithFile(ArkadeConstants.PublicJournalXmlFileName).FullName;
        }

        public string GetRunningJournalFileName()
        {
            return WorkingDirectory.Content().WithFile(ArkadeConstants.RunningJournalXmlFileName).FullName;
        }

        public string GetPublicJournalXmlSchemaFileName()
        {
            return WorkingDirectory.Content().WithFile(ArkadeConstants.PublicJournalXsdFileName).FullName;
        }

        public string GetRunningJournalXmlSchemaFileName()
        {
            return WorkingDirectory.Content().WithFile(ArkadeConstants.RunningJournalXsdFileName).FullName;
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

        public bool HasPublicJournalXmlSchema()
        {
            return File.Exists(GetPublicJournalXmlSchemaFileName());
        }

        public bool HasRunningJournalXmlSchema()
        {
            return File.Exists(GetRunningJournalXmlSchemaFileName());
        }

        public string GetInformationPackageFileName()
        {
            return Uuid + ".tar";
        }

        public DirectoryInfo GetDocumentsDirectory()
        {
            if (DocumentsDirectory != null)
                return DocumentsDirectory;

            foreach (DirectoryInfo directory in WorkingDirectory.Content().DirectoryInfo().EnumerateDirectories())
            foreach (string documentDirectoryName in ArkadeConstants.DocumentDirectoryNames)
                if (directory.Name.Equals(documentDirectoryName))
                    DocumentsDirectory = directory;

            return DocumentsDirectory ?? DefaultNamedDocumentsDirectory();
        }

        private DirectoryInfo DefaultNamedDocumentsDirectory()
        {
            return WorkingDirectory.Content().WithSubDirectory(
                ArkadeConstants.DocumentDirectoryNames[0]
            ).DirectoryInfo();
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