using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.ExternalModels.SubmissionDescription;
using Arkivverket.Arkade.Core.Util;
using Serilog;

namespace Arkivverket.Arkade.Core.Metadata
{
    public class SubmissionDescriptionCreator : MetsCreator<mets>
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public void CreateAndSaveFile(ArchiveMetadata metadata, string packageFileName, string diasMetsFileName,
            string submissionDescriptionFileName)
        {
            var packageFile = new FileInfo(packageFileName);
            var diasMetsFile = new FileInfo(diasMetsFileName);

            PrepareForPackageDescription(metadata, packageFile, diasMetsFile);

            mets submissionDescription = Create(metadata);

            var targetFileObject = new FileInfo(
                Path.Combine(packageFile.DirectoryName, submissionDescriptionFileName)
            );

            XmlSerializerNamespaces namespaces = SetupNamespaces();

            SerializeUtil.SerializeToFile(submissionDescription, targetFileObject, namespaces);

            Log.Debug($"Created {targetFileObject}");
        }

        private static void PrepareForPackageDescription(ArchiveMetadata metadata, FileInfo packageFile,
            FileInfo diasMetsFile)
        {
            metadata.FileDescriptions = null; // Removes any existing file-descriptions

            if (packageFile.Exists && diasMetsFile.Exists)
            {
                FileDescription informationPackageFileDescription = GetFileDescription
                (
                    packageFile,
                    packageFile.Directory
                );

                FileDescription metsFileDescription = GetFileDescription
                (
                    diasMetsFile,
                    diasMetsFile.Directory
                );

                string metsFileDirectoryName = Path.GetFileNameWithoutExtension(packageFile.Name);

                informationPackageFileDescription.Id = 0;
                metsFileDescription.Id = 1;
                metsFileDescription.Name = Path.Combine(metsFileDirectoryName, diasMetsFile.Name);

                metadata.FileDescriptions = new List<FileDescription>
                {
                    informationPackageFileDescription,
                    metsFileDescription
                };
            }
        }

        public static mets Create(ArchiveMetadata metadata)
        {
            MetadataCleaner.Clean(metadata);

            var mets = new mets();

            CreateMetsElementAttributes(mets, metadata);

            CreateMetsHdr(mets, metadata);

            CreateAmdSec(mets, metadata);

            CreateFileSec(mets, metadata);

            CreateStructMap(mets);

            return mets;
        }

        private static void CreateMetsElementAttributes(mets mets, ArchiveMetadata metadata)
        {
            mets.OBJID = metadata.Id;
            mets.PROFILE = "http://xml.ra.se/METS/RA_METS_eARD.xml";

            mets.TYPE = metadata.PackageType == PackageType.SubmissionInformationPackage
                ? metsTypeTYPE.SIP
                : metsTypeTYPE.AIP;

            if (!string.IsNullOrEmpty(metadata.Label))
                mets.LABEL = metadata.Label;
        }

        private static void CreateMetsHdr(mets mets, ArchiveMetadata metadata)
        {
            var metsHdr = new metsTypeMetsHdr();

            if (metadata.ExtractionDate != null)
                metsHdr.CREATEDATE = (DateTime)metadata.ExtractionDate;

            if (!string.IsNullOrEmpty(metadata.RecordStatus) &&
                Enum.TryParse(metadata.RecordStatus, out metsTypeMetsHdrRECORDSTATUS recordStatus))
            {
                metsHdr.RECORDSTATUSSpecified = true;
                metsHdr.RECORDSTATUS = recordStatus;
            }

            CreateAltRecordIDs(metsHdr, metadata);

            CreateHdrAgents(metsHdr, metadata);

            if (metadata.ExtractionDate != null || metsHdr.altRecordID != null || metsHdr.agent != null)
                mets.metsHdr = metsHdr;
        }

        private static void CreateAltRecordIDs(metsTypeMetsHdr metsHdr, ArchiveMetadata metadata)
        {
            var altRecordIDs = new List<metsTypeMetsHdrAltRecordID>();

            if (!string.IsNullOrEmpty(metadata.ArchiveDescription))
            {
                altRecordIDs.Add(new metsTypeMetsHdrAltRecordID
                {
                    TYPESpecified = true,
                    TYPE = metsTypeMetsHdrAltRecordIDTYPE.DELIVERYSPECIFICATION,
                    Value = metadata.ArchiveDescription
                });
            }

            if (!string.IsNullOrEmpty(metadata.AgreementNumber))
            {
                altRecordIDs.Add(new metsTypeMetsHdrAltRecordID
                {
                    TYPESpecified = true,
                    TYPE = metsTypeMetsHdrAltRecordIDTYPE.SUBMISSIONAGREEMENT,
                    Value = metadata.AgreementNumber
                });
            }

            if (!string.IsNullOrEmpty(metadata.DeliveryType))
            {
                altRecordIDs.Add(new metsTypeMetsHdrAltRecordID
                {
                    TYPESpecified = true,
                    TYPE = metsTypeMetsHdrAltRecordIDTYPE.DELIVERYTYPE,
                    Value = metadata.DeliveryType
                });
            }

            if (!string.IsNullOrEmpty(metadata.PackageNumber))
            {
                altRecordIDs.Add(new metsTypeMetsHdrAltRecordID
                {
                    TYPESpecified = true,
                    TYPE = metsTypeMetsHdrAltRecordIDTYPE.PACKAGENUMBER,
                    Value = metadata.PackageNumber
                });
            }

            if (!string.IsNullOrEmpty(metadata.ReferenceCode))
            {
                altRecordIDs.Add(new metsTypeMetsHdrAltRecordID
                {
                    TYPESpecified = true,
                    TYPE = metsTypeMetsHdrAltRecordIDTYPE.REFERENCECODE,
                    Value = metadata.ReferenceCode
                });
            }

            if (metadata.StartDate != null)
            {
                altRecordIDs.Add(new metsTypeMetsHdrAltRecordID
                {
                    TYPESpecified = true,
                    TYPE = metsTypeMetsHdrAltRecordIDTYPE.STARTDATE,
                    Value = ((DateTime)metadata.StartDate).ToString(DateFormat)
                });
            }

            if (metadata.EndDate != null)
            {
                altRecordIDs.Add(new metsTypeMetsHdrAltRecordID
                {
                    TYPESpecified = true,
                    TYPE = metsTypeMetsHdrAltRecordIDTYPE.ENDDATE,
                    Value = ((DateTime)metadata.EndDate).ToString(DateFormat)
                });
            }

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

                    if (HasContactData(metadataArchiveCreator))
                    {
                        metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                        {
                            TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                            ROLE = metsTypeMetsHdrAgentROLE.ARCHIVIST,
                            name = metadataArchiveCreator.ContactPerson,
                            note = GetEntityInfoUnitNotes(metadataArchiveCreator)
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
                        OTHERROLESpecified = true,
                        OTHERROLE = metsTypeMetsHdrAgentOTHERROLE.SUBMITTER,
                        name = metadata.Transferer.Entity
                    });
                }

                if (HasContactData(metadata.Transferer))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                        ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                        OTHERROLESpecified = true,
                        OTHERROLE = metsTypeMetsHdrAgentOTHERROLE.SUBMITTER,
                        name = metadata.Transferer.ContactPerson,
                        note = GetEntityInfoUnitNotes(metadata.Transferer)
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
                        OTHERROLESpecified = true,
                        OTHERROLE = metsTypeMetsHdrAgentOTHERROLE.PRODUCER,
                        name = metadata.Producer.Entity
                    });
                }

                if (HasContactData(metadata.Producer))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                        ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                        OTHERROLESpecified = true,
                        OTHERROLE = metsTypeMetsHdrAgentOTHERROLE.PRODUCER,
                        name = metadata.Producer.ContactPerson,
                        note = GetEntityInfoUnitNotes(metadata.Producer)
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

                    if (HasContactData(metadataOwner))
                    {
                        metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                        {
                            TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                            ROLE = metsTypeMetsHdrAgentROLE.IPOWNER,
                            name = metadataOwner.ContactPerson,
                            note = GetEntityInfoUnitNotes(metadataOwner)
                        });
                    }
                }
            }

            // CREATOR:

            if (metadata.Creator != null)
            {
                if (HasEntity(metadata.Creator))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.ORGANIZATION,
                        ROLE = metsTypeMetsHdrAgentROLE.CREATOR,
                        name = metadata.Creator.Entity
                    });
                }

                if (HasContactData(metadata.Creator))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                        ROLE = metsTypeMetsHdrAgentROLE.CREATOR,
                        name = metadata.Creator.ContactPerson,
                        note = GetEntityInfoUnitNotes(metadata.Creator)
                    });
                }
            }

            // CREATOR SOFTWARE SYSTEM:

            metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
            {
                ROLE = metsTypeMetsHdrAgentROLE.CREATOR,
                TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                OTHERTYPESpecified = true,
                OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                name = "Arkade 5",
                note = new[] { $"{ArkadeVersion.Current}", "notescontent:Version" }
            });

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
                    var systemAgent = new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                        OTHERTYPESpecified = true,
                        OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                        ROLE = metsTypeMetsHdrAgentROLE.ARCHIVIST,
                        name = system.Name
                    };

                    systemAgent.note = GetSystemInfoUnitNotes(system);

                    metsTypeMetsHdrAgents.Add(systemAgent);
                }
            }

            // ARCHIVE SYSTEM:

            if (metadata.ArchiveSystem != null)
            {
                MetadataSystemInformationUnit archiveSystem = metadata.ArchiveSystem;

                if (!string.IsNullOrEmpty(archiveSystem.Name))
                {
                    var archiveSystemAgent = new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                        OTHERTYPESpecified = true,
                        OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                        ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                        OTHERROLESpecified = true,
                        OTHERROLE = metsTypeMetsHdrAgentOTHERROLE.PRODUCER,
                        name = archiveSystem.Name
                    };

                    archiveSystemAgent.note = GetSystemInfoUnitNotes(archiveSystem);

                    metsTypeMetsHdrAgents.Add(archiveSystemAgent);
                }
            }

            if (metsTypeMetsHdrAgents.Any())
                metsHdr.agent = metsTypeMetsHdrAgents.ToArray();
        }

        private static string[] GetEntityInfoUnitNotes(MetadataEntityInformationUnit entity)
        {
            var notesWriter = new HdrAgentNotesWriter();

            if (!string.IsNullOrWhiteSpace(entity.Address))
                notesWriter.AddAddress(entity.Address);

            if (!string.IsNullOrWhiteSpace(entity.Telephone))
                notesWriter.AddTelephone(entity.Telephone);

            if (!string.IsNullOrWhiteSpace(entity.Email))
                notesWriter.AddEmail(entity.Email);

            return notesWriter.HasNotes() ? notesWriter.GetNotes() : null;
        }

        private static string[] GetSystemInfoUnitNotes(MetadataSystemInformationUnit system)
        {
            var notesWriter = new HdrAgentNotesWriter();

            if (!string.IsNullOrWhiteSpace(system.Version))
                notesWriter.AddVersion(system.Version);

            if (!string.IsNullOrWhiteSpace(system.Type) && MetsTranslationHelper.IsValidSystemType(system.Type))
                notesWriter.AddType(system.Type);

            if (!string.IsNullOrWhiteSpace(system.TypeVersion) && MetsTranslationHelper.IsSystemTypeNoark5(system.Type))
                notesWriter.AddTypeVersion(system.TypeVersion);

            return notesWriter.HasNotes() ? notesWriter.GetNotes() : null;
        }

        private static void CreateAmdSec(metsType mets, ArchiveMetadata metadata)
        {
            // TODO: Implement
        }

        private static void CreateFileSec(mets mets, ArchiveMetadata metadata)
        {
            if (metadata.FileDescriptions == null || !metadata.FileDescriptions.Any())
                return;

            var metsFiles = new List<object>();

            foreach (FileDescription fileDescription in metadata.FileDescriptions)
            {
                metsFiles.Add(new fileType
                {
                    ID = $"fileId_{fileDescription.Id}",
                    MIMETYPE = $"application/{fileDescription.Extension}",
                    USE = "Datafile",
                    CHECKSUMTYPESpecified = true,
                    CHECKSUMTYPE = fileTypeCHECKSUMTYPE.SHA256,
                    CHECKSUM = fileDescription.Sha256Checksum.ToLower(),
                    SIZE = fileDescription.Size,
                    CREATED = fileDescription.CreationTime,
                    FLocat = new fileTypeFLocat
                    {
                        href = "file:" + fileDescription.Name.Replace("\\", "/"),
                        LOCTYPE = mdSecTypeMdRefLOCTYPE.URL
                    }
                });
            }

            var metsTypeFileSecFileGrp = new metsTypeFileSecFileGrp
            {
                ID = "fileGroup001",
                USE = "FILES",
                Items = metsFiles.ToArray()
            };

            mets.fileSec = new metsTypeFileSec { fileGrp = new[] { metsTypeFileSecFileGrp } };
        }

        private static void CreateStructMap(mets mets)
        {
            mets.structMap = new[] { new structMapType { div = new divType() } };
        }
    }
}
