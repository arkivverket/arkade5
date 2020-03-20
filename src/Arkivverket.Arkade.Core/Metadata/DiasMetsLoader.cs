using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.ExternalModels.Mets;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Metadata
{
    public static class DiasMetsLoader
    {
        public static ArchiveMetadata Load(string diasMetsFile)
        {
            var mets = SerializeUtil.DeserializeFromFile<mets>(diasMetsFile);

            var archiveMetadata = new ArchiveMetadata();

            LoadMetsElementAttributes(archiveMetadata, mets);

            if (mets.metsHdr != null)
                LoadMetsHdr(archiveMetadata, mets.metsHdr);

            MetadataLoader.HandleLabelPlaceholder(archiveMetadata);

            return archiveMetadata;
        }

        private static void LoadMetsElementAttributes(ArchiveMetadata archiveMetadata, mets mets)
        {
            archiveMetadata.Label = mets.LABEL;
        }

        private static void LoadMetsHdr(ArchiveMetadata archiveMetadata, metsTypeMetsHdr metsHdr)
        {
            archiveMetadata.ExtractionDate = metsHdr.CREATEDATE;

            if (metsHdr.altRecordID != null)
                LoadMetsHdrAltRecordIDs(archiveMetadata, metsHdr.altRecordID);

            if (metsHdr.agent != null)
                LoadMetsHdrAgents(archiveMetadata, metsHdr.agent);
        }

        private static void LoadMetsHdrAltRecordIDs(ArchiveMetadata archiveMetadata,
            metsTypeMetsHdrAltRecordID[] metsHdrAltRecordIds)
        {
            LoadArchiveDescription(archiveMetadata, metsHdrAltRecordIds);
            LoadAgreementNumber(archiveMetadata, metsHdrAltRecordIds);
            LoadStartDate(archiveMetadata, metsHdrAltRecordIds);
            LoadEndDate(archiveMetadata, metsHdrAltRecordIds);
        }

        private static void LoadMetsHdrAgents(ArchiveMetadata archiveMetadata, metsTypeMetsHdrAgent[] metsHdrAgents)
        {
            LoadArchiveCreators(archiveMetadata, metsHdrAgents);
            LoadTransferer(archiveMetadata, metsHdrAgents);
            LoadProducer(archiveMetadata, metsHdrAgents);
            LoadOwners(archiveMetadata, metsHdrAgents);
            LoadCreator(archiveMetadata, metsHdrAgents);
            LoadRecipient(archiveMetadata, metsHdrAgents);
            LoadSystem(archiveMetadata, metsHdrAgents);
            LoadCreatorSoftwareSystem(archiveMetadata, metsHdrAgents);
            LoadArchiveSystem(archiveMetadata, metsHdrAgents);
        }

        private static void LoadArchiveDescription(ArchiveMetadata archiveMetadata,
            IEnumerable<metsTypeMetsHdrAltRecordID> metsHdrAltRecordIds)
        {
            archiveMetadata.ArchiveDescription = metsHdrAltRecordIds.FirstOrDefault(a =>
                a.TYPE == metsTypeMetsHdrAltRecordIDTYPE.DELIVERYSPECIFICATION)?.Value;
        }

        private static void LoadAgreementNumber(ArchiveMetadata archiveMetadata,
            IEnumerable<metsTypeMetsHdrAltRecordID> metsHdrAltRecordIds)
        {
            archiveMetadata.AgreementNumber = metsHdrAltRecordIds.FirstOrDefault(a =>
                a.TYPE == metsTypeMetsHdrAltRecordIDTYPE.SUBMISSIONAGREEMENT)?.Value;
        }

        private static void LoadStartDate(ArchiveMetadata archiveMetadata,
            IEnumerable<metsTypeMetsHdrAltRecordID> metsHdrAltRecordIds)
        {
            string dateValue = metsHdrAltRecordIds.FirstOrDefault(a =>
                a.TYPE == metsTypeMetsHdrAltRecordIDTYPE.STARTDATE)?.Value;

            archiveMetadata.StartDate = LoadDateOrNull(dateValue);
        }

        private static void LoadEndDate(ArchiveMetadata archiveMetadata,
            IEnumerable<metsTypeMetsHdrAltRecordID> metsHdrAltRecordIds)
        {
            string dateValue = metsHdrAltRecordIds.FirstOrDefault(a =>
                a.TYPE == metsTypeMetsHdrAltRecordIDTYPE.ENDDATE)?.Value;

            archiveMetadata.EndDate = LoadDateOrNull(dateValue);
        }


        private static void LoadArchiveCreators(ArchiveMetadata archiveMetadata, metsTypeMetsHdrAgent[] metsHdrAgents)
        {
            metsTypeMetsHdrAgent[] metsArchiveCreatorAgents = metsHdrAgents.Where(a =>
                a.ROLE == metsTypeMetsHdrAgentROLE.ARCHIVIST &&
                (a.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION || a.TYPE == metsTypeMetsHdrAgentTYPE.INDIVIDUAL)
            ).ToArray();

            if (!metsArchiveCreatorAgents.Any())
                return;

            var archiveMetadataArchiveCreators = new List<MetadataEntityInformationUnit>();

            LoadEntityInformationUnits(archiveMetadataArchiveCreators, metsArchiveCreatorAgents);

            if (archiveMetadataArchiveCreators.Any())
                archiveMetadata.ArchiveCreators = archiveMetadataArchiveCreators;
        }

        private static void LoadTransferer(ArchiveMetadata archiveMetadata, metsTypeMetsHdrAgent[] metsHdrAgents)
        {
            metsTypeMetsHdrAgent[] metsTransfererAgents = metsHdrAgents.Where(a =>
                a.ROLE == metsTypeMetsHdrAgentROLE.OTHER &&
                a.OTHERROLE.Equals(metsTypeMetsHdrAgentOTHERROLE.SUBMITTER) &&
                (a.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION || a.TYPE == metsTypeMetsHdrAgentTYPE.INDIVIDUAL)
            ).ToArray();

            if (!metsTransfererAgents.Any())
                return;

            var archiveMetadataTransfererContainer = new List<MetadataEntityInformationUnit>();

            LoadEntityInformationUnits(archiveMetadataTransfererContainer, metsTransfererAgents);

            var archiveMetadataTransferer = archiveMetadataTransfererContainer.FirstOrDefault();

            if (archiveMetadataTransferer != null && HasData(archiveMetadataTransferer))
                archiveMetadata.Transferer = archiveMetadataTransferer;
        }

        private static void LoadProducer(ArchiveMetadata archiveMetadata, metsTypeMetsHdrAgent[] metsHdrAgents)
        {
            metsTypeMetsHdrAgent[] metsProducerAgents = metsHdrAgents.Where(a =>
                a.ROLE == metsTypeMetsHdrAgentROLE.OTHER &&
                a.OTHERROLE.Equals(metsTypeMetsHdrAgentOTHERROLE.PRODUCER) &&
                (a.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION || a.TYPE == metsTypeMetsHdrAgentTYPE.INDIVIDUAL)
            ).ToArray();

            if (!metsProducerAgents.Any())
                return;

            var archiveMetadataProducerContainer = new List<MetadataEntityInformationUnit>();

            LoadEntityInformationUnits(archiveMetadataProducerContainer, metsProducerAgents);

            var archiveMetadataProducer = archiveMetadataProducerContainer.FirstOrDefault();

            if (archiveMetadataProducer != null && HasData(archiveMetadataProducer))
                archiveMetadata.Producer = archiveMetadataProducer;
        }

        private static void LoadOwners(ArchiveMetadata archiveMetadata, metsTypeMetsHdrAgent[] metsHdrAgents)
        {
            metsTypeMetsHdrAgent[] metsOwnerAgents = metsHdrAgents.Where(a =>
                a.ROLE == metsTypeMetsHdrAgentROLE.IPOWNER &&
                (a.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION || a.TYPE == metsTypeMetsHdrAgentTYPE.INDIVIDUAL)
            ).ToArray();

            if (!metsOwnerAgents.Any())
                return;

            var archiveMetadataOwners = new List<MetadataEntityInformationUnit>();

            LoadEntityInformationUnits(archiveMetadataOwners, metsOwnerAgents);

            if (archiveMetadataOwners.Any())
                archiveMetadata.Owners = archiveMetadataOwners;
        }

        private static void LoadCreator(ArchiveMetadata archiveMetadata, metsTypeMetsHdrAgent[] metsHdrAgents)
        {
            metsTypeMetsHdrAgent[] metsCreators = metsHdrAgents.Where(a =>
                a.ROLE == metsTypeMetsHdrAgentROLE.CREATOR &&
                (a.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION || a.TYPE == metsTypeMetsHdrAgentTYPE.INDIVIDUAL)
            ).ToArray();

            if (!metsCreators.Any())
                return;

            var archiveMetadataCreatorContainer = new List<MetadataEntityInformationUnit>();

            LoadEntityInformationUnits(archiveMetadataCreatorContainer, metsCreators);

            var archiveMetadataCreator = archiveMetadataCreatorContainer.FirstOrDefault();

            if (archiveMetadataCreator != null && HasData(archiveMetadataCreator))
                archiveMetadata.Creator = archiveMetadataCreator;
        }

        private static void LoadEntityInformationUnits(List<MetadataEntityInformationUnit> entityInfoUnits,
            metsTypeMetsHdrAgent[] metsEntityAgents)
        {
            MetadataEntityInformationUnit entityInfoUnit = null;

            foreach (metsTypeMetsHdrAgent metsEntityAgent in metsEntityAgents)
            {
                if (metsEntityAgent.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION)
                {
                    entityInfoUnit = new MetadataEntityInformationUnit { Entity = metsEntityAgent.name };
                    entityInfoUnits.Add(entityInfoUnit);
                }

                // Attaches contact info to last seen organization:
                if (metsEntityAgent.TYPE == metsTypeMetsHdrAgentTYPE.INDIVIDUAL && entityInfoUnit != null)
                {
                    if (!string.IsNullOrEmpty(metsEntityAgent.name))
                        entityInfoUnit.ContactPerson = metsEntityAgent.name;

                    string address = HdrAgentNotesLoader.GetAddress(metsEntityAgent.note);

                    if (!string.IsNullOrEmpty(address))
                        entityInfoUnit.Address = address;

                    string phoneNumber = HdrAgentNotesLoader.GetTelephone(metsEntityAgent.note);

                    if (!string.IsNullOrEmpty(phoneNumber))
                        entityInfoUnit.Telephone = phoneNumber;

                    string emailAddress = HdrAgentNotesLoader.GetEmail(metsEntityAgent.note);

                    if (!string.IsNullOrEmpty(emailAddress))
                        entityInfoUnit.Email = emailAddress;
                }
            }
        }

        private static void LoadRecipient(ArchiveMetadata archiveMetadata, metsTypeMetsHdrAgent[] metsHdrAgents)
        {
            metsTypeMetsHdrAgent metsRecipientAgent = metsHdrAgents.FirstOrDefault(a =>
                a.TYPE == metsTypeMetsHdrAgentTYPE.ORGANIZATION &&
                a.ROLE == metsTypeMetsHdrAgentROLE.PRESERVATION
            );

            archiveMetadata.Recipient = metsRecipientAgent?.name;
        }

        private static void LoadSystem(ArchiveMetadata archiveMetadata, metsTypeMetsHdrAgent[] metsHdrAgents)
        {
            metsTypeMetsHdrAgent metsSystemAgent = metsHdrAgents.FirstOrDefault(a =>
                a.TYPE == metsTypeMetsHdrAgentTYPE.OTHER &&
                a.OTHERTYPE == metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE &&
                a.ROLE == metsTypeMetsHdrAgentROLE.ARCHIVIST
            );

            if (metsSystemAgent == null)
                return;

            var system = new MetadataSystemInformationUnit();

            LoadSystemProperties(system, metsSystemAgent);

            if (HasData(system))
                archiveMetadata.System = system;
        }


        private static void LoadCreatorSoftwareSystem(ArchiveMetadata archiveMetadata, metsTypeMetsHdrAgent[] metsHdrAgents)
        {
            metsTypeMetsHdrAgent metsSystemAgent = metsHdrAgents.FirstOrDefault(a =>
                a.TYPE == metsTypeMetsHdrAgentTYPE.OTHER &&
                a.OTHERTYPE == metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE &&
                a.ROLE == metsTypeMetsHdrAgentROLE.CREATOR 
       
            );

            if (metsSystemAgent == null)
                return;

            var creatorSoftwareSystem = new MetadataSystemInformationUnit();

            LoadSystemProperties(creatorSoftwareSystem, metsSystemAgent);

            if (HasData(creatorSoftwareSystem))
                archiveMetadata.CreatorSoftwareSystem = creatorSoftwareSystem;
        }

        private static void LoadArchiveSystem(ArchiveMetadata archiveMetadata, metsTypeMetsHdrAgent[] metsHdrAgents)
        {
            metsTypeMetsHdrAgent metsArchiveSystemAgent = metsHdrAgents.FirstOrDefault(a =>
                a.TYPE == metsTypeMetsHdrAgentTYPE.OTHER &&
                a.OTHERTYPE == metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE &&
                a.ROLE == metsTypeMetsHdrAgentROLE.OTHER &&
                a.OTHERROLE == metsTypeMetsHdrAgentOTHERROLE.PRODUCER
            );

            if (metsArchiveSystemAgent == null)
                return;

            var archiveSystem = new MetadataSystemInformationUnit();

            LoadSystemProperties(archiveSystem, metsArchiveSystemAgent);

            if (HasData(archiveSystem))
                archiveMetadata.ArchiveSystem = archiveSystem;
        }

        private static void LoadSystemProperties(MetadataSystemInformationUnit system,
            metsTypeMetsHdrAgent metsSystemAgent)
        {
            if (metsSystemAgent.name != null)
                system.Name = metsSystemAgent.name;

            string type = HdrAgentNotesLoader.GetType(metsSystemAgent.note);

            if (type != null)
                system.Type = type;

            string version = HdrAgentNotesLoader.GetVersion(metsSystemAgent.note);

            if (version != null)
                system.Version = version;

            string typeVersion = HdrAgentNotesLoader.GetTypeVersion(metsSystemAgent.note);

            if (typeVersion != null && MetsTranslationHelper.IsSystemTypeNoark5(system.Type))
                system.TypeVersion = typeVersion;
        }

        private static bool HasData(object anObject)
        {
            return anObject.GetType().GetProperties().Any(p => p.GetValue(anObject) != null);
        }

        private static DateTime? LoadDateOrNull(string dateValue)
        {
            return DateTime.TryParse(dateValue, out DateTime date) ? (DateTime?) date : null;
        }
    }
}
