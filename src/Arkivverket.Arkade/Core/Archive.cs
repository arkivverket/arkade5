using System.IO;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core
{
    public class Archive
    {
        public const string ContentDescriptionFileNameNoark5 = "arkivstruktur.xml";

        public Uuid Uuid { get; private set; }
        public WorkingDirectory WorkingDirectory { get; private set; }
        public ArchiveType ArchiveType { get; private set; }

        public Archive(ArchiveType archiveType, Uuid uuid, WorkingDirectory workingDirectory)
        {
            ArchiveType = archiveType;
            Uuid = uuid;
            WorkingDirectory = workingDirectory;
        }

        public string GetContentDescriptionFileName()
        {
            return WorkingDirectory.Content().WithFile(ContentDescriptionFileNameNoark5).FullName;
        }

        public string GetStructureDescriptionFileName()
        {
            string structureFilename;
            if (ArchiveType.Equals(ArchiveType.Noark5))
            {
                structureFilename = WorkingDirectory.Content().WithFile("arkivuttrekk.xml").FullName;
            }
            else if (ArchiveType.Equals(ArchiveType.Noark4))
            {
                structureFilename = WorkingDirectory.ContentWorkDirectory().WithFile(ArkadeConstants.AddmlXmlFileName).FullName;
            }
            else
            {
                structureFilename = WorkingDirectory.Content().WithFile(ArkadeConstants.AddmlXmlFileName).FullName;
            }
            return structureFilename;
        }

        public FileInfo GetInformationPackageFileName()
        {
            return WorkingDirectory.Root().WithFile(Uuid.ToString() + ".tar");
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