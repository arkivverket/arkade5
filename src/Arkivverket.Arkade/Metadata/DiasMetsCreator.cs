using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var mets = new mets();

            CreateMetsHdr(mets, metadata);

            CreateAmdSec(mets, metadata);

            return mets;
        }

        private static void CreateMetsHdr(metsType mets, ArchiveMetadata metadata)
        {
            var metsHdr = new metsTypeMetsHdr();

            CreateAltRecordIDs(metsHdr, metadata);

            CreateHdrAgents(metsHdr, metadata);

            if (metsHdr.altRecordID != null || metsHdr.agent != null)
                mets.metsHdr = metsHdr;
        }

        private static void CreateAltRecordIDs(metsTypeMetsHdr metsHdr, ArchiveMetadata metadata)
        {
            var altRecordIDs = new List<metsTypeMetsHdrAltRecordID>();

            if (!string.IsNullOrEmpty(metadata.ArchiveDescription))
            {
                altRecordIDs.Add(new metsTypeMetsHdrAltRecordID
                {
                    TYPE = "DELIVERYSPECIFICATION",
                    Value = metadata.ArchiveDescription
                });
            }

            if (!string.IsNullOrEmpty(metadata.AgreementNumber))
            {
                altRecordIDs.Add(new metsTypeMetsHdrAltRecordID
                {
                    TYPE = "SUBMISSIONAGREEMENT",
                    Value = metadata.AgreementNumber
                });
            }

            if (altRecordIDs.Any())
                metsHdr.altRecordID = altRecordIDs.ToArray();
        }

        private static void CreateHdrAgents(metsTypeMetsHdr metsHdr, ArchiveMetadata metadata)
        {
            var metsTypeMetsHdrAgents = new List<metsTypeMetsHdrAgent>();

            // CREATORS:

            if (metadata.ArchiveCreators != null)
            {
                foreach (MetadataEntityInformationUnit metadataArchiveCreator in metadata.ArchiveCreators)
                {
                    if (HasEntity(metadataArchiveCreator))
                    {
                        metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                        {
                            TYPE = metsTypeMetsHdrAgentTYPE.ORGANIZATION,
                            ROLE = metsTypeMetsHdrAgentROLE.ARCHIVIST,
                            name = metadataArchiveCreator.Entity
                        });
                    }

                    if (!string.IsNullOrEmpty(metadataArchiveCreator.ContactPerson))
                    {
                        metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                        {
                            TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                            ROLE = metsTypeMetsHdrAgentROLE.ARCHIVIST,
                            name = metadataArchiveCreator.ContactPerson
                        });
                    }

                    if (!string.IsNullOrEmpty(metadataArchiveCreator.Telephone))
                    {
                        metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                        {
                            TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                            ROLE = metsTypeMetsHdrAgentROLE.ARCHIVIST,
                            note = new[] { metadataArchiveCreator.Telephone }
                        });
                    }

                    if (!string.IsNullOrEmpty(metadataArchiveCreator.Email))
                    {
                        metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                        {
                            TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                            ROLE = metsTypeMetsHdrAgentROLE.ARCHIVIST,
                            note = new[] { metadataArchiveCreator.Email }
                        });
                    }
                }
            }

            // TRANSFERRER:

            if (metadata.Transferer != null)
            {
                if (HasEntity(metadata.Transferer))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.ORGANIZATION,
                        ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                        OTHERROLE = "SUBMITTER",
                        name = metadata.Transferer.Entity
                    });
                }

                if (!string.IsNullOrEmpty(metadata.Transferer.ContactPerson))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                        ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                        OTHERROLE = "SUBMITTER",
                        name = metadata.Transferer.ContactPerson
                    });
                }

                if (!string.IsNullOrEmpty(metadata.Transferer.Telephone))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                        ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                        OTHERROLE = "SUBMITTER",
                        note = new[] { metadata.Transferer.Telephone }
                    });
                }

                if (!string.IsNullOrEmpty(metadata.Transferer.Email))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                        ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                        OTHERROLE = "SUBMITTER",
                        note = new[] { metadata.Transferer.Email }
                    });
                }
            }

            // PRODUCER:

            if (metadata.Producer != null)
            {
                if (HasEntity(metadata.Producer))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.ORGANIZATION,
                        ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                        OTHERROLE = "PRODUCER",
                        name = metadata.Producer.Entity
                    });
                }

                if (!string.IsNullOrEmpty(metadata.Producer.ContactPerson))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                        ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                        OTHERROLE = "PRODUCER",
                        name = metadata.Producer.ContactPerson,
                    });
                }

                if (!string.IsNullOrEmpty(metadata.Producer.Telephone))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                        ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                        OTHERROLE = "PRODUCER",
                        note = new[] { metadata.Producer.Telephone }
                    });
                }

                if (!string.IsNullOrEmpty(metadata.Producer.Email))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                        ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                        OTHERROLE = "PRODUCER",
                        note = new[] { metadata.Producer.Email }
                    });
                }
            }

            // OWNERS:

            if (metadata.Owners != null)
            {
                foreach (MetadataEntityInformationUnit metadataOwner in metadata.Owners)
                {
                    if (HasEntity(metadataOwner))
                    {
                        metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                        {
                            TYPE = metsTypeMetsHdrAgentTYPE.ORGANIZATION,
                            ROLE = metsTypeMetsHdrAgentROLE.IPOWNER,
                            name = metadataOwner.Entity
                        });
                    }

                    if (!string.IsNullOrEmpty(metadataOwner.ContactPerson))
                    {
                        metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                        {
                            TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                            ROLE = metsTypeMetsHdrAgentROLE.IPOWNER,
                            name = metadataOwner.ContactPerson
                        });
                    }

                    if (!string.IsNullOrEmpty(metadataOwner.Telephone))
                    {
                        metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                        {
                            TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                            ROLE = metsTypeMetsHdrAgentROLE.IPOWNER,
                            note = new[] { metadataOwner.Telephone }
                        });
                    }

                    if (!string.IsNullOrEmpty(metadataOwner.Email))
                    {
                        metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                        {
                            TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                            ROLE = metsTypeMetsHdrAgentROLE.IPOWNER,
                            note = new[] { metadataOwner.Email }
                        });
                    }
                }
            }

            // RECIPIENT:

            if (!string.IsNullOrEmpty(metadata.Recipient))
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
                MetadataSystemInformationUnit system = metadata.System;

                if (!string.IsNullOrEmpty(system.Name))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                        OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                        ROLE = metsTypeMetsHdrAgentROLE.ARCHIVIST,
                        name = system.Name
                    });
                }

                if (!string.IsNullOrEmpty(system.Version))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                        OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                        ROLE = metsTypeMetsHdrAgentROLE.ARCHIVIST,
                        note = new[] { system.Version }
                    });
                }

                if (!string.IsNullOrEmpty(system.Type) && IsValidSystemType(system.Type))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                        OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                        ROLE = metsTypeMetsHdrAgentROLE.ARCHIVIST,
                        note = new[] { system.Type }
                    });
                }

                if (!string.IsNullOrEmpty(system.TypeVersion) && IsSystemTypeNoark5(system.Type))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                        OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                        ROLE = metsTypeMetsHdrAgentROLE.ARCHIVIST,
                        note = new[] { system.TypeVersion }
                    });
                }
            }

            // ARCHIVE SYSTEM:

            if (metadata.ArchiveSystem != null)
            {
                MetadataSystemInformationUnit archiveSystem = metadata.ArchiveSystem;

                if (!string.IsNullOrEmpty(archiveSystem.Name))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                        OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                        ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                        OTHERROLE = "PRODUCER",
                        name = archiveSystem.Name
                    });
                }

                if (!string.IsNullOrEmpty(archiveSystem.Version))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                        OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                        ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                        OTHERROLE = "PRODUCER",
                        note = new[] { archiveSystem.Version }
                    });
                }

                if (!string.IsNullOrEmpty(archiveSystem.Type) && IsValidSystemType(archiveSystem.Type))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                        OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                        ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                        OTHERROLE = "PRODUCER",
                        note = new[] { archiveSystem.Type }
                    });
                }

                if (!string.IsNullOrEmpty(archiveSystem.TypeVersion) && IsSystemTypeNoark5(archiveSystem.Type))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                        OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                        ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                        OTHERROLE = "PRODUCER",
                        note = new[] { archiveSystem.TypeVersion }
                    });
                }
            }

            if (metsTypeMetsHdrAgents.Any())
                metsHdr.agent = metsTypeMetsHdrAgents.ToArray();
        }

        private static void CreateAmdSec(metsType mets, ArchiveMetadata metadata)
        {
            // TODO: Implement when type "mdSecTypeMdRefOTHERMDTYPE.COMMENT" is supported in built in mets schema
        }

        private static bool HasEntity(MetadataEntityInformationUnit entityInformationUnit)
        {
            return !string.IsNullOrEmpty(entityInformationUnit.Entity);
        }

        private static bool HasContactData(MetadataEntityInformationUnit entityInformationUnit)
        {
            return !string.IsNullOrEmpty(entityInformationUnit.ContactPerson) ||
                   !string.IsNullOrEmpty(entityInformationUnit.Telephone) ||
                   !string.IsNullOrEmpty(entityInformationUnit.Email);
        }

        private static bool IsValidSystemType(string systemType)
        {
            // TODO: Use Enum ExternalModels.Mets.type (not ExternalModels.Info.type) when/if supported in built in mets schema

            return Enum.IsDefined(typeof(ExternalModels.Info.type), systemType);
        }

        private static bool IsSystemTypeNoark5(string systemType)
        {
            // TODO: Use Enum ExternalModels.Mets.type (not ExternalModels.Info.type) when/if supported in built in mets schema

            ExternalModels.Info.type enumSystemType;

            bool isParsableSystemType = Enum.TryParse(systemType, out enumSystemType);

            return isParsableSystemType && enumSystemType == ExternalModels.Info.type.Noark5;
        }
    }
}
