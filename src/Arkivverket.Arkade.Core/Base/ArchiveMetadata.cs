using System;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Base
{
    public class ArchiveMetadata
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string ArchiveDescription { get; set; }
        public string AgreementNumber { get; set; }
        public List<MetadataEntityInformationUnit> ArchiveCreators { get; set; }
        public MetadataEntityInformationUnit Transferer { get; set; }
        public MetadataEntityInformationUnit Producer { get; set; }
        public List<MetadataEntityInformationUnit> Owners { get; set; }
        public MetadataEntityInformationUnit Creator { get; set; }
        public MetadataSystemInformationUnit CreatorSoftwareSystem { get; set; }
        public string Recipient { get; set; }
        public MetadataSystemInformationUnit System { get; set; }
        public MetadataSystemInformationUnit ArchiveSystem { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ExtractionDate { get; set; }
        public List<FileDescription> FileDescriptions { get; set; }
        public PackageType PackageType { get; set; }
    }


    public class MetadataEntityInformationUnit
    {
        public string Entity { get; set; }
        public string ContactPerson { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
    }

    public class MetadataSystemInformationUnit
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Type { get; set; }
        public string TypeVersion { get; set; }
    }

    public class FileDescription
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Sha256Checksum { get; set; }
        public long Size { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
