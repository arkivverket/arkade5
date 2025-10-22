using System;
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
    public abstract class Archive
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        private static IStatusEventHandler _statusEventHandler;

        public DirectoryInfo ProcessingDirectory { get; }

        public bool IsNoark5TarArchive => InputDiasPackage?.TarFile != null && ArchiveType is ArchiveType.Noark5;
        public ArkadeDirectory Content { get; }
        //public ArchiveContent Content { get; }
        public InputDiasPackage InputDiasPackage { get; }
        public OutputDiasPackage OutputDiasPackage { get; set; }
        public ArchiveType ArchiveType { get; }
        private DirectoryInfo DocumentsDirectory { get; set; }
        private string DocumentsDirectoryName { get; set; }
        internal DocumentFiles DocumentFiles { get; }
        public AddmlXmlUnit AddmlXmlUnit { get; }
        public AddmlInfo AddmlInfo { get; }
        public IArchiveDetails Details { get; }
        public List<ArchiveXmlUnit> XmlUnits { get; private set; }
        public TestSession TestSession { get; set; }

        protected Archive(ArchiveType archiveType, DirectoryInfo archiveExtractionDirectory,
            DirectoryInfo processingDirectory, IStatusEventHandler statusEventHandler,InputDiasPackage inputDiasPackage = null)
        {
            _statusEventHandler = statusEventHandler;

            ArchiveType = archiveType;

            Content = new ArkadeDirectory(archiveExtractionDirectory);
            
            ProcessingDirectory = processingDirectory;

            InputDiasPackage = inputDiasPackage;
            
            if (archiveType == ArchiveType.Siard)
            {
                Details = SetupSiardArchiveDetails(Content);
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

                DocumentFiles = InputDiasPackage == null
                    ? new DocumentFiles(GetDocumentsDirectory())
                    : new DocumentFiles(InputDiasPackage.TarFile.FullName);
            }
        }

        private static IArchiveDetails SetupSiardArchiveDetails(ArkadeDirectory content)
        {
            FileInfo siardArchiveFile = content.DirectoryInfo().GetFiles("*.siard").FirstOrDefault();
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

            foreach (DirectoryInfo directory in Content.DirectoryInfo().EnumerateDirectories())
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
                var tarInputStream = new TarInputStream(File.OpenRead(InputDiasPackage.TarFile.FullName!), Encoding.UTF8);

                string archiveRootDirectoryName = Path.GetFileNameWithoutExtension(InputDiasPackage.TarFile.FullName);

                while (tarInputStream.GetNextEntry() is { Name: not null } entry)
                {
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
            return Content.WithSubDirectory(
                DocumentDirectoryNames[0]
            ).DirectoryInfo();
        }

        private AddmlXmlUnit SetupAddmlXmlUnit()
        {
            FileInfo addmlFileInfo = Content.WithFile(AddmlXmlFileName);

            if (!addmlFileInfo.Exists && ArchiveType == ArchiveType.Noark5)
                addmlFileInfo = Content.WithFile(ArkivuttrekkXmlFileName);

            var addmlXmlFile = new ArchiveXmlFile(addmlFileInfo);

            FileInfo addmlXsdFileInfo = Content.WithFile(AddmlXsdFileName);

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
                    documentedXmlSchemas.Select(s => ArchiveXmlSchema.Create(Content.WithFile(s)));

                IEnumerable<ArchiveXmlSchema> arkadeSuppliedSchemas = Details.StandardXmlUnits[documentedXmlFileName]
                    .Except(documentedXmlSchemas).Select(s => ArchiveXmlSchema
                        .Create(s, AddmlVersionIsSupported() ? Details.ArchiveStandard : LatestNoark5Version));

                var archiveXmlSchemas = new List<ArchiveXmlSchema>(userProvidedSchemas.Concat(arkadeSuppliedSchemas));

                var archiveXmlFile = new ArchiveXmlFile(Content.WithFile(documentedXmlFileName));

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

    [Flags]
    public enum ArchiveType
    {
        Noark3,
        Noark4,
        Noark5,
        Fagsystem,
        Siard,
    }
}