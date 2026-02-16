using System;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.ExternalModels.SubmissionDescription;
using Arkivverket.Arkade.Core.Util;
using Serilog;

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
            {
                LogWarning(ArkadeConstants.AddmlXmlFileName + " was not found in the chosen archive directory");
                return null;
            }

            try
            {
                var addml = SerializeUtil.DeserializeFromFile<addml>(addmlFile.FullName);
                if (TypeOfChosenArchiveDirectoryIsNoark3(addml))
                    return ArchiveType.Noark3;

                if (TypeOfChosenArchiveDirectoryIsNoark4(addml))
                    return ArchiveType.Noark4;

                if (TypeOfChosenArchiveDirectoryIsSpecializedSystem(addml))
                    return ArchiveType.SpecializedSystem;

                if (TypeOfChosenArchiveDirectoryIsNoark5(addml))
                    return ArchiveType.Noark5;
            }
            catch(Exception exception)
            {
                LogWarning(exception.Message);
                return null;
            }
            
            LogWarning();
            return null;
        }

        public ArchiveType? IdentifyTypeOfChosenArchiveFile(string archiveFileName)
        {
            if (Path.GetExtension(archiveFileName).Equals(".siard"))
                return ArchiveType.Siard;

            string infoFilePath = archiveFileName.Replace(Path.GetExtension(archiveFileName), ".xml");

            if (!File.Exists(infoFilePath))
            {
                LogWarning($"No info file found for archive package file [path: {archiveFileName}]");
                return null;
            }

            var infoFile = SerializeUtil.DeserializeFromFile<mets>(infoFilePath);

            metsTypeMetsHdrAgent archiveExtractionTypeAgent = infoFile.metsHdr.agent.FirstOrDefault(
                a => a.ROLE == metsTypeMetsHdrAgentROLE.OTHER &&
                     a.OTHERROLE == metsTypeMetsHdrAgentOTHERROLE.PRODUCER &&
                     a.TYPE == metsTypeMetsHdrAgentTYPE.OTHER &&
                     a.OTHERTYPE == metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE);

            if (archiveExtractionTypeAgent == default)
            {
                LogWarning($"No archive type information found in package info file [path: {archiveFileName}]");
                return null;
            }

            foreach (string note in archiveExtractionTypeAgent.note)
            {
                if (note.Contains("fagsystem", StringComparison.OrdinalIgnoreCase))
                    return ArchiveType.SpecializedSystem;

                if (note.Contains("siard", StringComparison.OrdinalIgnoreCase))
                    return ArchiveType.Siard;

                if (note.Contains("noark", StringComparison.OrdinalIgnoreCase) && note.Contains("3"))
                    return ArchiveType.Noark3;

                if (note.Contains("noark", StringComparison.OrdinalIgnoreCase) && note.Contains("4"))
                    return ArchiveType.Noark4;

                if (note.Contains("noark", StringComparison.OrdinalIgnoreCase) && note.Contains("5"))
                    return ArchiveType.Noark5;
            }

            LogWarning();
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

        private static bool TypeOfChosenArchiveDirectoryIsNoark4(addml addml)
        {
            string archiveExtractionType = addml.dataset[0].reference?.context?.additionalElements?
                .additionalElement
                .FirstOrDefault(additionalElement => additionalElement.name == "systemType")?
                .value;

            return archiveExtractionType != null &&
                   archiveExtractionType.Contains("noark", StringComparison.OrdinalIgnoreCase) &&
                   archiveExtractionType.Contains("4");
        }

        private static bool TypeOfChosenArchiveDirectoryIsNoark5(addml addml)
        {
            string archiveExtractionType = addml.dataset[0].dataObjects?.dataObject[0].properties?
                .FirstOrDefault(p => p.name.Equals("info"))?.properties?
                .FirstOrDefault(p => p.name.Equals("type"))?.value;

            return archiveExtractionType != null &&
                   archiveExtractionType.Contains("noark", StringComparison.OrdinalIgnoreCase) &&
                   archiveExtractionType.Contains("5");
        }

        private static bool TypeOfChosenArchiveDirectoryIsSpecializedSystem(addml addml)
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

        private static void LogWarning(string details = null)
        {
            const string warning = "Arkade could not automatically identify the type of the chosen archive";

            if (!string.IsNullOrEmpty(details))
                Log.Warning(warning + ":\n" + details);

            Log.Warning(warning);
        }
    }
}
