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

        public void CreateAndSaveFile(Archive archive, ArchiveMetadata metadata)
        {
            PrepareForPackageDescription(archive, metadata);

            mets infoXml = Create(metadata);

            FileInfo targetFileName = archive.GetInfoXmlFileName();

            XmlSerializerNamespaces namespaces = SetupNamespaces();

            SerializeUtil.SerializeToFile(infoXml, targetFileName, namespaces);

            Log.Information($"Created {targetFileName}");
        }

        private static void PrepareForPackageDescription(Archive archive, ArchiveMetadata metadata)
        {
            metadata.FileDescriptions = null; // Removes any existing file-descriptions

            FileInfo informationPackageFile = archive.GetInformationPackageFileName();

            if (informationPackageFile.Exists)
            {
                var informationPackageFileIdForMets = 0;

                metadata.FileDescriptions = new List<FileDescription>
                {
                    GetFileDescription
                    (
                        informationPackageFile,
                        ref informationPackageFileIdForMets,
                        informationPackageFile.Directory
                    )
                };
            }
        }
    }
}
