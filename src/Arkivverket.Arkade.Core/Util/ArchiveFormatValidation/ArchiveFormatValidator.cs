using System;
using System.IO;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public class ArchiveFormatValidator : IArchiveFormatValidator
    {
        public async Task<ArchiveFormatValidationResult> ValidateAsync(FileSystemInfo item, ArchiveFormat format)
        {
            return format switch
            {
                ArchiveFormat.PdfA => await PdfAValidator.ValidateAsync(item),
                ArchiveFormat.DiasSip or ArchiveFormat.DiasAip or ArchiveFormat.DiasAipN5 or ArchiveFormat.DiasSipN5 => await DiasValidator.ValidateAsync(item, format),
                _ => throw new ArgumentOutOfRangeException($"No validator for {format}")
            };
        }
    }
}
