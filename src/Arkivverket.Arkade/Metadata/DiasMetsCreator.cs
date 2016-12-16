using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.ExternalModels.DiasMets;
using Arkivverket.Arkade.Util;
using Serilog;

namespace Arkivverket.Arkade.Metadata
{
    public class DiasMetsCreator
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public mets Create(Archive archive, ArchiveMetadata metadata)
        {
            var mets = new mets()
            {
                metsHdr = new metsTypeMetsHdr() {CREATEDATE = DateTime.Now}
            };

            // TODO insert metadata here..

            return mets;
        }

        public void CreateAndSaveFile(Archive archive, ArchiveMetadata metadata)
        {
            mets mets = Create(archive, metadata);

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("mets", "http://arkivverket.no/standarder/DIAS-METS");
            namespaces.Add("xlink", "http://www.w3.org/1999/xlink");
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            FileInfo targetFileName = archive.WorkingDirectory.Root().WithFile(ArkadeConstants.DiasMetsXmlFileName);
            SerializeUtil.SerializeToFile(mets, targetFileName, namespaces);

            Log.Information($"Created {ArkadeConstants.DiasMetsXmlFileName}");
        }

    }
}
