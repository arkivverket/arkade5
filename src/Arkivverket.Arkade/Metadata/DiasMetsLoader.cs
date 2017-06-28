using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.ExternalModels.DiasMets;
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
                a.TYPE.Equals("DELIVERYSPECIFICATION"))?.Value;
        }

        private static void LoadAgreementNumber(ArchiveMetadata archiveMetadata,
            IEnumerable<metsTypeMetsHdrAltRecordID> metsHdrAltRecordIds)
        {
            archiveMetadata.AgreementNumber = metsHdrAltRecordIds.FirstOrDefault(a =>
                a.TYPE.Equals("SUBMISSIONAGREEMENT"))?.Value;
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
                a.OTHERROLE.Equals("SUBMITTER") &&
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
                a.OTHERROLE.Equals("PRODUCER") &&
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
                    {
                        entityInfoUnit.ContactPerson = metsEntityAgent.name;
                        continue;
                    }

                    string phoneNumber = metsEntityAgent.note?.FirstOrDefault(LooksLikePhoneNumber);

                    if (!string.IsNullOrEmpty(phoneNumber))
                    {
                        entityInfoUnit.Telephone = phoneNumber;
                        continue;
                    }

                    string emailAddress = metsEntityAgent.note?.FirstOrDefault(LooksLikeEmailAddress);

                    if (!string.IsNullOrEmpty(emailAddress))
                    {
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
            metsTypeMetsHdrAgent[] metsSystemAgents = metsHdrAgents.Where(a =>
                a.TYPE == metsTypeMetsHdrAgentTYPE.OTHER &&
                a.OTHERTYPE == metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE &&
                a.ROLE == metsTypeMetsHdrAgentROLE.ARCHIVIST
            ).ToArray();

            if (!metsSystemAgents.Any()) return;

            var system = new MetadataSystemInformationUnit();

            LoadSystemProperties(system, metsSystemAgents);

            if (HasData(system))
                archiveMetadata.System = system;
        }

        private static void LoadArchiveSystem(ArchiveMetadata archiveMetadata, metsTypeMetsHdrAgent[] metsHdrAgents)
        {
            metsTypeMetsHdrAgent[] metsArchiveSystemAgents = metsHdrAgents.Where(a =>
                a.TYPE == metsTypeMetsHdrAgentTYPE.OTHER &&
                a.OTHERTYPE == metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE &&
                a.ROLE == metsTypeMetsHdrAgentROLE.OTHER &&
                a.OTHERROLE == "PRODUCER"
            ).ToArray();

            if (!metsArchiveSystemAgents.Any())
                return;

            var archiveSystem = new MetadataSystemInformationUnit();

            LoadSystemProperties(archiveSystem, metsArchiveSystemAgents);

            if (HasData(archiveSystem))
                archiveMetadata.ArchiveSystem = archiveSystem;
        }

        private static void LoadSystemProperties(MetadataSystemInformationUnit system,
            metsTypeMetsHdrAgent[] metsSystemAgents)
        {
            foreach (metsTypeMetsHdrAgent metsSystemAgent in metsSystemAgents)
            {
                if (metsSystemAgent.name != null && LooksLikeSystemName(metsSystemAgent.name))
                {
                    system.Name = metsSystemAgent.name;
                    continue;
                }

                string version = metsSystemAgent.note?.FirstOrDefault(LooksLikeSystemVersion);

                if (version != null && system.Version == null) // May be TypeVersion (found after Version)
                {
                    system.Version = version;
                    continue;
                }

                string type = metsSystemAgent.note?.FirstOrDefault(LooksLikeSystemType);

                if (type != null)
                {
                    system.Type = type;
                    continue;
                }

                string typeVersion = metsSystemAgent.note?.FirstOrDefault(LooksLikeSystemTypeVersion);

                if (typeVersion != null && MetsTranslationHelper.IsSystemTypeNoark5(system.Type))
                {
                    system.TypeVersion = typeVersion;
                }
            }
        }

        private static void LoadComments(ArchiveMetadata archiveMetadata, IEnumerable<amdSecType> amdSecTypes)
        {
            // Implement when type mdSecTypeMdRefOTHERMDTYPE.COMMENT is supported in built in mets schema

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
    }
}
