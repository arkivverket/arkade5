using System.IO;
using Arkivverket.Arkade.Util;
using static Arkivverket.Arkade.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core
{
    public class Archive
    {
        public Uuid Uuid { get; }
        public WorkingDirectory WorkingDirectory { get; }
        public ArchiveType ArchiveType { get; }
        private DirectoryInfo DocumentsDirectory { get; set; }

        public ArkadeFile ArchiveStructureFile => SetupXmlFile(ArkivstrukturXmlFileName);
        public ArkadeFile ArchiveStructureSchemaFile => SetupXmlFile(ArkivstrukturXsdFileName);
        public ArkadeFile AddmlFile => SetupXmlFile(AddmlXmlFileName);
        public ArkadeFile AddmlSchemaFile => SetupXmlFile(AddmlXsdFileName);
        public ArkadeFile MetadataCatalogSchemaFile => SetupXmlFile(MetadatakatalogXsdFileName);
        public ArkadeFile PublicJournalFile => SetupXmlFile(PublicJournalXmlFileName);
        public ArkadeFile PublicJournalSchemaFile => SetupXmlFile(PublicJournalXsdFileName);
        public ArkadeFile RunningJournalFile => SetupXmlFile(RunningJournalXmlFileName);
        public ArkadeFile RunningJournalSchemaFile => SetupXmlFile(RunningJournalXsdFileName);
        
        public Archive(ArchiveType archiveType, Uuid uuid, WorkingDirectory workingDirectory)
        {
            ArchiveType = archiveType;
            Uuid = uuid;
            WorkingDirectory = workingDirectory;
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
            foreach (string documentDirectoryName in DocumentDirectoryNames)
                if (directory.Name.Equals(documentDirectoryName))
                    DocumentsDirectory = directory;

            return DocumentsDirectory ?? DefaultNamedDocumentsDirectory();
        }

        private DirectoryInfo DefaultNamedDocumentsDirectory()
        {
            return WorkingDirectory.Content().WithSubDirectory(
                DocumentDirectoryNames[0]
            ).DirectoryInfo();
        }

        private ArkadeFile SetupXmlFile(string fileName)
        {
            FileInfo fileInfo = fileName == AddmlXmlFileName
                ? WorkingDirectory.AdministrativeMetadata().WithFile(fileName)
                : WorkingDirectory.Content().WithFile(fileName);

            return new ArkadeFile(fileInfo);
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