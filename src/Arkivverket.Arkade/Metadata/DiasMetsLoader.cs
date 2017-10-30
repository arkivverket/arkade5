using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.ExternalModels.Mets;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Metadata
{
    public static class DiasMetsLoader
    {
        public static ArchiveMetadata Load(string diasMetsFile)
        {
            var mets = SerializeUtil.DeserializeFromFile<mets>(diasMetsFile);

            var archiveMetadata = new ArchiveMetadata();

            if (mets.metsHdr != null)
                LoadMetsHdr(archiveMetadata, mets.metsHdr);

            if (mets.amdSec != null)
                LoadAmdSec(archiveMetadata, mets.amdSec);

            return archiveMetadata;
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
            LoadRecipient(archiveMetadata, metsHdrAgents);
            LoadSystem(archiveMetadata, metsHdrAgents);
            LoadArchiveSystem(archiveMetadata, metsHdrAgents);
        }

        private static void LoadAmdSec(ArchiveMetadata archiveMetadata, IEnumerable<amdSecType> metsAmdSec)
        {
            LoadComments(archiveMetadata, metsAmdSec);
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

                    string phoneNumber = metsEntityAgent.note?.FirstOrDefault(LooksLikePhoneNumber);

                    if (!string.IsNullOrEmpty(phoneNumber))
                        entityInfoUnit.Telephone = phoneNumber;

                    string emailAddress = metsEntityAgent.note?.FirstOrDefault(LooksLikeEmailAddress);

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
            if (metsSystemAgent.name != null && LooksLikeSystemName(metsSystemAgent.name))
                system.Name = metsSystemAgent.name;

            string type = metsSystemAgent.note?.FirstOrDefault(LooksLikeSystemType);

            if (type != null)
                system.Type = type;

            // Find first occurance of a version number defined before Type. That's probably the version ...
            string version = metsSystemAgent.note?.TakeWhile(n => !n.Equals(type))
                .FirstOrDefault(LooksLikeSystemVersion);

            if (version != null)
                system.Version = version;

            // Find first occurance of a version number defined after Type. That's probably the type-version ...
            string typeVersion = metsSystemAgent.note?.SkipWhile(n => !n.Equals(type)).FirstOrDefault(LooksLikeSystemTypeVersion);

            if (typeVersion != null && MetsTranslationHelper.IsSystemTypeNoark5(system.Type))
                system.TypeVersion = typeVersion;
        }

        private static void LoadComments(ArchiveMetadata archiveMetadata, IEnumerable<amdSecType> amdSecTypes)
        {
            // TODO: Implement

            /*
            var archiveMetadataComments = new List<string>();

            foreach (amdSecType amdSecType in amdSecTypes)
            {
                IEnumerable<mdSecType> metsCommentMdSecTypes = amdSecType.techMD.Where(m =>
                        m.mdWrap.MDTYPE == mdSecTypeMdRefMDTYPE.OTHER &&
                        m.mdWrap.OTHERMDTYPE == mdSecTypeMdRefOTHERMDTYPE.COMMENT
                );

                foreach (mdSecType metsCommentMdSecType in metsCommentMdSecTypes)
                    archiveMetadataComments.Add(metsCommentMdSecType.mdWrap.Item as string);
            }

            if (archiveMetadataComments.Any())
                archiveMetadata.Comments = archiveMetadataComments;
            */
        }

        private static bool LooksLikePhoneNumber(string possiblePhoneNumber)
        {
            return Regex.IsMatch(possiblePhoneNumber, @"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$");
        }

        private static bool LooksLikeEmailAddress(string possibleEmailAddress)
        {
            return Regex.IsMatch(possibleEmailAddress,
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                RegexOptions.IgnoreCase
            );
        }

        private static bool LooksLikeSystemName(string possibleSystemName)
        {
            return !LooksLikeSystemVersion(possibleSystemName) &&
                   !LooksLikeSystemType(possibleSystemName) &&
                   !LooksLikeSystemTypeVersion(possibleSystemName);
        }

        private static bool LooksLikeSystemVersion(string possibleSystemVersion)
        {
            return LooksLikeVersionNumber(possibleSystemVersion);
        }

        private static bool LooksLikeSystemType(string possibleSystemType)
        {
            return MetsTranslationHelper.IsValidSystemType(possibleSystemType);
        }

        private static bool LooksLikeSystemTypeVersion(string possibleSystemTypeVersion)
        {
            return LooksLikeVersionNumber(possibleSystemTypeVersion);
        }

        private static bool LooksLikeVersionNumber(string possibleVersionNumber)
        {
            return Regex.IsMatch(possibleVersionNumber, @"\d+(\.\d+)+");
        }

        private static bool HasData(object anObject)
        {
            return anObject.GetType().GetProperties().Any(p => p.GetValue(anObject) != null);
        }

        private static DateTime? LoadDateOrNull(string dateValue)
        {
            DateTime date;

            if (DateTime.TryParse(dateValue, out date))
                return date;

            return null;
        }
    }
}
