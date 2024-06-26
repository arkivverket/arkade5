using System;
using System.IO;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public class ArchiveFormatValidator : IArchiveFormatValidator
    {
        private readonly PdfAValidator _pdfAValidator;
        private readonly DiasValidator _diasValidator;

        public ArchiveFormatValidator()
        {
            _pdfAValidator = new PdfAValidator();
            _diasValidator = new DiasValidator();
        }

        public async Task<ArchiveFormatValidationReport> ValidateAsync(FileSystemInfo item, ArchiveFormat format, string resultFileDirectoryPath="")
        {
            return format switch
            {
                ArchiveFormat.PdfA => await _pdfAValidator.ValidateAsync(item, resultFileDirectoryPath),
                ArchiveFormat.DiasSip or ArchiveFormat.DiasAip or 
                    ArchiveFormat.DiasAipN5 or ArchiveFormat.DiasSipN5 or
                    ArchiveFormat.DiasSipSiard or ArchiveFormat.DiasAipSiard 
                    => await _diasValidator.ValidateAsync(item, format),
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
