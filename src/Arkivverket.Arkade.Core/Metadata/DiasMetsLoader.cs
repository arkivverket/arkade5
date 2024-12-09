using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.ExternalModels.DiasMets;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Metadata
{
    public static class DiasMetsLoader
    {
        public static ArchiveMetadata Load(string diasMetsFile)
        {
            var mets = SerializeUtil.DeserializeFromFile<mets>(diasMetsFile);

            var archiveMetadata = new ArchiveMetadata(); // NB! Metadata-origin (metadata loading)

            LoadMetsElementAttributes(archiveMetadata, mets);

            if (mets.metsHdr != null)
                LoadMetsHdr(archiveMetadata, mets.metsHdr);

            if (mets.fileSec?.fileGrp != null)
                LoadExtractionDate(archiveMetadata, mets.fileSec?.fileGrp);

            MetadataLoader.HandleLabelPlaceholder(archiveMetadata);

            return archiveMetadata;
        }

        private static void LoadExtractionDate(ArchiveMetadata archiveMetadata, IEnumerable<fileGrpType> fileGroups)
        {
            var archiveExtractionFileGroups = new List<fileGrpType>();

            foreach (fileGrpType fileGroup in fileGroups)
                CollectArchiveExtractionFileGroups(fileGroup, archiveExtractionFileGroups);

            if (archiveExtractionFileGroups.Count != 1)
                return;

            if (archiveExtractionFileGroups.First() is { VERSDATESpecified: true } extractionDatedFileGroup)
                archiveMetadata.ExtractionDate = extractionDatedFileGroup.VERSDATE;
        }

        private static void CollectArchiveExtractionFileGroups(fileGrpType fileGroup, List<fileGrpType> archiveExtractionFileGroups)
        {
            if (string.Equals(fileGroup.USE?.ToLower(), ArkadeConstants.MetsArchiveExtractionFileGroupUse.ToLower()))
                archiveExtractionFileGroups.Add(fileGroup);

            if (fileGroup.Items != null)
                foreach (fileGrpType subFileGroup in fileGroup.Items.Where(f => f is fileGrpType))
                    CollectArchiveExtractionFileGroups(subFileGroup, archiveExtractionFileGroups);
        }

        private static void LoadMetsElementAttributes(ArchiveMetadata archiveMetadata, mets mets)
        {
            archiveMetadata.Label = mets.LABEL;
        }

        private static void LoadMetsHdr(ArchiveMetadata archiveMetadata, metsTypeMetsHdr metsHdr)
        {
            archiveMetadata.RecordStatus = metsHdr.RECORDSTATUS;

            if (metsHdr.altRecordID != null)
                LoadMetsHdrAltRecordIDs(archiveMetadata, metsHdr.altRecordID);

            if (metsHdr.agent != null)
                LoadMetsHdrAgents(archiveMetadata, metsHdr.agent);
        }

        private static void LoadMetsHdrAltRecordIDs(ArchiveMetadata archiveMetadata, metsTypeMetsHdrAltRecordID[] altRecordIds)
        {
            foreach (metsTypeMetsHdrAltRecordID altRecordId in altRecordIds)
            {
                if (altRecordId.TYPE == AltRecordIdType.DELIVERYSPECIFICATION.ToString())
                    archiveMetadata.ArchiveDescription = altRecordId.Value;

                else if (altRecordId.TYPE == AltRecordIdType.SUBMISSIONAGREEMENT.ToString())
                    archiveMetadata.AgreementNumber = altRecordId.Value;

                else if (altRecordId.TYPE == AltRecordIdType.DELIVERYTYPE.ToString())
                    archiveMetadata.DeliveryType = altRecordId.Value;

                else if (altRecordId.TYPE == AltRecordIdType.PROJECTNAME.ToString())
                    archiveMetadata.ProjectName = altRecordId.Value;

                else if (altRecordId.TYPE == AltRecordIdType.PACKAGENUMBER.ToString())
                    archiveMetadata.PackageNumber = altRecordId.Value;

                else if (altRecordId.TYPE == AltRecordIdType.REFERENCECODE.ToString())
                    archiveMetadata.ReferenceCode = altRecordId.Value;

                else if (altRecordId.TYPE == AltRecordIdType.STARTDATE.ToString())
                    archiveMetadata.StartDate = LoadDateOrNull(altRecordId.Value);

                else if (altRecordId.TYPE == AltRecordIdType.ENDDATE.ToString())
                    archiveMetadata.EndDate = LoadDateOrNull(altRecordId.Value);
            }
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
                a.OTHERROLE.Equals(MetsHdrAgentOtherRoleType.SUBMITTER.ToString()) &&
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
                a.OTHERROLE.Equals(MetsHdrAgentOtherRoleType.PRODUCER.ToString()) &&
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

                    if (metsEntityAgent.note != null)
                    {
                        var notesLoader = new HdrAgentNotesLoader(metsEntityAgent.note);

                        string address = notesLoader.LoadAddress();

                        if (!string.IsNullOrEmpty(address))
                            entityInfoUnit.Address = address;

                        string phoneNumber = notesLoader.LoadTelephone();

                        if (!string.IsNullOrEmpty(phoneNumber))
                            entityInfoUnit.Telephone = phoneNumber;

                        string emailAddress = notesLoader.LoadEmail();

                        if (!string.IsNullOrEmpty(emailAddress))
                            entityInfoUnit.Email = emailAddress;
                    }
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
                // TODO: Include OTHERROLE in ARKADE 3.0?
                //a.OTHERROLE == MetsHdrAgentOtherRoleType.PRODUCER.ToString() &&
                // Including the above condition will break loading this data from dias-mets.xml files created with
                // Arkade versions 2.6.0 - 2.9.x
                a.ROLE == metsTypeMetsHdrAgentROLE.OTHER
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

            if (metsSystemAgent.note != null)
            {
                var notesLoader = new HdrAgentNotesLoader(metsSystemAgent.note);

                string type = notesLoader.LoadType();

                if (type != null)
                    system.Type = type;

                string version = notesLoader.LoadVersion();

                if (version != null)
                    system.Version = version;

                string typeVersion = notesLoader.LoadTypeVersion();

                if (typeVersion != null && MetsTranslationHelper.IsSystemTypeNoark5(system.Type))
                    system.TypeVersion = typeVersion;
            }
        }

        private static bool HasData(object anObject)
        {
            return anObject.GetType().GetProperties().Any(p => p.GetValue(anObject) != null);
        }

        private static DateTime? LoadDateOrNull(string dateValue)
        {
            return DateTime.TryParse(dateValue, out DateTime date) ? date : null;
        }
    }
}
