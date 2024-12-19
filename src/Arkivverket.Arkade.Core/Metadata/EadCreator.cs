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

        public ead Create(Uuid outputPackageUuid)
        {
            return new ead()
            {
                control = new control() {recordid = new recordid() { Text = new[] { outputPackageUuid.ToString() } } } // NB! UUID-writeout (package creation)
            };
        }

        public void CreateAndSaveFile(OutputDiasPackage diasPackage)
        {
            ead ead = Create(diasPackage.Id);

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "http://ead3.archivists.org/schema/"); // use blank in namespace prefix to create files without prefixed elements
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            FileInfo targetFileName = diasPackage.Archive.WorkingDirectory.DescriptiveMetadata().WithFile(ArkadeConstants.EadXmlFileName);
            SerializeUtil.SerializeToFile(ead, targetFileName, namespaces);

            Log.Debug($"Created {ArkadeConstants.EadXmlFileName}");
        }
    }
}
