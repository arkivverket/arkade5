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

        public void CreateAndSaveFile(ArchiveMetadata metadata, string packageFileName, string diasMetsFileName,
            string infoXmlFileName)
        {
            var packageFile = new FileInfo(packageFileName);
            var diasMetsFile = new FileInfo(diasMetsFileName);

            PrepareForPackageDescription(metadata, packageFile, diasMetsFile);

            mets infoXml = Create(metadata);

            var targetFileObject = new FileInfo(
                Path.Combine(packageFile.DirectoryName, infoXmlFileName)
            );

            XmlSerializerNamespaces namespaces = SetupNamespaces();

            SerializeUtil.SerializeToFile(infoXml, targetFileObject, namespaces);

            Log.Debug($"Created {targetFileObject}");
        }

        private static void PrepareForPackageDescription(ArchiveMetadata metadata, FileInfo packageFile,
            FileInfo diasMetsFile)
        {
            metadata.FileDescriptions = null; // Removes any existing file-descriptions

            if (packageFile.Exists && diasMetsFile.Exists)
            {
                FileDescription informationPackageFileDescription = GetFileDescription
                (
                    packageFile,
                    packageFile.Directory
                );

                FileDescription metsFileDescription = GetFileDescription
                (
                    diasMetsFile,
                    diasMetsFile.Directory
                );

                string metsFileDirectoryName = Path.GetFileNameWithoutExtension(packageFile.Name);

                informationPackageFileDescription.Id = 0;
                metsFileDescription.Id = 1;
                metsFileDescription.Name = Path.Combine(metsFileDirectoryName, diasMetsFile.Name);

                metadata.FileDescriptions = new List<FileDescription>
                {
                    informationPackageFileDescription,
                    metsFileDescription
                };
            }
        }
    }
}
