using System;
using System.IO;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public class ArchiveFormatValidator : IArchiveFormatValidator
    {
        public async Task<ArchiveFormatValidationReport> ValidateAsync(FileSystemInfo item, ArchiveFormat format)
        {
            return format switch
            {
                ArchiveFormat.PdfA => await PdfAValidator.ValidateAsync(item),
                _ => throw new ArgumentOutOfRangeException($"No validator for {format}")
            };
        }
    }
}
