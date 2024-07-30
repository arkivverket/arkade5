using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.ExternalModels.DiasMets;
using Arkivverket.Arkade.Core.Util;
using Serilog;
 
namespace Arkivverket.Arkade.Core.Metadata
{
    public class DiasMetsCreator : MetsCreator<mets>
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public void CreateAndSaveFile(Archive archive, ArchiveMetadata metadata)
        {
            DirectoryInfo rootDirectory = archive.WorkingDirectory.Root().DirectoryInfo();

            if (rootDirectory.Exists)
            {
                string[] filesToSkip = metadata.PackageType == PackageType.SubmissionInformationPackage
                    ? new[] { ArkadeConstants.EadXmlFileName, ArkadeConstants.EacCpfXmlFileName }
                    : null;

                metadata.FileDescriptions = GetFileDescriptions(rootDirectory, rootDirectory, filesToSkip: filesToSkip);
            }

            if (archive.WorkingDirectory.HasExternalContentDirectory())
            {
                DirectoryInfo externalContentDirectory = archive.WorkingDirectory.Content().DirectoryInfo();

                if (externalContentDirectory.Exists)
                {
                    string[] directoriesToSkip = ArkadeConstants.DocumentDirectoryNames;

                    // The loaded archive (as dictionary) might have a name different from "content", therefore it is
                    // necessary to strip the name of the source "content" directory from the file descriptions, 
                    // before prepending "content/" to their names - to ensure the files are correctly referred in
                    // the dias-mets.xml of the produced IP.
                    List<FileDescription> fileDescriptions = GetFileDescriptions(externalContentDirectory, 
                        externalContentDirectory, directoriesToSkip);

                    PrependFileDescriptionsNameWithContent(fileDescriptions);

                    metadata.FileDescriptions.AddRange(fileDescriptions);
                }
            }

            if (archive.ArchiveType is ArchiveType.Noark5)
            {
                if (!archive.DocumentFiles.AreMetsReady())
                    archive.DocumentFiles.Register(includeChecksums: true);

                ReadOnlyDictionary<string, DocumentFile> documentFiles = archive.DocumentFiles.Get();

                metadata.FileDescriptions.AddRange(GetFileDescriptionsFromDocumentFiles(documentFiles));
            }

            const int fileIdOffset = 1; // Reserving 0 for package file
            AutoIncrementFileIds(metadata.FileDescriptions, fileIdOffset);

            mets mets = Create(metadata);

            FileInfo targetFileName = archive.WorkingDirectory.Root().WithFile(ArkadeConstants.DiasMetsXmlFileName);

            XmlSerializerNamespaces namespaces = SetupNamespaces();

            SerializeUtil.SerializeToFile(mets, targetFileName, namespaces);

            Log.Debug($"Created {ArkadeConstants.DiasMetsXmlFileName}");
        }

        private static void PrependFileDescriptionsNameWithContent(List<FileDescription> fileDescriptions)
        {
            foreach (FileDescription fileDescription in fileDescriptions)
                fileDescription.Name = Path.Combine(ArkadeConstants.DirectoryNameContent, fileDescription.Name);
        }

        private static IEnumerable<FileDescription> GetFileDescriptionsFromDocumentFiles(ReadOnlyDictionary<string, DocumentFile> documentFiles)
        {
            foreach ((string name, DocumentFile documentFile) in documentFiles)
            {
                yield return new FileDescription
                {
                    Name = Path.Combine(ArkadeConstants.DirectoryNameContent, name),
                    Extension = documentFile.Extension,
                    Sha256Checksum = documentFile.CheckSum,
                    Size = documentFile.Size,
                    CreationTime = documentFile.CreationTime
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
                metsHdr.CREATEDATE = metadata.ExtractionDate.Value;
                
            if (!string.IsNullOrEmpty(metadata.RecordStatus))
                metsHdr.RECORDSTATUS = metadata.RecordStatus;

            CreateAltRecordIDs(metsHdr, metadata);

            metsHdr.metsDocumentID = new metsTypeMetsHdrMetsDocumentID { Value = ArkadeConstants.DiasMetsXmlFileName };

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
                    TYPE = AltRecordIdType.DELIVERYSPECIFICATION.ToString(),
                    Value = metadata.ArchiveDescription
                });
            }

            if (!string.IsNullOrEmpty(metadata.AgreementNumber))
            {
                altRecordIDs.Add(new metsTypeMetsHdrAltRecordID
                {
                    TYPE = AltRecordIdType.SUBMISSIONAGREEMENT.ToString(),
                    Value = metadata.AgreementNumber
                });
            }

            if (!string.IsNullOrEmpty(metadata.DeliveryType))
            {
                altRecordIDs.Add(new metsTypeMetsHdrAltRecordID
                {
                    TYPE = AltRecordIdType.DELIVERYTYPE.ToString(),
                    Value = metadata.DeliveryType
                });
            }

            if (!string.IsNullOrEmpty(metadata.ProjectName))
            {
                altRecordIDs.Add(new metsTypeMetsHdrAltRecordID
                {
                    TYPE = AltRecordIdType.PROJECTNAME.ToString(),
                    Value = metadata.ProjectName
                });
            }

            if (!string.IsNullOrEmpty(metadata.PackageNumber))
            {
                altRecordIDs.Add(new metsTypeMetsHdrAltRecordID
                {
                    TYPE = AltRecordIdType.PACKAGENUMBER.ToString(),
                    Value = metadata.PackageNumber
                });
            }

            if (!string.IsNullOrEmpty(metadata.ReferenceCode))
            {
                altRecordIDs.Add(new metsTypeMetsHdrAltRecordID
                {
                    TYPE = AltRecordIdType.REFERENCECODE.ToString(),
                    Value = metadata.ReferenceCode
                });
            }

            if (metadata.StartDate != null)
            {
                altRecordIDs.Add(new metsTypeMetsHdrAltRecordID
                {
                    TYPE = AltRecordIdType.STARTDATE.ToString(),
                    Value = ((DateTime)metadata.StartDate).ToString(DateFormat)
                });
            }

            if (metadata.EndDate != null)
            {
                altRecordIDs.Add(new metsTypeMetsHdrAltRecordID
                {
                    TYPE = AltRecordIdType.ENDDATE.ToString(),
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
                        OTHERROLE = MetsHdrAgentOtherRoleType.SUBMITTER.ToString(),
                        name = metadata.Transferer.Entity
                    });
                }

                if (HasContactData(metadata.Transferer))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                        ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                        OTHERROLE = MetsHdrAgentOtherRoleType.SUBMITTER.ToString(),
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
                        OTHERROLE = MetsHdrAgentOtherRoleType.PRODUCER.ToString(),
                        name = metadata.Producer.Entity
                    });
                }

                if (HasContactData(metadata.Producer))
                {
                    metsTypeMetsHdrAgents.Add(new metsTypeMetsHdrAgent
                    {
                        TYPE = metsTypeMetsHdrAgentTYPE.INDIVIDUAL,
                        ROLE = metsTypeMetsHdrAgentROLE.OTHER,
                        OTHERROLE = MetsHdrAgentOtherRoleType.PRODUCER.ToString(),
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
                        OTHERROLE = MetsHdrAgentOtherRoleType.PRODUCER.ToString(),
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
                    MIMETYPE = MimeTypeParser(fileDescription),
                    USE = "Datafile",
                    CHECKSUMTYPE = mdSecTypeMdRefCHECKSUMTYPE.SHA256,
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

        private static mdSecTypeMdRefMIMETYPE MimeTypeParser(FileDescription fileDescription)
        {
            //https://mimetype.io/
            return fileDescription.Extension.Split('.').Last().ToLower() switch
            {
                "pdf" => mdSecTypeMdRefMIMETYPE.imagepdf,
                "jpe" or "jpeg" or "jpg" or "pjpg" or "jfif" or "jfif-tbnl" or "jif" => mdSecTypeMdRefMIMETYPE.imagejpg, 
                "tiff" or "tif" => mdSecTypeMdRefMIMETYPE.imagetiff,
                "xml" or "xpdl" or "xsl" or "gml" or "xsd" => mdSecTypeMdRefMIMETYPE.applicationxml,
                "tar" => mdSecTypeMdRefMIMETYPE.applicationxtar,
                "m2a" or "m3a" or "mp2" or "mp2a" or "mp3" or "mpga" => mdSecTypeMdRefMIMETYPE.audiomp3,
                "m1v" or "m2v" or "mpa" or "mpe" or "mpeg" or "mpg" => mdSecTypeMdRefMIMETYPE.videompg,
                "conf" or "def" or "diff" or "in" or "ksh" or "list" or "log" or "pl" or "text" or "txt" => mdSecTypeMdRefMIMETYPE.textplain,
                _ => mdSecTypeMdRefMIMETYPE.textplain // todo: should have a 'undefined' fallback of sorts - not possible with current version (1.9) of DIAS-METS.xsd
            };
        }
    }
}
