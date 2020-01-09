using System.Text.RegularExpressions;
using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Metadata
{
    public static class MetadataCleaner
    {
        public static void Clean(ArchiveMetadata metadata)
        {
            if (metadata.Id != null)
                metadata.Id = Clean(metadata.Id);

            if (metadata.Label != null)
                metadata.Label = Clean(metadata.Label);

            if (metadata.ArchiveDescription != null)
                metadata.ArchiveDescription = Clean(metadata.ArchiveDescription);

            if (metadata.AgreementNumber != null)
                metadata.AgreementNumber = Clean(metadata.AgreementNumber);

            if (metadata.Recipient != null)
                metadata.Recipient = Clean(metadata.Recipient);

            metadata.ArchiveCreators?.ForEach(Clean);

            metadata.Owners?.ForEach(Clean);

            if (metadata.Transferer != null)
                Clean(metadata.Transferer);

            if (metadata.Producer != null)
                Clean(metadata.Producer);

            if (metadata.Creator != null)
                Clean(metadata.Creator);

            if (metadata.System != null)
                Clean(metadata.System);

            if (metadata.ArchiveSystem != null)
                Clean(metadata.ArchiveSystem);
        }

        private static void Clean(MetadataEntityInformationUnit entityInfoUnit)
        {
            if (entityInfoUnit.Address != null)
                entityInfoUnit.Address = Clean(entityInfoUnit.Address);

            if (entityInfoUnit.ContactPerson != null)
                entityInfoUnit.ContactPerson = Clean(entityInfoUnit.ContactPerson);

            if (entityInfoUnit.Email != null)
                entityInfoUnit.Email = Clean(entityInfoUnit.Email);

            if (entityInfoUnit.Entity != null)
                entityInfoUnit.Entity = Clean(entityInfoUnit.Entity);

            if (entityInfoUnit.Telephone != null)
                entityInfoUnit.Telephone = Clean(entityInfoUnit.Telephone);
        }

        private static void Clean(MetadataSystemInformationUnit systemInfoUnit)
        {
            if (systemInfoUnit.Name != null)
                systemInfoUnit.Name = Clean(systemInfoUnit.Name);

            if (systemInfoUnit.Type != null)
                systemInfoUnit.Type = Clean(systemInfoUnit.Type);

            if (systemInfoUnit.Version != null)
                systemInfoUnit.Version = Clean(systemInfoUnit.Version);

            if (systemInfoUnit.TypeVersion != null)
                systemInfoUnit.TypeVersion = Clean(systemInfoUnit.TypeVersion);
        }

        private static string Clean(string text)
        {
            return Regex.Replace(text, @"\t|\n|\r", "").Trim();
        }
    }
}
