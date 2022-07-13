namespace Arkivverket.Arkade.Core.Util
{
    public class ArkadeConstants
    {
        public const string SystemLogFileNameFormat = "arkade-.log";

        public const string NoarkihXmlFileName = "NOARKIH.XML";
        public const string AddmlXmlFileName = "addml.xml";
        public const string AddmlXsdFileName = "addml.xsd";
        public const string ArkivstrukturXmlFileName = "arkivstruktur.xml";
        public const string ArkivstrukturXsdFileName = "arkivstruktur.xsd";
        public const string MetadatakatalogXsdFileName = "metadatakatalog.xsd";
        public const string ArkadeXmlLogFileName = "arkade-log.xml";
        public const string EadXmlFileName = "ead.xml";
        public const string EadXsdFileName = "ead.xsd";
        public const string EacCpfXmlFileName = "eac-cpf.xml";
        public const string EacCpfXsdFileName = "eac-cpf.xsd";
        public const string DiasPremisXmlFileName = "dias-premis.xml";
        public const string DiasPremisXsdFileName = "dias-premis.xsd";
        public const string DiasMetsXmlFileName = "dias-mets.xml";
        public const string DiasMetsXsdFileName = "dias-mets.xsd";
        public const string LogXmlFileName = "log.xml";
        public const string ArkivuttrekkXmlFileName = "arkivuttrekk.xml";
        public const string MetadataXmlFileName = "metadata.xml";
        public const string PublicJournalXmlFileName = "offentligJournal.xml";
        public const string PublicJournalXsdFileName = "offentligJournal.xsd";
        public const string RunningJournalXmlFileName = "loependeJournal.xml";
        public const string RunningJournalXsdFileName = "loependeJournal.xsd";
        public const string ChangeLogXmlFileName = "endringslogg.xml";
        public const string ChangeLogXsdFileName = "endringslogg.xsd";
        public const string SystemhaandbokPdfFileName = "systemhåndbok.pdf";

        public const string SiardHeaderDirectoryName = "header";
        public const string SiardMetadataXmlFileName = "metadata.xml";
        public const string SiardMetadataXsdFileName = "metadata.xsd";

        public const string AddmlXsdResource = "Arkivverket.Arkade.Core.ExternalModels.xsd.addml.xsd";
        public const string Addml82XsdResource = "Arkivverket.Arkade.Core.ExternalModels.xsd.addml82.xsd";
        public const string ArkivstrukturXsdResource = "Arkivverket.Arkade.Core.ExternalModels.xsd.arkivstruktur.xsd";
        public const string MetadatakatalogXsdResource = "Arkivverket.Arkade.Core.ExternalModels.xsd.metadatakatalog.xsd";
        public const string DiasPremisXsdResource = "Arkivverket.Arkade.Core.ExternalModels.xsd.DIAS_PREMIS.xsd";
        public const string DiasMetsXsdResource = "Arkivverket.Arkade.Core.ExternalModels.xsd.DIAS_METS.xsd";
        public const string SubmissionDescriptionXsdResource = "Arkivverket.Arkade.Core.ExternalModels.xsd.submissionDescription.xsd";
        public const string ChangeLogXsdResource = "Arkivverket.Arkade.Core.ExternalModels.xsd.endringslogg.xsd";
        public const string PublicJournalXsdResource = "Arkivverket.Arkade.Core.ExternalModels.xsd.offentligJournal.xsd";
        public const string RunningJournalXsdResource = "Arkivverket.Arkade.Core.ExternalModels.xsd.loependeJournal.xsd";

        public const string DirectoryPathBuiltInXsdResources = "Arkivverket.Arkade.Core.ExternalModels.xsd";
        public const string DirectoryPathNoark5XsdResources = "Arkivverket.Arkade.Core.ExternalModels.Noark5.{0}";

        public const string LatestNoark5Version = "5.0";
        public static readonly string[] SupportedNoark5Versions =
            { "3.1", "4.0", "5.0"};

        public const string BuiltInAddmlSchemaVersion = "8.3";
        public const string DefaultAddmlVersion = "83";

        public const string DirectoryNameArkadeProcessingAreaRoot = "arkade-tmp";
        public const string DirectoryNameArkadeProcessingAreaWork = "work";
        public const string DirectoryNameArkadeProcessingAreaLogs = "logs";
        public const string DirectoryNameTemporaryLogsLocation = ".arkade-tmplogs";
        public const string DirectoryNameRepositoryOperations = "repository_operations";
        public const string DirectoryNameContent = "content";
        public const string DirectoryNameAppDataArkadeSubFolder = "Arkivverket";
        public const string DirectoryNameDescriptiveMetadata = "descriptive_metadata";
        public const string DirectoryNameAdministrativeMetadata = "administrative_metadata";

        public const string DirectoryNameThirdPartySoftware = "ThirdPartySoftware";
        public const string DirectoryNameSiegfried = "Siegfried";
        public const string DirectoryNameDbptk = "DBPTK";

        public static readonly string[] DocumentDirectoryNames =
            { "dokumenter", "DOKUMENTER", "dokument", "DOKUMENT" };

        public const string SiegfriedLinuxExecutable = "siegfried_linux";
        public const string SiegfriedMacOSXExecutable = "siegfried_mac";
        public const string SiegfriedWindowsExecutable = "siegfried.exe";

        public const string MetadataStandardLabelPlaceholder = "[standard_label]";
        public const string MetsHdrAgentMetaNoteKeyWord = "notescontent:";

        public const string ArkadeWebSiteUrl = "https://arkade.arkivverket.no";

        public const string DbptkLibraryDownloadUrl =
            "https://github.com/keeps/dbptk-developer/releases/";

        public static readonly string[] SuppressedDbptkWarningMessages =
        {
            "WARN  Could not create report file in current working directory. Attempting to use a temporary file",
        };
    }
}