using System.Collections.Generic;
using System.IO;
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

        public static void Write(DiasDirectory diasDirectory, string directoryName)
        {
            WriteEntries(diasDirectory.GetEntries(), Directory.CreateDirectory(directoryName).FullName);
        }

        private static DiasDirectory ProvideSipStructureFagsystem()
        {
            DiasDirectory diasDirectory = GetSipStructureBase();

            diasDirectory.GetSubDirectory(DirectoryNameAdministrativeMetadata).AddEntries(
                new DiasFile(AddmlXmlFileName),
                new DiasFile(AddmlXsdFileName));

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

            diasDirectory.GetSubDirectory(DirectoryNameAdministrativeMetadata).AddEntries(
                new DiasFile(ArkivuttrekkXmlFileName),
                new DiasFile(AddmlXsdFileName));

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

        private static void WriteEntries(IEnumerable<DiasEntry> entries, string path)
        {
            foreach (DiasEntry entry in entries)
            {
                string entryName = Path.Join(path, entry.Name);

                if (entry is DiasFile)
                    File.Create(entryName);

                if (entry is DiasDirectory diasDirectory)
                {
                    DirectoryInfo directory = Directory.CreateDirectory(entryName);
                    WriteEntries(diasDirectory.GetEntries(), directory.FullName);
                }
            }
        }
    }
}
