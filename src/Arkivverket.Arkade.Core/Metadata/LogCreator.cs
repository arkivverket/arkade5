using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.ExternalModels.DiasPremis;
using Arkivverket.Arkade.Core.Util;
using Serilog;

namespace Arkivverket.Arkade.Core.Metadata
{
    public class LogCreator
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public premisComplexType Create(Uuid outputPackageUuid)
        {
            return new premisComplexType()
            {
                @object = new objectComplexType[]
                {
                    CreateEntryForTarFile(outputPackageUuid)
                }
            };
        }

        private static @file CreateEntryForTarFile(Uuid uuid)
        {
            return new file()
            {
                objectIdentifier = new []{
                    new objectIdentifierComplexType()
                    {
                        objectIdentifierType = "NO/RA",
                        objectIdentifierValue = uuid.ToString() // NB! UUID-writeout (package creation)
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

        public void CreateAndSaveFile(OutputDiasPackage diasPackage)
        {
            premisComplexType premis = Create(diasPackage.Uuid);

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("premis", "http://arkivverket.no/standarder/PREMIS");
            namespaces.Add("xlink", "http://www.w3.org/1999/xlink");
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            FileInfo targetFileName = diasPackage.Archive.WorkingDirectory.Root().WithFile(ArkadeConstants.LogXmlFileName);
            SerializeUtil.SerializeToFile(premis, targetFileName, namespaces);

            Log.Debug($"Created {ArkadeConstants.LogXmlFileName}");
        }
    }
}
