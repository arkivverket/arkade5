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
    public static class ArchiveFormatValidator
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

        public static async Task<ArchiveFormatValidationReport> ValidateAsFormat(FileSystemInfo item,
            ArchiveFormat format)
        {
            return format switch
            {
                ArchiveFormat.PdfA => await ValidateAsPdfA(item),
                _ => throw new ArgumentOutOfRangeException($"No validator for {format}")
            };
        }

        public static async Task<ArchiveFormatValidationReport> ValidateAsPdfA(FileSystemInfo item)
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
                    throw new Exception("PDF/A validator doesn't support directory input");

                ValidationReport report = (
                    await new PdfAValidator().ValidateWithDetailedReportAsync(item.FullName)
                ).Jobs.Job.ValidationReport;

                string reportedPdfAProfile = report.ProfileName.Split(' ')[0];

                return report.IsCompliant && approvedPdfAProfiles.Contains(reportedPdfAProfile)
                    ? new ArchiveFormatValidationReport(
                        item, ArchiveFormat.PdfA, ArchiveFormatValidationResult.Valid, reportedPdfAProfile
                        )
                    : new ArchiveFormatValidationReport(item, ArchiveFormat.PdfA, ArchiveFormatValidationResult.Invalid);
            }
            catch (Exception exception)
            {
                Log.Error("Validation failed: " + exception.Message);

                return new ArchiveFormatValidationReport(
                    item,
                    ArchiveFormat.PdfA,
                    ArchiveFormatValidationResult.Error,
                    ArchiveFormatValidationMessages.FileFormatValidationErrorMessage
                );
            }
        }
    }
}
