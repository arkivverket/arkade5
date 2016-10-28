using System.IO;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Util;
using Serilog;

namespace Arkivverket.Arkade.Core
{
    public class Archive
    {
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
            return $"{WorkingDirectory}{Path.DirectorySeparatorChar}arkivstruktur.xml"; // noark5 filename
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