using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.ExternalModels.Mets;
using Arkivverket.Arkade.Util;
using Serilog;

namespace Arkivverket.Arkade.Metadata
{
    public class InfoXmlCreator : MetsCreator
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public void CreateAndSaveFile(Archive archive, ArchiveMetadata metadata, string packageFileName)
        {
            var packageFile = new FileInfo(packageFileName);

            PrepareForPackageDescription(metadata, packageFile);

            mets infoXml = Create(metadata);

            FileInfo targetFileObject = PrepareTargetFileObject(archive, packageFile);

            XmlSerializerNamespaces namespaces = SetupNamespaces();

            SerializeUtil.SerializeToFile(infoXml, targetFileObject, namespaces);

            Log.Information($"Created {targetFileObject}");
        }

        private static void PrepareForPackageDescription(ArchiveMetadata metadata, FileInfo packageFile)
        {
            metadata.FileDescriptions = null; // Removes any existing file-descriptions
            
            if (packageFile.Exists)
            {
                var informationPackageFileIdForMets = 0;

                metadata.FileDescriptions = new List<FileDescription>
                {
                    GetFileDescription
                    (
                        packageFile,
                        ref informationPackageFileIdForMets,
                        packageFile.Directory
                    )
                };
            }
        }

        private static FileInfo PrepareTargetFileObject(Archive archive, FileInfo packageFile)
        {
            string infoXmlFileName = archive.Uuid + ".xml";

            string infoXmlFullFileName = Path.Combine(packageFile.DirectoryName, infoXmlFileName);

            return new FileInfo(infoXmlFullFileName);
        }
    }
}
