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

        public async Task<ArchiveFormatValidationReport> ValidateAsync(FileSystemInfo item, ArchiveFormat format, string resultFileDirectoryPath="")
        {
            return format switch
            {
                ArchiveFormat.PdfA => await _pdfAValidator.ValidateAsync(item, resultFileDirectoryPath),
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
