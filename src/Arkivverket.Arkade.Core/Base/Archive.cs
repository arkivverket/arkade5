using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using System.Linq;
using System.Reflection;
using Arkivverket.Arkade.Core.Util;
using Serilog;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Base
{
    public class Archive
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public Uuid Uuid { get; }
        public WorkingDirectory WorkingDirectory { get; }
        public ArchiveType ArchiveType { get; }
        private DirectoryInfo DocumentsDirectory { get; set; }
        private ReadOnlyDictionary<string, FileInfo> _documentFiles;
        public ReadOnlyDictionary<string, FileInfo> DocumentFiles => _documentFiles ?? GetDocumentFiles();
        public ArchiveXmlUnit AddmlXmlUnit { get; }
        public ArchiveDetails Details { get; }
        public List<ArchiveXmlUnit> XmlUnits { get; private set; }

        public Archive(ArchiveType archiveType, Uuid uuid, WorkingDirectory workingDirectory)
        {
            Uuid = uuid;

            ArchiveType = archiveType;

            WorkingDirectory = workingDirectory;

            AddmlXmlUnit = SetupAddmlXmlUnit();

            if (AddmlXmlUnit.File.Exists)
            {
                Details = new ArchiveDetails(this);

                if (archiveType == ArchiveType.Noark5)
                    SetupArchiveXmlUnits();
            }
        }

        public string GetInformationPackageFileName()
        {
            return Uuid + ".tar";
        }

        public string GetInfoXmlFileName()
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

        private ArchiveXmlUnit SetupAddmlXmlUnit()
        {
            FileInfo addmlFileInfo = WorkingDirectory.Content().WithFile(AddmlXmlFileName);

            if (!addmlFileInfo.Exists && ArchiveType == ArchiveType.Noark5)
                addmlFileInfo = WorkingDirectory.Content().WithFile(ArkivuttrekkXmlFileName);

            var archiveXmlFile = new ArchiveXmlFile(addmlFileInfo);

            FileInfo addmlXsdFileInfo = WorkingDirectory.Content().WithFile(AddmlXsdFileName);

            return new ArchiveXmlUnit(archiveXmlFile, new UserProvidedXmlSchema(addmlXsdFileInfo));
        }

        private void SetupArchiveXmlUnits()
        {
            XmlUnits = new List<ArchiveXmlUnit>();

            foreach (KeyValuePair<string, IEnumerable<string>> documentedXmlUnit in Details.DocumentedXmlUnits)
            {
                string xmlFileName = documentedXmlUnit.Key;

                IEnumerable<string> xmlSchemaNames = documentedXmlUnit.Value;

                var archiveXmlFile = new ArchiveXmlFile(WorkingDirectory.Content().WithFile(xmlFileName));

                var archiveXmlSchemas = new List<ArchiveXmlSchema>();

                foreach (string xmlSchemaName in xmlSchemaNames)
                {
                    FileInfo xmlSchemaFile = WorkingDirectory.Content().WithFile(xmlSchemaName);

                    ArchiveXmlSchema archiveXmlSchema = ArchiveXmlSchema.Create(xmlSchemaFile);

                    archiveXmlSchemas.Add(archiveXmlSchema);
                }

                XmlUnits.Add(new ArchiveXmlUnit(archiveXmlFile, archiveXmlSchemas));
            }
        }

        private ReadOnlyDictionary<string, FileInfo> GetDocumentFiles()
        {
            Log.Information("Registering document files.");

            var documentFiles = new Dictionary<string, FileInfo>();

            DirectoryInfo documentsDirectory = GetDocumentsDirectory();

            if (documentsDirectory.Exists)
            {
                FileInfo[] fileInfos = documentsDirectory.GetFiles("*", SearchOption.AllDirectories);

                string documentsDirectoryParentFullPath = documentsDirectory.Parent?.FullName + '/';

                documentFiles = fileInfos.ToDictionary(f =>
                {
                    string relativeName = f.FullName.Substring(documentsDirectoryParentFullPath.Length);
                    return relativeName.Replace('\\', '/');
                });
            }

            // Instantiate field for next access:
            _documentFiles = new ReadOnlyDictionary<string, FileInfo>(documentFiles);

            Log.Information($"{documentFiles.Count} document files registered.");

            return _documentFiles;
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