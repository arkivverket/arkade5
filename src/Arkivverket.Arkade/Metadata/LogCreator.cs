using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.ExternalModels.DiasPremis;
using Arkivverket.Arkade.Util;
using Serilog;

namespace Arkivverket.Arkade.Metadata
{
    public class LogCreator
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public premisComplexType Create(Archive archive, ArchiveMetadata metadata)
        {
            return new premisComplexType()
            {
                @object = new objectComplexType[]
                {
                    CreateEntryForTarFile(archive.Uuid)
                }
            };
        }

        private static file CreateEntryForTarFile(Uuid uuid)
        {
            return new file()
            {
                objectIdentifier = new []{
                    new objectIdentifierComplexType()
                    {
                        objectIdentifierType = "NO/RA",
                        objectIdentifierValue = uuid.ToString()
                    }
                },
                preservationLevel = new []{ new preservationLevelComplexType { preservationLevelValue = "full" } },
                objectCharacteristics = new []
                {
                    new objectCharacteristicsComplexType()
                    {
                        compositionLevel = "0",
                        format = new []
                        {
                            new formatComplexType
                            {
                                Items = new [] { new formatDesignationComplexType { formatName = "tar" } }
                            }
                        }
                    }
                }
            };
        }

        public void CreateAndSaveFile(Archive archive, ArchiveMetadata metadata)
        {
            premisComplexType premis = Create(archive, metadata);

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("premis", "http://arkivverket.no/standarder/PREMIS");
            namespaces.Add("xlink", "http://www.w3.org/1999/xlink");
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            FileInfo targetFileName = archive.WorkingDirectory.Root().WithFile(ArkadeConstants.LogXmlFileName);
            SerializeUtil.SerializeToFile(premis, targetFileName, namespaces);

            Log.Information($"Created {ArkadeConstants.LogXmlFileName}");
        }
    }
}
