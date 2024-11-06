using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using Arkivverket.Arkade.Core.Base.Siard;
using Arkivverket.Arkade.Core.ExternalModels.Metadata;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using ICSharpCode.SharpZipLib.Tar;
using Serilog;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Base
{
    public class Archive
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        private static IStatusEventHandler _statusEventHandler;

        internal string? ArchiveFileFullName { get; }

        public bool IsNoark5TarArchive => ArchiveFileFullName != null && ArchiveType is ArchiveType.Noark5;

        public WorkingDirectory WorkingDirectory { get; }
        public ArchiveType ArchiveType { get; }
        private DirectoryInfo DocumentsDirectory { get; set; }
        private string DocumentsDirectoryName { get; set; }
        internal DocumentFiles DocumentFiles { get; }
        public AddmlXmlUnit AddmlXmlUnit { get; }
        public AddmlInfo AddmlInfo { get; }
        public IArchiveDetails Details { get; }
        public List<ArchiveXmlUnit> XmlUnits { get; private set; }

        public Archive(ArchiveType archiveType, WorkingDirectory workingDirectory,
            IStatusEventHandler statusEventHandler, string archiveFileFullName=null)
        {
            _statusEventHandler = statusEventHandler;

            ArchiveType = archiveType;

            WorkingDirectory = workingDirectory;

            ArchiveFileFullName = archiveFileFullName;

            if (archiveType == ArchiveType.Siard)
            {
                Details = SetupSiardArchiveDetails(workingDirectory);
                return;
            }
            
            AddmlXmlUnit = SetupAddmlXmlUnit();

            if (!AddmlXmlUnit.File.Exists)
                return;

            using Stream xmlSchemaStream = AddmlXmlUnit.HasNoDefinedSchema()
                ? ResourceUtil.GetResourceAsStream(AddmlXsdResource)
                : AddmlXmlUnit.Schema.AsStream();

            AddmlInfo = AddmlUtil.ReadFromFile(AddmlXmlUnit.File.FullName, xmlSchemaStream);

            Details = new ArchiveDetails(AddmlInfo.Addml);

            if (archiveType == ArchiveType.Noark5)
            {
                if (AddmlXmlUnit.HasNoDefinedSchema())
                    AddmlXmlUnit.Schema = new ArkadeBuiltInXmlSchema(AddmlXsdFileName, Details.ArchiveStandard);

                SetupArchiveXmlUnits();

                DocumentFiles = archiveFileFullName == null
                    ? new DocumentFiles(GetDocumentsDirectory())
                    : new DocumentFiles(archiveFileFullName);
            }
        }

        private static IArchiveDetails SetupSiardArchiveDetails(WorkingDirectory workingDirectory)
        {
            FileInfo siardArchiveFile = workingDirectory.Content().DirectoryInfo().GetFiles("*.siard").FirstOrDefault();
            if (siardArchiveFile == null)
                throw new ArkadeException("Siard file not found");
            if (!siardArchiveFile.Exists)
                throw new ArkadeException(string.Format(ExceptionMessages.FileNotFound, siardArchiveFile.FullName));

            if (new SiardArchiveReader().TryDeserializeToSiard2_1(siardArchiveFile.FullName, out siardArchive siard2Archive, out string errorMessage))
                return new SiardArchiveDetails(siard2Archive);

            _statusEventHandler?.RaiseEventOperationMessage(null,
                string.Format(SiardMessages.DeserializationUnsuccessfulMessage, SiardMetadataXmlFileName, "2.1", errorMessage),
                OperationMessageStatus.Error);

            return null;
        }

        public DirectoryInfo GetTestReportDirectory()
        {
            return WorkingDirectory.RepositoryOperations().WithSubDirectory(OutputFileNames.TestReportDirectory)
                .DirectoryInfo();
        }



        public ArchiveXmlFile GetArchiveXmlFile(string fileName)
        {
            return XmlUnits.FirstOrDefault(xmlUnit => xmlUnit.File.Name.Equals(fileName))?.File;
        }

        public DirectoryInfo GetDocumentsDirectory()
        {
            if (IsNoark5TarArchive)
                return null;

            if (DocumentsDirectory != null)
                return DocumentsDirectory;

            foreach (DirectoryInfo directory in WorkingDirectory.Content().DirectoryInfo().EnumerateDirectories())
            foreach (string documentDirectoryName in DocumentDirectoryNames)
                if (directory.Name.Equals(documentDirectoryName))
                    DocumentsDirectory = directory;

            return DocumentsDirectory ?? DefaultNamedDocumentsDirectory();
        }

        public string GetDocumentsDirectoryName()
        {
            if (DocumentsDirectoryName != null)
                return DocumentsDirectoryName;

            if (IsNoark5TarArchive)
            {
                var tarInputStream = new TarInputStream(File.OpenRead(ArchiveFileFullName!), Encoding.UTF8);

                while (tarInputStream.GetNextEntry() is { Name: not null } entry)
                {
                    string archiveRootDirectoryName = Path.GetFileNameWithoutExtension(ArchiveFileFullName);

                    if (!entry.IsDirectory && entry.IsNoark5DocumentsEntry(archiveRootDirectoryName))
                    {
                        DocumentsDirectoryName = PathUtil.GetChild(DirectoryNameContent, entry.Name);
                        break;
                    }
                }

                return DocumentsDirectoryName ?? DefaultNamedDocumentsDirectory().Name;
            }

            return GetDocumentsDirectory()?.Name;
        }

        private DirectoryInfo DefaultNamedDocumentsDirectory()
        {
            return WorkingDirectory.Content().WithSubDirectory(
                DocumentDirectoryNames[0]
            ).DirectoryInfo();
        }

        private AddmlXmlUnit SetupAddmlXmlUnit()
        {
            FileInfo addmlFileInfo = WorkingDirectory.Content().WithFile(AddmlXmlFileName);

            if (!addmlFileInfo.Exists && ArchiveType == ArchiveType.Noark5)
                addmlFileInfo = WorkingDirectory.Content().WithFile(ArkivuttrekkXmlFileName);

            var addmlXmlFile = new ArchiveXmlFile(addmlFileInfo);

            FileInfo addmlXsdFileInfo = WorkingDirectory.Content().WithFile(AddmlXsdFileName);

            ArchiveXmlSchema addmlSchema = addmlXsdFileInfo.Exists
                ? ArchiveXmlSchema.Create(addmlXsdFileInfo)
                : null;

            return new AddmlXmlUnit(addmlXmlFile, addmlSchema);
        }

        private void SetupArchiveXmlUnits()
        {
            XmlUnits = new List<ArchiveXmlUnit>();

            foreach ((string documentedXmlFileName, IEnumerable<string> documentedXmlSchemas) in Details.DocumentedXmlUnits)
            {
                IEnumerable<ArchiveXmlSchema> userProvidedSchemas =
                    documentedXmlSchemas.Select(s => ArchiveXmlSchema.Create(WorkingDirectory.Content().WithFile(s)));

                IEnumerable<ArchiveXmlSchema> arkadeSuppliedSchemas = Details.StandardXmlUnits[documentedXmlFileName]
                    .Except(documentedXmlSchemas).Select(s => ArchiveXmlSchema
                        .Create(s, AddmlVersionIsSupported() ? Details.ArchiveStandard : LatestNoark5Version));

                var archiveXmlSchemas = new List<ArchiveXmlSchema>(userProvidedSchemas.Concat(arkadeSuppliedSchemas));

                var archiveXmlFile = new ArchiveXmlFile(WorkingDirectory.Content().WithFile(documentedXmlFileName));

                XmlUnits.Add(new ArchiveXmlUnit(archiveXmlFile, archiveXmlSchemas));
            }
        }

        private bool AddmlVersionIsSupported()
        {
            if (SupportedNoark5Versions.Contains(Details.ArchiveStandard))
                return true;

            Log.Warning(string.Format(Noark5Messages.Noark5VersionNotSupportedForBuiltInSchemas, Details.ArchiveStandard));
            return false;
        }
    }

    public enum ArchiveType
    {
        Noark3,
        Noark4,
        Noark5,
        Fagsystem,
        Siard,
    }
}