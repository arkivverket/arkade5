using System.Collections.Generic;
﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.ExternalModels.Mets;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Metadata
{
    public class MetsCreator
    {
        protected static XmlSerializerNamespaces SetupNamespaces()
        {
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("mets", "http://www.loc.gov/METS/");
            namespaces.Add("xlink", "http://www.w3.org/1999/xlink");
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            return namespaces;
        }

        public static mets Create(ArchiveMetadata metadata)
        {
            var mets = new mets();

            CreateMetsElementAttributes(mets, metadata);

            CreateMetsHdr(mets, metadata);

            CreateAmdSec(mets, metadata);

            CreateFileSec(mets, metadata);

            CreateStructMap(mets, metadata);

            return mets;
        }

        private static void CreateMetsElementAttributes(mets mets, ArchiveMetadata metadata)
        {
            mets.OBJID = metadata.Id;
            mets.PROFILE = "http://xml.ra.se/METS/RA_METS_eARD.xml";
            mets.LABEL = $"{metadata.System.Name}";

            if (metadata.StartDate != null && metadata.EndDate != null)
                mets.LABEL += $" ({metadata.StartDate?.Year} - {metadata.EndDate?.Year})";
        }

        private static void CreateMetsHdr(metsType mets, ArchiveMetadata metadata)
        {
            var metsHdr = new metsTypeMetsHdr();

            if (metadata.ExtractionDate != null)
                metsHdr.CREATEDATE = (DateTime) metadata.ExtractionDate;
            
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

            if (metadata.StartDate != null)
            {
                altRecordIDs.Add(new metsTypeMetsHdrAltRecordID
                {
                    TYPESpecified = true,
                    TYPE = metsTypeMetsHdrAltRecordIDTYPE.STARTDATE,
                    Value = ((DateTime) metadata.StartDate).ToShortDateString()
                });
            }

            if (metadata.EndDate != null)
            {
                altRecordIDs.Add(new metsTypeMetsHdrAltRecordID
                {
                    TYPESpecified = true,
                    TYPE = metsTypeMetsHdrAltRecordIDTYPE.ENDDATE,
                    Value = ((DateTime) metadata.EndDate).ToShortDateString()
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

                    if (HasContactData(metadataArchiveCreator))
                    {
                        metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                        {
                            TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                            ROLE = metsTypeMetsHdrAgentROLE.ARCHIVIST,
                            name = metadataArchiveCreator.ContactPerson,
                            note = new[] { metadataArchiveCreator.Telephone, metadataArchiveCreator.Email }
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
                        note = new[] { metadata.Transferer.Telephone, metadata.Transferer.Email }
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
                        note = new[] { metadata.Producer.Telephone, metadata.Producer.Email }
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
                            note = new[] { metadataOwner.Telephone, metadataOwner.Email }
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
                    var systemAgent = new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.OTHER,
                        OTHERTYPESpecified = true,
                        OTHERTYPE = metsTypeMetsHdrAgentOTHERTYPE.SOFTWARE,
                        ROLE = metsTypeMetsHdrAgentROLE.ARCHIVIST,
                        name = system.Name
                    };

                    systemAgent.note = GetSystemPropertiesNotes(system);

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

                    archiveSystemAgent.note = GetSystemPropertiesNotes(archiveSystem);

                    metsTypeMetsHdrAgents.Add(archiveSystemAgent);
                }
            }

            if (metsTypeMetsHdrAgents.Any())
                metsHdr.agent = metsTypeMetsHdrAgents.ToArray();
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
                        href = fileDescription.Name.Replace("\\", "/"),
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

        private static void CreateStructMap(mets mets, ArchiveMetadata metadata)
        {
            mets.structMap = new[] { new structMapType { div = new divType() } };
        }

        protected static List<FileDescription> GetFileDescriptions(DirectoryInfo directory,
            DirectoryInfo pathRoot = null)
        {
            var fileDescriptions = new List<FileDescription>();

            var fileId = 1; // Reserving 0 for package file

            foreach (FileInfo file in directory.EnumerateFiles(".", SearchOption.AllDirectories))
                fileDescriptions.Add(GetFileDescription(file, ref fileId, pathRoot));

            return fileDescriptions;
        }

        public static FileDescription GetFileDescription(FileInfo file, ref int fileId, DirectoryInfo pathRoot = null)
        {
            string fileName = file.FullName;

            if (pathRoot != null)
            {
                // Makes fileName contain path from pathRoot only:
                string excludedPath = pathRoot.FullName + Path.DirectorySeparatorChar;
                fileName = file.FullName.Replace(excludedPath, string.Empty);
            }

            return new FileDescription
            {
                Id = fileId++,
                Name = fileName,
                Extension = file.Extension.Replace(".", string.Empty),
                Sha256Checksum = GetSha256Checksum(file),
                Size = file.Length,
                CreationTime = file.CreationTime
            };
        }

        private static string GetSha256Checksum(FileInfo file)
        {
            return new Sha256ChecksumGenerator().GenerateChecksum(file.FullName);
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

        private static string[] GetSystemPropertiesNotes(MetadataSystemInformationUnit system)
        {
            var notes = new List<string>();

            if (!string.IsNullOrEmpty(system.Version))
            {
                notes.Add(system.Version);
            }

            if (!string.IsNullOrEmpty(system.Type) &&
                MetsTranslationHelper.IsValidSystemType(system.Type))
            {
                notes.Add(system.Type);
            }

            if (!string.IsNullOrEmpty(system.TypeVersion) &&
                MetsTranslationHelper.IsSystemTypeNoark5(system.Type))
            {
                notes.Add(system.TypeVersion);
            }

            return notes.Any() ? notes.ToArray() : null;
        }
    }
}
