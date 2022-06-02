using System;
using System.IO;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public class ArchiveFormatValidator : IArchiveFormatValidator
    {
        private readonly PdfAValidator _pdfAValidator;

        public ArchiveFormatValidator()
        {
            _pdfAValidator = new PdfAValidator();
        }

        public async Task<ArchiveFormatValidationResult> ValidateAsync(FileSystemInfo item, ArchiveFormat format)
        {
            return format switch
            {
                ArchiveFormat.PdfA => await _pdfAValidator.ValidateAsync(item),
                ArchiveFormat.DiasSip or ArchiveFormat.DiasAip or ArchiveFormat.DiasAipN5 or ArchiveFormat.DiasSipN5 => await DiasValidator.ValidateAsync(item, format),
                _ => throw new ArgumentOutOfRangeException($"No validator for {format}")
            };
        }

        public void Dispose()
        {
            _pdfAValidator.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
