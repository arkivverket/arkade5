using Arkivverket.Arkade.Core.Base;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public static class DiasProvider
    {
        public static DiasDirectory ProvideForFormat(ArchiveFormat format)
        {
            return format switch
            {
                ArchiveFormat.DiasSip => ProvideSipStructureFagsystem(),
                ArchiveFormat.DiasAip => ProvideAipStructureFagsystem(),
                ArchiveFormat.DiasSipN5 => ProvideSipStructureNoark5(),
                ArchiveFormat.DiasAipN5 => ProvideAipStructureNoark5(),
                _ => throw new ArkadeException($"Archive format type {format} is not implemented."),
            };
        }

        private static DiasDirectory ProvideSipStructureFagsystem()
        {
            DiasDirectory diasDirectory = GetSipStructureBase();

            return diasDirectory;
        }

        private static DiasDirectory ProvideAipStructureFagsystem()
        {
            DiasDirectory diasDirectory = GetAipStructureBase();

            diasDirectory.GetSubDirectory(DirectoryNameAdministrativeMetadata).AddEntries(
                new DiasFile(AddmlXmlFileName),
                new DiasFile(AddmlXsdFileName));

            return diasDirectory;
        }

        private static DiasDirectory ProvideSipStructureNoark5()
        {
            DiasDirectory diasDirectory = GetSipStructureBase();

            diasDirectory.GetSubDirectory(DirectoryNameContent).AddEntries(
                new DiasFile(ArkivstrukturXmlFileName),
                new DiasFile(ArkivstrukturXsdFileName),
                new DiasFile(ChangeLogXmlFileName),
                new DiasFile(ChangeLogXsdFileName),
                new DiasFile(RunningJournalXmlFileName),
                new DiasFile(RunningJournalXsdFileName),
                new DiasFile(PublicJournalXmlFileName),
                new DiasFile(PublicJournalXsdFileName),
                new DiasFile(MetadatakatalogXsdFileName));

            return diasDirectory;
        }

        private static DiasDirectory ProvideAipStructureNoark5()
        {
            DiasDirectory diasDirectory = GetAipStructureBase();

            diasDirectory.Merge(ProvideSipStructureNoark5());

            diasDirectory.GetSubDirectory(DirectoryNameAdministrativeMetadata).AddEntries(
                new DiasFile(ArkivuttrekkXmlFileName),
                new DiasFile(AddmlXsdFileName));

            return diasDirectory;
        }

        private static DiasDirectory GetSipStructureBase()
        {
            return new DiasDirectory("SipStructureBase",
                new DiasFile(DiasMetsXmlFileName),
                new DiasFile(DiasMetsXsdFileName),
                new DiasDirectory(DirectoryNameDescriptiveMetadata),
                new DiasDirectory(DirectoryNameAdministrativeMetadata,
                    new DiasFile(DiasPremisXmlFileName),
                    new DiasFile(DiasPremisXsdFileName)),
                new DiasDirectory(DirectoryNameContent));
        }

        private static DiasDirectory GetAipStructureBase()
        {
            DiasEntry[] sipStructureBaseEntries = GetSipStructureBase().GetEntries();

            var diasDirectory = new DiasDirectory("AipStructureBase", sipStructureBaseEntries);

            diasDirectory.GetSubDirectory(DirectoryNameDescriptiveMetadata).AddEntries(
                new DiasFile(EadXmlFileName),
                new DiasFile(EadXsdFileName),
                new DiasFile(EacCpfXmlFileName),
                new DiasFile(EacCpfXsdFileName));

            diasDirectory.GetSubDirectory(DirectoryNameAdministrativeMetadata).AddEntry(
                new DiasDirectory(DirectoryNameRepositoryOperations));

            diasDirectory.GetSubDirectory(DirectoryNameContent).AddEntry(
                new DiasFile(SystemhaandbokPdfFileName));

            return diasDirectory;
        }
    }
}
