using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.ExternalModels.Mets;
using Arkivverket.Arkade.Core.Util;
using Serilog;

namespace Arkivverket.Arkade.Core.Metadata
{
    public class DiasMetsCreator : MetsCreator
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public void CreateAndSaveFile(Archive archive, ArchiveMetadata metadata)
        {
            DirectoryInfo rootDirectory = archive.WorkingDirectory.Root().DirectoryInfo();

            if (rootDirectory.Exists)
            {
                string[] filesToSkip = metadata.PackageType == PackageType.SubmissionInformationPackage
                    ? new[] { ArkadeConstants.EadXmlFileName, ArkadeConstants.EacCpfXmlFileName }
                    : null;

                metadata.FileDescriptions = GetFileDescriptions(rootDirectory, rootDirectory, archive.DocumentFiles, filesToSkip);
            }

            if (archive.WorkingDirectory.HasExternalContentDirectory())
            {
                DirectoryInfo externalContentDirectory = archive.WorkingDirectory.Content().DirectoryInfo();

                if (externalContentDirectory.Exists)
                {
                    var fileDescriptions = GetFileDescriptions(externalContentDirectory, externalContentDirectory, archive.DocumentFiles);

                    foreach (FileDescription fileDescription in fileDescriptions)
                        fileDescription.Name = Path.Combine("content", fileDescription.Name);

                    metadata.FileDescriptions.AddRange(fileDescriptions);
                }
            }

            const int fileIdOffset = 1; // Reserving 0 for package file
            AutoIncrementFileIds(metadata.FileDescriptions, fileIdOffset);

            mets mets = Create(metadata);

            FileInfo targetFileName = archive.WorkingDirectory.Root().WithFile(ArkadeConstants.DiasMetsXmlFileName);

            XmlSerializerNamespaces namespaces = SetupNamespaces();

            SerializeUtil.SerializeToFile(mets, targetFileName, namespaces);

            Log.Debug($"Created {ArkadeConstants.DiasMetsXmlFileName}");
        }
    }
}
