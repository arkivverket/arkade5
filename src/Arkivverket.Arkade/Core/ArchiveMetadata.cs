using System;
using System.Collections.Generic;
using Arkivverket.Arkade.ExternalModels.Cpf;
using list = Arkivverket.Arkade.ExternalModels.Ead.list;

namespace Arkivverket.Arkade.Core
{
    public class ArchiveMetadata
    {

        public string ArchiveDescription { get; set; }
        public string AgreementNumber { get; set; }
        public List<MetadataEntityInformationUnit> ArchiveCreator { get; set; }
        public MetadataEntityInformationUnit Transferer { get; set; }
        public MetadataEntityInformationUnit Producer { get; set; }
        public List<MetadataEntityInformationUnit> Owner { get; set; }
        public string  Recipient { get; set; }
        public MetadataSystemInformationUnit System { get; set; }
        public MetadataSystemInformationUnit ArchiveSystem { get; set; }
        public List<string> Comments { get; set; }
        public string History { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ExtractionDate { get; set; }
        public string IncommingSeparator { get; set; }
        public string OutgoingSeparator { get; set; }


        public ArchiveMetadata()
        {
            Comments = new List<string>();
            ArchiveCreator = new List<MetadataEntityInformationUnit>();
            Owner = new List<MetadataEntityInformationUnit>();
        }

    }


    public class MetadataEntityInformationUnit
    {
        public string Entity { get; set; }
        public string ContactPerson { get; set; }
        public string  Telephone { get; set; }
        public string Email { get; set; }

        public MetadataEntityInformationUnit()
        {
            
        }

        public MetadataEntityInformationUnit(string entity, string contactPerson, string telephone, string email)
        {
            Entity = entity;
            ContactPerson = contactPerson;
            Telephone = telephone;
            Email = email;
        }
    }


    public class MetadataSystemInformationUnit
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Type { get; set; }
        public string TypeVersion { get; set; }

        public MetadataSystemInformationUnit()
        {

        }

        public MetadataSystemInformationUnit(string name, string version, string type, string typeVersion)
        {
            Name = name;
            Version = version;
            Type = type;
            TypeVersion = typeVersion;
        }
    }



}