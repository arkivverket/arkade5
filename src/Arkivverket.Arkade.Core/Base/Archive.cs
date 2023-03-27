using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public bool IsTarArchive => ArchiveFileFullName != null;

        public Uuid Uuid { get; }
        public WorkingDirectory WorkingDirectory { get; }
        public ArchiveType ArchiveType { get; }
        private DirectoryInfo DocumentsDirectory { get; set; }
        private ReadOnlyDictionary<string, DocumentFile> _documentFiles;

        public ReadOnlyDictionary<string, DocumentFile> DocumentFiles => _documentFiles ??
                                                                         (IsTarArchive
                                                                             ? GetDocumentFilesFromTar()
                                                                             : GetDocumentFiles());
        public AddmlXmlUnit AddmlXmlUnit { get; }
        public AddmlInfo AddmlInfo { get; }
        public IArchiveDetails Details { get; }
        public List<ArchiveXmlUnit> XmlUnits { get; private set; }

        public Archive(ArchiveType archiveType, Uuid uuid, WorkingDirectory workingDirectory,
            IStatusEventHandler statusEventHandler, string archiveFileFullName=null)
        {
            _statusEventHandler = statusEventHandler;
            ArchiveFileFullName = archiveFileFullName;

            Uuid = uuid;

            ArchiveType = archiveType;

            WorkingDirectory = workingDirectory;

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

        public string GetInformationPackageFileName()
        {
            return Uuid + ".tar";
        }

        public string GetSubmissionDescriptionFileName()
        {
            return Uuid + ".xml";
        }

        public ArchiveXmlFile GetArchiveXmlFile(string fileName)
        {
            return XmlUnits.FirstOrDefault(xmlUnit => xmlUnit.File.Name.Equals(fileName))?.File;
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

        private ReadOnlyDictionary<string, DocumentFile> GetDocumentFiles()
        {
            Log.Information("Registering document files.");

            var documentFiles = new Dictionary<string, DocumentFile>();

            DirectoryInfo documentsDirectory = GetDocumentsDirectory();

            if (documentsDirectory.Exists)
            {
                foreach (FileInfo documentFileInfo in documentsDirectory.GetFiles("*", SearchOption.AllDirectories))
                {
                    string relativePath = documentsDirectory.Parent != null
                        ? Path.GetRelativePath(documentsDirectory.Parent.FullName, documentFileInfo.FullName)
                        : documentFileInfo.FullName;

                    documentFiles.Add(relativePath.Replace('\\', '/'), new DocumentFile(documentFileInfo));
                }
            }

            // Instantiate field for next access:
            _documentFiles = new ReadOnlyDictionary<string, DocumentFile>(documentFiles);

            Log.Information($"{documentFiles.Count} document files registered.");

            return _documentFiles;
        }

        private ReadOnlyDictionary<string, DocumentFile> GetDocumentFilesFromTar()
        {
            var documentFiles = new Dictionary<string, DocumentFile>();

            var checksumGenerator = new Sha256ChecksumGenerator();

            using var tarInputStream = new TarInputStream(File.OpenRead(ArchiveFileFullName), Encoding.UTF8);
            while (tarInputStream.GetNextEntry() is { Name: { } } entry)
            {
                string entryName = entry.Name;

                if (!entryName.StartsWith($"{Uuid}/content/dokumenter/") || entry.IsDirectory)
                    continue;

                checksumGenerator.Initialize();

                var tempBuffer = new byte[32 * 1024];

                while (true)
                {
                    int numRead = tarInputStream.Read(tempBuffer, 0, tempBuffer.Length);
                    if (numRead <= 0)
                    {
                        break;
                    }

                    checksumGenerator.TransformBlock(tempBuffer, numRead);
                }
                string checksum = checksumGenerator.GenerateChecksum();

                var documentFile = new DocumentFile(null)
                {
                    CheckSum = checksum,
                    Size = entry.Size,
                    Extension = Path.GetExtension(entry.Name).Replace(".", string.Empty),
                    CreationTime = entry.ModTime,
                };

                string relativeEntryName = entryName.Remove(0, entryName.IndexOf("dokumenter", StringComparison.OrdinalIgnoreCase));

                documentFiles.Add(relativeEntryName, documentFile);
            }

            // Instantiate field for next access:
            _documentFiles = new ReadOnlyDictionary<string, DocumentFile>(documentFiles);

            return _documentFiles;
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