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
            mets infoXml = Create(metadata);

            FileInfo targetFileName = archive.WorkingDirectory.Root().WithFile(ArkadeConstants.InfoXmlFileName);

            XmlSerializerNamespaces namespaces = SetupNamespaces();

            SerializeUtil.SerializeToFile(infoXml, targetFileName, namespaces);

            Log.Information($"Created {ArkadeConstants.InfoXmlFileName}");
        }
    }
}
