using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Codeuctivity;
using Serilog;
using static Arkivverket.Arkade.Core.Resources.ArchiveFormatValidationMessages;
using static Arkivverket.Arkade.Core.Util.ArchiveFormatValidation.ArchiveFormatValidationResult;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public class PdfAValidator : IDisposable
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

        private readonly Codeuctivity.PdfAValidator _validator;

        private readonly ReadOnlyCollection<string> _approvedPdfAProfiles = new(new List<string>
        {
            "PDF/A-1A",
            "PDF/A-1B",
            "PDF/A-2A",
            "PDF/A-2B",
            "PDF/A-2U",
            //"PDF/A-3A",
            //"PDF/A-3B",
            //"PDF/A-3U",
            //"PDF/A-4",
            //"PDF/A-4E",
            //"PDF/A-4F",
            //"PDF/UA-1",
        });

        public PdfAValidator()
        {
            _validator = new Codeuctivity.PdfAValidator();
        }

        public async Task<ArchiveFormatValidationReport> ValidateAsync(FileSystemInfo item)
        {
            try
            {
                if ((item.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                    throw new Exception("PDF/A validator doesn't support directory input");

                ValidationReport report = (
                    await _validator.ValidateWithDetailedReportAsync(item.FullName)
                ).Jobs.Job.ValidationReport;

                string reportedPdfAProfile = report.ProfileName.Split(' ')[0];

                return report.IsCompliant && _approvedPdfAProfiles.Contains(reportedPdfAProfile)
                    ? new ArchiveFormatValidationReport(item, ArchiveFormat.PdfA, Valid,
                        validationInfo: reportedPdfAProfile)
                    : new ArchiveFormatValidationReport(item, ArchiveFormat.PdfA, Invalid);
            }
            catch (Exception exception)
            {
                Log.Error("Validation failed: " + exception.Message);

                return new ArchiveFormatValidationReport(
                    item, ArchiveFormat.PdfA, Error, false, FileFormatValidationErrorMessage
                );
            }
        }

        public void Dispose()
        {
            _validator.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
