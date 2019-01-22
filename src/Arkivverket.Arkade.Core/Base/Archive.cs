using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using System.Linq;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Base
{
    public class Archive
    {
        public Uuid Uuid { get; }
        public WorkingDirectory WorkingDirectory { get; }
        public ArchiveType ArchiveType { get; }
        private DirectoryInfo DocumentsDirectory { get; set; }
        public ArchiveXmlUnit AddmlXmlUnit { get; }
        public ArchiveDetails Details { get; }
        public List<ArchiveXmlUnit> XmlUnits { get; private set; }

        public Archive(ArchiveType archiveType, Uuid uuid, WorkingDirectory workingDirectory)
        {
            Uuid = uuid;

            ArchiveType = archiveType;

            WorkingDirectory = workingDirectory;

            AddmlXmlUnit = SetupAddmlXmlUnit();

            Details = new ArchiveDetails(this);

            if (archiveType == ArchiveType.Noark5)
                SetupArchiveXmlUnits();
        }

        public string GetInformationPackageFileName()
        {
            return Uuid + ".tar";
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

            if (!addmlFileInfo.Exists)
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
    }

    public enum ArchiveType
    {
        Noark3,
        Noark4,
        Noark5,
        Fagsystem,
    }
}