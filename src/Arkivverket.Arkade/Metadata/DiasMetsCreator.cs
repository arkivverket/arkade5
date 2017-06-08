using System.Collections.Generic;
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

        public void CreateAndSaveFile(Archive archive, ArchiveMetadata metadata)
        {
            mets mets = Create(metadata);

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("mets", "http://arkivverket.no/standarder/DIAS-METS");
            namespaces.Add("xlink", "http://www.w3.org/1999/xlink");
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            FileInfo targetFileName = archive.WorkingDirectory.Root().WithFile(ArkadeConstants.DiasMetsXmlFileName);
            SerializeUtil.SerializeToFile(mets, targetFileName, namespaces);

            Log.Information($"Created {ArkadeConstants.DiasMetsXmlFileName}");
        }

        public mets Create(ArchiveMetadata metadata)
        {
            var mets = new mets
            {
                metsHdr = new metsTypeMetsHdr
                {
                    agent = CreateHdrAgents(metadata),
                    altRecordID = new[]
                    {
                        new metsTypeMetsHdrAltRecordID
                        {
                            TYPE = "DELIVERYSPECIFICATION",
                            Value = metadata.ArchiveDescription
                        },
                        new metsTypeMetsHdrAltRecordID
                        {
                            TYPE = "SUBMISSIONAGREEMENT",
                            Value = metadata.AgreementNumber
                        }
                    },
                },
                //amdSec = new[] { new amdSecType { techMD = CreateMdSecTypesForTechMd(metadata) } },
            };

            return mets;
        }

        private static mdSecType[] CreateMdSecTypesForTechMd(ArchiveMetadata metadata)
        {
            var mdSecTypes = new List<mdSecType>();

            foreach (string comment in metadata.Comments)
            {
                mdSecTypes.Add(new mdSecType
                {
                    mdWrap = new mdSecTypeMdWrap
                    {
                        MDTYPE = mdSecTypeMdRefMDTYPE.OTHER,
                        //OTHERMDTYPE = mdSecTypeMdRefOTHERMDTYPE.METS, // "COMMENT"
                        Item = comment
                    }
                });
            }

            return mdSecTypes.ToArray();
        }

        private static metsTypeMetsHdrAgent[] CreateHdrAgents(ArchiveMetadata metadata)
        {
            var metsTypeMetsHdrAgents = new List<metsTypeMetsHdrAgent>();

            // CREATORS:

            foreach (MetadataEntityInformationUnit metadataArchiveCreator in metadata.ArchiveCreator)
            {
                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.ORGANIZATION,
                    ROLE = metsTypeMetsHdrAgentROLE.ARCHIVIST,
                    name = metadataArchiveCreator.Entity
                });

                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                    ROLE = metsTypeMetsHdrAgentROLE.CREATOR,
                    name = metadataArchiveCreator.ContactPerson,
                    note = new[] { metadataArchiveCreator.Telephone, metadataArchiveCreator.Email }
                });
            }

            // TRANSFERRER:

            if (metadata.Transferer != null)
            {
                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.ORGANIZATION,
                    ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                    OTHERROLE = "SUBMITTER",
                    name = metadata.Transferer.Entity
                });

                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                    ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                    OTHERROLE = "SUBMITTER",
                    name = metadata.Transferer.ContactPerson,
                    note = new[] { metadata.Transferer.Telephone, metadata.Transferer.Email }
                });
            }

            // PRODUCER:

            if (metadata.Producer != null)
            {
                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.ORGANIZATION,
                    ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                    OTHERROLE = "PRODUCER",
                    name = metadata.Producer.Entity
                });

                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                    ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                    OTHERROLE = "PRODUCER",
                    name = metadata.Producer.ContactPerson,
                    note = new[] { metadata.Producer.Telephone, metadata.Producer.Email }
                });
            }

            // OWNERS:

            foreach (MetadataEntityInformationUnit metadataOwner in metadata.Owner)
            {
                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.ORGANIZATION,
                    ROLE = metsTypeMetsHdrAgentROLE.IPOWNER,
                    name = metadataOwner.Entity
                });

                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                    ROLE = metsTypeMetsHdrAgentROLE.IPOWNER,
                    name = metadataOwner.ContactPerson,
                    note = new[] { metadataOwner.Telephone, metadataOwner.Email }
                });
            }

            // RECIPIENT:

            if (metadata.Recipient != null)
            {
                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.ORGANIZATION,
                    ROLE = metsTypeMetsHdrAgentROLE.PRESERVATION,
                    name = metadata.Recipient
                });
            }

            // SYSTEM:

            if (metadata.System != null)
            {
                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                    OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                    ROLE = metsTypeMetsHdrAgentROLE.ARCHIVIST,
                    name = metadata.System.Name
                });

                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                    OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                    ROLE = metsTypeMetsHdrAgentROLE.ARCHIVIST,
                    name = metadata.System.Version
                });

                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                    OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                    ROLE = metsTypeMetsHdrAgentROLE.ARCHIVIST,
                    name = metadata.System.Type
                });

                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                    OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                    ROLE = metsTypeMetsHdrAgentROLE.ARCHIVIST,
                    name = metadata.System.TypeVersion
                });

                // ARCHIVE SYSTEM:

                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                    OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                    ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                    OTHERROLE = "PRODUCER",
                    name = metadata.ArchiveSystem.Name
                });

                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                    OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                    ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                    OTHERROLE = "PRODUCER",
                    name = metadata.ArchiveSystem.Version
                });

                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                    OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                    ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                    OTHERROLE = "PRODUCER",
                    name = metadata.ArchiveSystem.Type
                });

                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                    OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                    ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                    OTHERROLE = "PRODUCER",
                    name = metadata.ArchiveSystem.TypeVersion
                });
            }

            // ARCHIVE SYSTEM:

            if (metadata.System != null)
            {
                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                    OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                    ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                    OTHERROLE = "PRODUCER",
                    name = metadata.ArchiveSystem.Name
                });

                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                    OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                    ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                    OTHERROLE = "PRODUCER",
                    name = metadata.ArchiveSystem.Version
                });

                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                    OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                    ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                    OTHERROLE = "PRODUCER",
                    name = metadata.ArchiveSystem.Type
                });

                metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                {
                    TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                    OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                    ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                    OTHERROLE = "PRODUCER",
                    name = metadata.ArchiveSystem.TypeVersion
                });
            }

            return metsTypeMetsHdrAgents.ToArray();
        }
    }
}
