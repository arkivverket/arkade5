using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.ExternalModels.Mets;
using Arkivverket.Arkade.Core.Util;
using Serilog;

namespace Arkivverket.Arkade.Core.Metadata
{
    public class InfoXmlCreator : MetsCreator
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public void CreateAndSaveFile(ArchiveMetadata metadata, string packageFileName)
        {
            var packageFile = new FileInfo(packageFileName);

            PrepareForPackageDescription(metadata, packageFile);

            mets infoXml = Create(metadata);

            var targetFileObject = new FileInfo(
                Path.Combine(packageFile.DirectoryName, ArkadeConstants.InfoXmlFileName)
            );

            XmlSerializerNamespaces namespaces = SetupNamespaces();

            SerializeUtil.SerializeToFile(infoXml, targetFileObject, namespaces);

            Log.Information($"Created {targetFileObject}");
        }

        private static void PrepareForPackageDescription(ArchiveMetadata metadata, FileInfo packageFile)
        {
            metadata.FileDescriptions = null; // Removes any existing file-descriptions
            
            if (packageFile.Exists)
            {
                FileDescription informationPackageFileDescription = GetFileDescription
                (
                    packageFile,
                    packageFile.Directory
                );

                informationPackageFileDescription.Id = 0;

                metadata.FileDescriptions = new List<FileDescription>
                {
                    informationPackageFileDescription
                };
            }
        }
    }
}
