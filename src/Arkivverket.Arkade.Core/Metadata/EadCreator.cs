using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.ExternalModels.Ead;
using Arkivverket.Arkade.Core.Util;
using Serilog;

namespace Arkivverket.Arkade.Core.Metadata
{
    public class EadCreator
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public ead Create(Archive archive, ArchiveMetadata metadata)
        {
            return new ead()
            {
                control = new control() {recordid = new recordid() { Text = new[] { archive.Uuid.ToString() } } }
            };
        }

        public void CreateAndSaveFile(Archive archive, ArchiveMetadata metadata)
        {
            ead ead = Create(archive, metadata);

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "http://ead3.archivists.org/schema/"); // use blank in namespace prefix to create files without prefixed elements
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            FileInfo targetFileName = archive.WorkingDirectory.DescriptiveMetadata().WithFile(ArkadeConstants.EadXmlFileName);
            SerializeUtil.SerializeToFile(ead, targetFileName, namespaces);

            Log.Information($"Created {ArkadeConstants.EadXmlFileName}");
        }
    }
}
