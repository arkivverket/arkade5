using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.ExternalModels.Mets;
using Arkivverket.Arkade.Util;
using Serilog;

namespace Arkivverket.Arkade.Metadata
{
    public class DiasMetsCreator : MetsCreator
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public void CreateAndSaveFile(Archive archive, ArchiveMetadata metadata)
        {
            DirectoryInfo documentsDirectory = archive.WorkingDirectory.Content()
                .WithSubDirectory(ArkadeConstants.DirectoryNameDocuments).DirectoryInfo();

            if(documentsDirectory.Exists)
                metadata.FileDescriptions = GetFileDescriptions(documentsDirectory);

            mets mets = Create(metadata);

            FileInfo targetFileName = archive.WorkingDirectory.Root().WithFile(ArkadeConstants.DiasMetsXmlFileName);

            XmlSerializerNamespaces namespaces = SetupNamespaces();

            SerializeUtil.SerializeToFile(mets, targetFileName, namespaces);

            Log.Information($"Created {ArkadeConstants.DiasMetsXmlFileName}");
        }
    }
}
