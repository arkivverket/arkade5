using System;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Core
{
    public class ArchiveMetadata
    {
        public string ArchiveDescription { get; set; }
        public string AgreementNumber { get; set; }
        public List<MetadataEntityInformationUnit> ArchiveCreators { get; set; }
        public MetadataEntityInformationUnit Transferer { get; set; }
        public MetadataEntityInformationUnit Producer { get; set; }
        public List<MetadataEntityInformationUnit> Owners { get; set; }
        public string Recipient { get; set; }
        public MetadataSystemInformationUnit System { get; set; }
        public MetadataSystemInformationUnit ArchiveSystem { get; set; }
        public List<string> Comments { get; set; }
        public string History { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ExtractionDate { get; set; }
        public string IncommingSeparator { get; set; }
        public string OutgoingSeparator { get; set; }
    }


    public class MetadataEntityInformationUnit
    {
        public string Entity { get; set; }
        public string ContactPerson { get; set; }
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
}
