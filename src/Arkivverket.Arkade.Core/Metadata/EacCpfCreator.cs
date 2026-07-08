using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.ExternalModels.Cpf;
using Arkivverket.Arkade.Core.Util;
using Serilog;

namespace Arkivverket.Arkade.Core.Metadata
{
    public class EacCpfCreator : IMetadataCreator
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public eaccpf Create(Uuid outputPackageUuid)
        {
            return new eaccpf()
            {
               control = new control() { recordId =  new recordId() { Value = outputPackageUuid.ToString() } }
            };
        }

        public void CreateAndSaveFile(OutputDiasPackage outputDiasPackage)
        {
            eaccpf eaccpf = Create(outputDiasPackage.Id);

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "urn:isbn:1-931666-33-4"); // use blank in namespace prefix to create files without prefixed elements
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            FileInfo targetFileName = outputDiasPackage.WorkingDirectory.DescriptiveMetadata().WithFile(ArkadeConstants.EacCpfXmlFileName);
            SerializeUtil.SerializeToFile(eaccpf, targetFileName, namespaces);

            Log.Debug($"Created {ArkadeConstants.EacCpfXmlFileName}");
        }
    }
}
