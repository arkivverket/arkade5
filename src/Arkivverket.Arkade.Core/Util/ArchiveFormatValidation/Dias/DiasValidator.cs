using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Tar;
using static System.Environment;
using static Arkivverket.Arkade.Core.Resources.ArchiveFormatValidationMessages;
using static Arkivverket.Arkade.Core.Util.ArkadeConstants;
using static Arkivverket.Arkade.Core.Util.ArchiveFormatValidation.ArchiveFormatValidationResultType;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public class DiasValidator
    {
        public async Task<ArchiveFormatValidationReport> ValidateAsync(FileSystemInfo item, ArchiveFormat format)
        {
            List<string> missingEntries = await GetMissingEntriesAsync(item, format);

            ArchiveFormatValidationResultType result;
            bool resultIsAcceptable = true;

            if (missingEntries == null || !missingEntries.Any())
                result = Valid;
            else
            {
                result = Invalid;
                resultIsAcceptable = DetermineAcceptability(missingEntries, format);
            }

            missingEntries = new List<string>(
                missingEntries.Select(e => e.Replace(
                    Path.GetFileNameWithoutExtension(item.Name) + Path.DirectorySeparatorChar, string.Empty
                ))); // Excluding DIAS root directory name from entry paths

            string validationInfo = CreateValidationInfoString(result, resultIsAcceptable, missingEntries);

            return new ArchiveFormatValidationReport(item, format, result, resultIsAcceptable, validationInfo);
        }

        private async Task<List<string>> GetMissingEntriesAsync(FileSystemInfo item, ArchiveFormat format)
        {
            DiasDirectory dias = DiasProvider.ProvideForFormat(format);

            return item is FileInfo { Extension: ".tar" } tarArchive
                ? await Task.Run(() => GetEntryPathsNotInTarArchiveAsync(tarArchive, dias))
                : await Task.Run(() => dias.GetEntryPaths(item.FullName, getNonExistingOnly: true, recursive: true));
        }

        private async Task<List<string>> GetEntryPathsNotInTarArchiveAsync(FileInfo tarArchive, DiasDirectory validDias)
        {
            string tarArchiveRootDirectoryName = Path.GetFileNameWithoutExtension(tarArchive.Name);
            List<string> diasEntryPaths = validDias.GetEntryPaths(tarArchiveRootDirectoryName, recursive: true);

            await using var tarInputStream = new TarInputStream(File.OpenRead(tarArchive.FullName), Encoding.Latin1);
            var tarEntryPaths = new HashSet<string>();
            await Task.Run(() =>
            {
                while (tarInputStream.GetNextEntry() is { } entry)
                    tarEntryPaths.Add(Path.TrimEndingDirectorySeparator(entry.Name.ForwardSlashed()));
            });

            return diasEntryPaths.Where(diasEntryPath => !tarEntryPaths.Contains(Path.TrimEndingDirectorySeparator(diasEntryPath.ForwardSlashed()))).ToList();
        }

        private static bool DetermineAcceptability(List<string> missingEntries, ArchiveFormat format)
        {
            if (format is ArchiveFormat.DiasSip)
                return false;

            if (format is ArchiveFormat.DiasAip)
                return missingEntries.All(e =>
                    e.Contains(EadXmlFileName) || e.Contains(EadXsdFileName) ||
                    e.Contains(EacCpfXmlFileName) || e.Contains(EacCpfXsdFileName) ||
                    e.Contains(SystemhaandbokPdfFileName));

            var isMissingChangeLogXml = false;
            var isMissingChangeLogXsd = false;
            var isMissingRunningJournalXml = false;
            var isMissingRunningJournalXsd = false;
            var isMissingPublicJournalXml = false;
            var isMissingPublicJournalXsd = false;

            foreach (string missingEntry in missingEntries)
            {
                // format will always be either DiasSipN5 or DiasAipN5
                if (!isMissingChangeLogXml && missingEntry.Contains(ChangeLogXmlFileName))
                    isMissingChangeLogXml = true;
                else if (!isMissingChangeLogXsd && missingEntry.Contains(ChangeLogXsdFileName))
                    isMissingChangeLogXsd = true;
                else if (!isMissingRunningJournalXml && missingEntry.Contains(RunningJournalXmlFileName))
                    isMissingRunningJournalXml = true;
                else if (!isMissingRunningJournalXsd && missingEntry.Contains(RunningJournalXsdFileName))
                    isMissingRunningJournalXsd = true;
                else if (!isMissingPublicJournalXml && missingEntry.Contains(PublicJournalXmlFileName))
                    isMissingPublicJournalXml = true;
                else if (!isMissingPublicJournalXsd && missingEntry.Contains(PublicJournalXsdFileName))
                    isMissingPublicJournalXsd = true;
                else if (missingEntry.Contains(EadXmlFileName) || missingEntry.Contains(EadXsdFileName) ||
                         missingEntry.Contains(EacCpfXmlFileName) || missingEntry.Contains(EacCpfXsdFileName) ||
                         missingEntry.Contains(SystemhaandbokPdfFileName))
                    // ReSharper disable once RedundantJumpStatement
                    continue;
                else
                    return false;
            }

            if (!isMissingChangeLogXml && isMissingChangeLogXsd)
                return false;
            if (!isMissingRunningJournalXml && isMissingRunningJournalXsd)
                return false;
            if (!isMissingPublicJournalXml && isMissingPublicJournalXsd)
                return false;
            if (isMissingChangeLogXml || isMissingRunningJournalXml || isMissingPublicJournalXml)
                return true;

            return false;
        }

        private static string CreateValidationInfoString(
            ArchiveFormatValidationResultType resultType,
            bool resultIsAcceptable,
            IEnumerable<string> missingEntries)
        {
            return resultType switch
            {
                Valid => NewLine + MandatoryDiasEntriesWereFound,
                Invalid when resultIsAcceptable => string.Format(DiasAcceptableWithMissingEntries, "\t" + string.Join(NewLine + "\t", missingEntries)),
                Invalid => string.Format(MissingDiasEntries, "\t" + string.Join(NewLine, missingEntries)),
                _ => throw new NotImplementedException()
            };
        }
    }
}
