using System.IO;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core
{
    public class Archive
    {
        public const string ContentDescriptionFileNameNoark5 = "arkivstruktur.xml";

        public Uuid Uuid { get; private set; }
        public DirectoryInfo WorkingDirectory { get; private set; }
        public ArchiveType ArchiveType { get; private set; }

        public Archive(ArchiveType archiveType, Uuid uuid, DirectoryInfo workingDirectory)
        {
            ArchiveType = archiveType;
            Uuid = uuid;
            WorkingDirectory = workingDirectory;
        }

        public string GetContentDescriptionFileName()
        {
            return $"{WorkingDirectory}{Path.DirectorySeparatorChar}{ContentDescriptionFileNameNoark5}"; // noark5 filename
        }

        public string GetStructureDescriptionFileName()
        {
            string structureFilename = WorkingDirectory.FullName + Path.DirectorySeparatorChar;
            if (ArchiveType.Equals(ArchiveType.Noark5))
            {
                structureFilename = structureFilename + "arkivuttrekk.xml";
            }
            else
            {
                structureFilename = structureFilename + ArkadeConstants.AddmlXmlFileName;
            }

            return structureFilename;
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