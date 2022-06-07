using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core.Resources;
using Codeuctivity;
using CsvHelper.Configuration;
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
                if (item is FileInfo fileItem)
                    return await ValidateSingleFileAsync(fileItem);

                return await ValidateDirectoryContentAsync(item as DirectoryInfo);
            }
            catch (Exception exception)
            {
                Log.Error("Validation failed: " + exception.Message);

                return new ArchiveFormatValidationReport(
                    item, ArchiveFormat.PdfA, Error, false, FileFormatValidationErrorMessage
                );
            }
        }

        private async Task<ArchiveFormatValidationReport> ValidateSingleFileAsync(FileInfo item)
        {
            ValidationReport report = (
                await _validator.ValidateWithDetailedReportAsync(item.FullName)
            ).Jobs.Job.ValidationReport;

            string reportedPdfAProfile = report.ProfileName.Split(' ')[0];

            return report.IsCompliant && _approvedPdfAProfiles.Contains(reportedPdfAProfile)
                ? new ArchiveFormatValidationReport(item, ArchiveFormat.PdfA, Valid, validationInfo: reportedPdfAProfile)
                : new ArchiveFormatValidationReport(item, ArchiveFormat.PdfA, Invalid);
        }

        private async Task<ArchiveFormatValidationReport> ValidateDirectoryContentAsync(
            DirectoryInfo directory)
        {
            // The BatchSummary object available from the result of the ValidateBatchWithDetailedReportAsync method
            // below does have information about total number of jobs, valid items and invalid items. However, this
            // information only keep track of files ending with .pdf. All other files are ignored. Hence, custom
            // counters have been implemented. Additionally, this validator is more strict than veraPDF in which
            // PDF/A-profiles it accepts - see field _approvedPdfAProfiles.
            IEnumerable<Job> validationJobs = (await _validator.ValidateBatchWithDetailedReportAsync(
                new[] { directory.FullName }, "")).Jobs.AllJobs.AsEnumerable();

            var validationItems = new List<PdfAValidationItem>();

            ArchiveFormatValidationResult overallResult = Valid;

            FileInfo[] filesInDirectory = directory.GetFiles();
            var totalNumberOfFiles = 0L;
            var numberOfValidFiles = 0L;
            var numberOfInvalidFiles = 0L;
            var numberOfUndeterminedFiles = 0L;

            foreach (FileInfo file in filesInDirectory)
            {
                totalNumberOfFiles++;
                
                Job job = validationJobs.FirstOrDefault(j => j.Item.Name.Equals(file.FullName));

                string itemName = file.Name;

                if (job == default(Job))
                {
                    numberOfUndeterminedFiles++;
                    validationItems.Add(new PdfAValidationItem
                    {
                        ItemName = itemName,
                        PdfAProfile = "N/A",
                        ValidationOutcome = Error
                    });
                    continue;
                }

                ValidationReport validationReport = job.ValidationReport;
                string reportedPdfAProfile = validationReport.ProfileName.Split(' ')[0];

                bool itemIsValid = validationReport.IsCompliant && _approvedPdfAProfiles.Contains(reportedPdfAProfile);

                if (itemIsValid) numberOfValidFiles++;
                else numberOfInvalidFiles++;

                validationItems.Add(new PdfAValidationItem
                {
                    ItemName = itemName,
                    PdfAProfile = reportedPdfAProfile,
                    ValidationOutcome = itemIsValid ? Valid : Invalid
                });
            }

            string resultFileFullName = Path.Combine(directory.Parent?.FullName ?? directory.FullName,
                OutputFileNames.PdfAValidationResultFile);

            CsvHelper.WriteToFile<PdfAValidationItem, PdfAValidationItemMap>(resultFileFullName, validationItems);

            return new ArchiveFormatValidationReport(
                directory, ArchiveFormat.PdfA, overallResult,
                validationInfo: string.Format(PdfABatchValidationInfoMessage, totalNumberOfFiles, numberOfValidFiles,
                    numberOfInvalidFiles, numberOfUndeterminedFiles, resultFileFullName)
            );
        }

        public void Dispose()
        {
            _validator.Dispose();
            GC.SuppressFinalize(this);
        }

        private sealed class PdfAValidationItemMap : ClassMap<PdfAValidationItem>
        {
            public PdfAValidationItemMap()
            {
                Map(m => m.ItemName).Name(PdfAValidationResultFileContent.HeaderFileName);
                Map(m => m.PdfAProfile).Name(PdfAValidationResultFileContent.HeaderPdfAProfile);
                Map(m => m.ValidationOutcome).Name(PdfAValidationResultFileContent.HeaderValidationResult)
                    .TypeConverter<ArchiveFormatValidationResultConverter>();
            }
        }

        private class PdfAValidationItem
        {
            public string ItemName { get; set; }
            public string PdfAProfile { get; set; }
            public ArchiveFormatValidationResult ValidationOutcome { get; set; }
        }
    }
}
