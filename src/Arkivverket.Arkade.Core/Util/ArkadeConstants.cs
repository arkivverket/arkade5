namespace Arkivverket.Arkade.Core.Util
{
    public class ArkadeConstants
    {
        public const string SystemLogFileNameFormat = "arkade-{Date}.log";

        public const string NoarkihXmlFileName = "NOARKIH.XML";
        public const string AddmlXmlFileName = "addml.xml";
        public const string AddmlXsdFileName = "addml.xsd";
        public const string ArkivstrukturXmlFileName = "arkivstruktur.xml";
        public const string ArkivstrukturXsdFileName = "arkivstruktur.xsd";
        public const string MetadatakatalogXsdFileName = "metadatakatalog.xsd";
        public const string ArkadeXmlLogFileName = "arkade-log.xml";
        public const string EadXmlFileName = "ead.xml";
        public const string EacCpfXmlFileName = "eac-cpf.xml";
        public const string DiasPremisXmlFileName = "dias-premis.xml";
        public const string DiasPremisXsdFileName = "dias-premis.xsd";
        public const string DiasMetsXmlFileName = "dias-mets.xml";
        public const string DiasMetsXsdFileName = "dias-mets.xsd";
        public const string LogXmlFileName = "log.xml";
        public const string ArkivuttrekkXmlFileName = "arkivuttrekk.xml";
        public const string PublicJournalXmlFileName = "offentligJournal.xml";
        public const string PublicJournalXsdFileName = "offentligJournal.xsd";
        public const string RunningJournalXmlFileName = "loependeJournal.xml";
        public const string RunningJournalXsdFileName = "loependeJournal.xsd";
        public const string ChangeLogXmlFileName = "endringslogg.xml";
        public const string ChangeLogXsdFileName = "endringslogg.xsd";

        public const string AddmlXsdResource = "Arkivverket.Arkade.Core.ExternalModels.xsd.addml.xsd";
        public const string ArkivstrukturXsdResource = "Arkivverket.Arkade.Core.ExternalModels.xsd.arkivstruktur.xsd";
        public const string MetadatakatalogXsdResource = "Arkivverket.Arkade.Core.ExternalModels.xsd.metadatakatalog.xsd";
        public const string DiasPremisXsdResource = "Arkivverket.Arkade.Core.ExternalModels.xsd.DIAS_PREMIS.xsd";
        public const string DiasMetsXsdResource = "Arkivverket.Arkade.Core.ExternalModels.xsd.mets.xsd";
        public const string ChangeLogXsdResource = "Arkivverket.Arkade.Core.ExternalModels.xsd.endringslogg.xsd";
        public const string PublicJournalXsdResource = "Arkivverket.Arkade.Core.ExternalModels.xsd.offentligJournal.xsd";
        public const string RunningJournalXsdResource = "Arkivverket.Arkade.Core.ExternalModels.xsd.loependeJournal.xsd";

        public const string DirectoryPathBuiltInXsdResources = "Arkivverket.Arkade.Core.ExternalModels.xsd";

        public const string DirectoryNameArkadeProcessingAreaRoot = "arkade-tmp";
        public const string DirectoryNameArkadeProcessingAreaWork = "work";
        public const string DirectoryNameArkadeProcessingAreaLogs = "logs";
        public const string DirectoryNameTemporaryLogsLocation = ".arkade-tmplogs";
        public const string DirectoryNameRepositoryOperations = "repository_operations";
        public const string DirectoryNameContent = "content";
        public const string DirectoryNameResultOutputContainer = "Arkaderesultater";
        public const string DirectoryNameAppDataArkadeSubFolder = "Arkivverket";
        
        public static readonly string[] DocumentDirectoryNames =
            { "dokumenter", "DOKUMENTER", "dokument", "DOKUMENT" };

        public const string SiegfriedLinuxExecutable = "siegfried_linux";
        public const string SiegfriedMacOSXExecutable = "siegfried_mac";
        public const string SiegfriedWindowsExecutable = "siegfried.exe";
        public const string Noark5TestListFileName = "noark5-testlist.txt";
        public const string MetadataFileName = "arkade-ip-metadata.json";
        public const string FileFormatInfoFileName = "fileformatinfo.csv";
        public const string FileFormatInfoStatisticsFileName = "fileformatinfo-statistics.csv";
        public struct FileFormatInfoHeaders
        {
            public const string FileName = "Filnavn";
            public const string FileExtension = "Filendelse";
            public const string FormatId = "Format-ID";
            public const string FormatName = "Formatnavn";
            public const string FormatVersion = "Formatversjon";
            public const string MimeType = "MIME-type";
            public const string FileScanError = "Feil";
        }
        public struct FileFormatInfoStatisticsHeaders
        {
            public const string FileType = "Filtype";
            public const string Amount = "Antall";
        }

        public const string MetadataStandardLabelPlaceholder = "[standard_label]";
        public const string MetsHdrAgentMetaNoteKeyWord = "notescontent:";

        public const string ArkadeWebSiteUrl = "https://arkade.arkivverket.no";
    }
}