using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core.Resources;
using Codeuctivity;
using Serilog;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public static partial class ArchiveFormatValidator
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

        public static async Task<ArchiveFormatValidationResponse> ValidateAsFormat(FileSystemInfo item,
            ArchiveFormat format, bool isBatchValidation = false)
        {
            return format switch
            {
                ArchiveFormat.PdfA => await ValidateAsPdfA(item, isBatchValidation),
                ArchiveFormat.DiasSip or ArchiveFormat.DiasAip or ArchiveFormat.DiasAipN5 or ArchiveFormat.DiasSipN5 =>
                    new ArchiveFormatValidationResponse(await ValidateAsDiasAsync(item, format)),
                _ => throw new ArgumentOutOfRangeException($"No validator for {format}")
            };
        }

        public static async Task<ArchiveFormatValidationResponse> ValidateAsPdfA(FileSystemInfo item, bool isBatchValidation = false)
        {
            var approvedPdfAProfiles = new ReadOnlyCollection<string>(
                new List<string>
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
                }
            );

            try
            {
                if ((item.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                    throw new Exception("PDF/A validator doesn't support directory input"); // TODO: So ...

                ValidationReport report = (
                    await new PdfAValidator().ValidateWithDetailedReportAsync(item.FullName)
                ).Jobs.Job.ValidationReport;

                string reportedPdfAProfile = report.ProfileName.Split(' ')[0];

                ArchiveFormatValidationReport validationReport = report.IsCompliant && approvedPdfAProfiles.Contains(reportedPdfAProfile)
                        ? new ArchiveFormatValidationReport(item, ArchiveFormat.PdfA, ArchiveFormatValidationResult.Valid, reportedPdfAProfile)
                        : new ArchiveFormatValidationReport(item, ArchiveFormat.PdfA, ArchiveFormatValidationResult.Invalid);

                return new ArchiveFormatValidationResponse(validationReport);
            }
            catch (Exception exception)
            {
                Log.Error("Validation failed: " + exception.Message);

                return new ArchiveFormatValidationResponse(
                    new ArchiveFormatValidationReport(
                        item,
                        ArchiveFormat.PdfA,
                        ArchiveFormatValidationResult.Error,
                        ArchiveFormatValidationMessages.FileFormatValidationErrorMessage
                    )
                );
            }
        }
    }
}
