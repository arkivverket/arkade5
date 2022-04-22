using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Tar;
using static System.Environment;
using static Arkivverket.Arkade.Core.Resources.ArchiveFormatValidationMessages;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public static partial class ArchiveFormatValidator
    {
        private static async Task<ArchiveFormatValidationReport> ValidateAsDiasAsync(FileSystemInfo item, ArchiveFormat format)
        {
            List<string> missingEntries = await GetMissingEntries(item, format);

            ArchiveFormatValidationResult result = missingEntries.Any()
                ? ArchiveFormatValidationResult.Invalid
                : ArchiveFormatValidationResult.Valid;

            missingEntries = new List<string>(
                missingEntries.Select(e => e.Replace(
                    Path.GetFileNameWithoutExtension(item.Name) + Path.DirectorySeparatorChar, string.Empty
                ))); // Excluding DIAS root directory name from entry paths
            
            string validationInfo = NewLine + string.Format(MissingDiasEntries, NewLine + string.Join(NewLine, missingEntries));

            return new ArchiveFormatValidationReport(item, format, result, validationInfo);
        }

        private static async Task<List<string>> GetMissingEntries(FileSystemInfo item, ArchiveFormat format)
        {
            DiasDirectory dias = DiasProvider.ProvideForFormat(format);

            return item is FileInfo { Extension: ".tar" } tarArchive
                ? await Task.Run(() => GetEntryPathsNotInTarArchive(tarArchive, dias))
                : await Task.Run(() => dias.GetEntryPaths(item.FullName, getNonExistingOnly: true, recursive: true));
        }

        private static List<string> GetEntryPathsNotInTarArchive(FileInfo tarArchive, DiasDirectory validDias)
        {
            string tarArchiveRootDirectoryName = Path.GetFileNameWithoutExtension(tarArchive.Name);
            List<string> diasEntryPaths = validDias.GetEntryPaths(tarArchiveRootDirectoryName, recursive: true);

            using var tarInputStream = new TarInputStream(File.OpenRead(tarArchive.FullName), Encoding.Latin1);
            var tarEntryPaths = new HashSet<string>();
            while (tarInputStream.GetNextEntry() is { } entry)
                tarEntryPaths.Add(entry.Name.ForwardSlashed());

            return diasEntryPaths.Where(diasEntryPath => !tarEntryPaths.Contains(diasEntryPath.ForwardSlashed())).ToList();
        }
    }
}
