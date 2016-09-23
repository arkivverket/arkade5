using System.IO;

namespace Arkivverket.Arkade.Core
{
    public class Archive
    {

        public Archive(string uuid, string workingDirectory)
        {
            Uuid = uuid;
            WorkingDirectory = workingDirectory;
        }

        public string Uuid { get; private set; }
        public string WorkingDirectory { get; private set; }
        public ArchiveType ArchiveType { get; set; }

        public string GetContentDescriptionFileName()
        {
            return $"{WorkingDirectory}{Path.DirectorySeparatorChar}arkivstruktur.xml"; // noark5 filename
        }

        public string GetStructureDescriptionFileName()
        {
            string structureFilename = WorkingDirectory + Path.DirectorySeparatorChar;
            if (ArchiveType.Equals(ArchiveType.Noark5))
                structureFilename = structureFilename + "arkivuttrekk.xml";
            else
                structureFilename = structureFilename + "addml.xml";

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