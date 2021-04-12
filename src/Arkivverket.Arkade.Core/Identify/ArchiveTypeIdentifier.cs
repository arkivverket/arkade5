using System;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.ExternalModels.Mets;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Identify
{
    public class ArchiveTypeIdentifier : IArchiveTypeIdentifier
    {
        public ArchiveType? IdentifyTypeOfChosenArchiveDirectory(string archiveDirectoryName)
        {
            var archiveDirectory = new DirectoryInfo(archiveDirectoryName);

            FileInfo[] archiveContentFiles = archiveDirectory.GetFiles();

            FileInfo addmlFile = archiveContentFiles.FirstOrDefault(
                f => f.Name.Equals(ArkadeConstants.AddmlXmlFileName));

            if (archiveContentFiles.Any(f => f.Name.Equals(ArkadeConstants.ArkivuttrekkXmlFileName)))
                return ArchiveType.Noark5;

            if (archiveContentFiles.Any(f => f.Extension.Equals(".siard")) || TypeOfChosenArchiveDirectoryIsSiard(archiveDirectory))
                return ArchiveType.Siard;

            if (!File.Exists(addmlFile?.FullName))
                return null;

            var addml = SerializeUtil.DeserializeFromFile<addml>(addmlFile.FullName);

            if (TypeOfChosenArchiveDirectoryIsNoark3(addml))
                return ArchiveType.Noark3;

            if (TypeOfChosenArchiveDirectoryIsFagsystem(addml))
                return ArchiveType.Fagsystem;

            if (TypeOfChosenArchiveDirectoryIsNoark5(addml))
                return ArchiveType.Noark5;

            return null;
        }

        public ArchiveType? IdentifyTypeOfChosenArchiveFile(string archiveFileName)
        {
            if (Path.GetExtension(archiveFileName).Equals(".siard"))
                return ArchiveType.Siard;

            string infoFilePath = archiveFileName.Replace(Path.GetExtension(archiveFileName), ".xml");

            if (!File.Exists(infoFilePath))
            {
                return null;
            }

            var infoFile = SerializeUtil.DeserializeFromFile<mets>(infoFilePath);

            metsTypeMetsHdrAgent archiveExtractionTypeAgent = infoFile.metsHdr.agent.FirstOrDefault(
                a => a.ROLE == metsTypeMetsHdrAgentROLE.OTHER &&
                     a.OTHERROLE == metsTypeMetsHdrAgentOTHERROLE.PRODUCER &&
                     a.TYPE == metsTypeMetsHdrAgentTYPE.OTHER &&
                     a.OTHERTYPE == metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE);

            if (archiveExtractionTypeAgent == default)
                return null;

            foreach (string note in archiveExtractionTypeAgent.note)
            {
                if (note.Contains("fagsystem", StringComparison.OrdinalIgnoreCase))
                    return ArchiveType.Fagsystem;

                if (note.Contains("siard", StringComparison.OrdinalIgnoreCase))
                    return ArchiveType.Siard;

                if (note.Contains("noark", StringComparison.OrdinalIgnoreCase) && note.Contains("3"))
                    return ArchiveType.Noark3;

                if (note.Contains("noark", StringComparison.OrdinalIgnoreCase) && note.Contains("5"))
                    return ArchiveType.Noark5;
            }

            return null;
        }

        private static bool TypeOfChosenArchiveDirectoryIsNoark3(addml addml)
        {
            string archiveExtractionType = addml.dataset[0].reference?.context?.additionalElements?
                .additionalElement
                .FirstOrDefault(additionalElement => additionalElement.name == "systemType")?
                .value;

            return archiveExtractionType != null &&
                   archiveExtractionType.Contains("noark", StringComparison.OrdinalIgnoreCase) &&
                   archiveExtractionType.Contains("3");
        }

        private static bool TypeOfChosenArchiveDirectoryIsNoark5(addml addml)
        {
            string archiveExtractionType = addml.dataset[0].dataObjects?.dataObject[0].properties
                .FirstOrDefault(p => p.name.Equals("info"))?.properties
                .FirstOrDefault(p => p.name.Equals("type"))?.value;

            return archiveExtractionType != null &&
                   archiveExtractionType.Contains("noark", StringComparison.OrdinalIgnoreCase) &&
                   archiveExtractionType.Contains("5");
        }

        private static bool TypeOfChosenArchiveDirectoryIsFagsystem(addml addml)
        {
            return addml.dataset[0].flatFiles != null;
        }

        private static bool TypeOfChosenArchiveDirectoryIsSiard(DirectoryInfo archiveDirectory)
        {
            bool? isSiard = archiveDirectory.GetDirectories()
                .FirstOrDefault(d => d.Name.Equals(ArkadeConstants.SiardHeaderDirectoryName))?.GetFiles()
                .Any(f => f.Name.Equals(ArkadeConstants.SiardMetadataXmlFileName));

            if (isSiard == null)
                return false;

            return (bool)isSiard;
        }
    }
}
