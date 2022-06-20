using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core.Logging;
using Arkivverket.Arkade.Core.Resources;
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
        private IStatusEventHandler _statusEventHandler;

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

        private string _baseDirectoryPath;

        public PdfAValidator()
        {
            _validator = new Codeuctivity.PdfAValidator();
        }

        public PdfAValidator(IStatusEventHandler statusEventHandler)
        {
            _validator = new Codeuctivity.PdfAValidator();
            _statusEventHandler = statusEventHandler;
        }

        public async Task<ArchiveFormatValidationReport> ValidateAsync(FileSystemInfo item, string resultFileDirectoryPath)
        {
            try
            {
                if (item is FileInfo fileItem)
                    return await ValidateSingleFileAsync(fileItem);

                return await ValidateDirectoryContentAsync(item as DirectoryInfo, resultFileDirectoryPath);
                //return await ValidateDirectoryContentAlt2Async(item as DirectoryInfo, resultFileDirectoryPath);
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
            DirectoryInfo directory, string resultFileDirectoryPath)
        {
            _baseDirectoryPath = directory.FullName;

            PdfAValidationReport report = await CreatePdfAValidationReportAsync(directory);

            string resultFileFullName = Path.Combine(resultFileDirectoryPath, OutputFileNames.PdfAValidationResultFile);

            CsvHelper.WriteToFile<PdfAValidationItem, PdfAValidationItemMap>(resultFileFullName, report.ValidationItems);

            return new ArchiveFormatValidationReport(
                directory, ArchiveFormat.PdfA, report.NumberOfInvalidFiles > 0 ? Invalid : Valid,
                validationInfo: string.Format(PdfABatchValidationInfoMessage, report.TotalNumberOfFiles, 
                    report.NumberOfValidFiles, report.NumberOfInvalidFiles, report.NumberOfUndeterminedFiles,
                    resultFileFullName)
            );
        }

        private async Task<PdfAValidationReport> CreatePdfAValidationReportAsync(
            DirectoryInfo directory)
        {
            // The BatchSummary object available from the result of the ValidateBatchWithDetailedReportAsync method
            // below does have information about total number of jobs, valid items and invalid items. However, this
            // information only keep track of files ending with .pdf. All other files are ignored. Hence, custom
            // counters have been implemented. Additionally, this validator is more strict than veraPDF in which
            // PDF/A-profiles it accepts - see field _approvedPdfAProfiles.
            IEnumerable<Job> validationJobs = (await _validator.ValidateBatchWithDetailedReportAsync(
                new[] { directory.FullName }, "")).Jobs.AllJobs.AsEnumerable();

            var partialReport = new PdfAValidationReport();
            
            foreach (FileSystemInfo fileSystemInfo in directory.EnumerateFileSystemInfos())
            {
                if (fileSystemInfo is DirectoryInfo directoryInfo)
                {
                    partialReport.Merge(await CreatePdfAValidationReportAsync(directoryInfo));
                    continue;
                }

                partialReport.TotalNumberOfFiles++;

                string itemName = Path.GetRelativePath(_baseDirectoryPath, fileSystemInfo.FullName);

                Job job = validationJobs.FirstOrDefault(j => j.Item.Name.Equals(fileSystemInfo.FullName));

                if (job == default(Job))
                {
                    partialReport.NumberOfUndeterminedFiles++;
                    partialReport.ValidationItems.Add(new PdfAValidationItem
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

                if (itemIsValid) partialReport.NumberOfValidFiles++;
                else partialReport.NumberOfInvalidFiles++;

                partialReport.ValidationItems.Add(new PdfAValidationItem
                {
                    ItemName = itemName,
                    PdfAProfile = reportedPdfAProfile,
                    ValidationOutcome = itemIsValid ? Valid : Invalid
                });
            }

            return partialReport;
        }

        private async Task<ArchiveFormatValidationReport> ValidateDirectoryContentAlt2Async(
            DirectoryInfo directory, string resultFileDirectoryPath)
        {
            _baseDirectoryPath = directory.FullName;

            _statusEventHandler.RaiseEventFormatValidationStarted(directory.GetNumberOfFileInfoObjects());

            PdfAValidationReport report = await CreatePdfAValidationReportAsyncAlt2(directory);

            string resultFileFullName = Path.Combine(resultFileDirectoryPath, OutputFileNames.PdfAValidationResultFile);

            CsvHelper.WriteToFile<PdfAValidationItem, PdfAValidationItemMap>(resultFileFullName, report.ValidationItems);

            _statusEventHandler.RaiseEventFormatValidationFinished();

            return new ArchiveFormatValidationReport(
                directory, ArchiveFormat.PdfA, report.NumberOfInvalidFiles > 0 ? Invalid : Valid,
                validationInfo: string.Format(PdfABatchValidationInfoMessage, report.TotalNumberOfFiles,
                    report.NumberOfValidFiles, report.NumberOfInvalidFiles, report.NumberOfUndeterminedFiles,
                    resultFileFullName)
            );
        }

        private async Task<PdfAValidationReport> CreatePdfAValidationReportAsyncAlt2(
            DirectoryInfo directory)
        {
            var partialReport = new PdfAValidationReport();

            var validationTasks = CreateBatchValidationTasksAsync(directory);

            foreach (PdfAValidationItem item in await validationTasks)
            {
                partialReport.TotalNumberOfFiles++;
                switch (item.ValidationOutcome)
                {
                    case Valid:
                        partialReport.NumberOfValidFiles++;
                        break;
                    case Invalid:
                        partialReport.NumberOfInvalidFiles++;
                        break;
                    case Error:
                        partialReport.NumberOfUndeterminedFiles++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                partialReport.ValidationItems.Add(item);
            }

            return partialReport;
        }

        private async Task<IEnumerable<PdfAValidationItem>> CreateBatchValidationTasksAsync(DirectoryInfo directory)
        {
            var tasks = new List<PdfAValidationItem>();

            await Task.Run(() =>
            {
                Parallel.ForEach(directory.GetFileSystemInfos(), fileSystemInfo =>
                {
                    if (fileSystemInfo is DirectoryInfo directoryInfo)
                    {
                        tasks.AddRange(CreateBatchValidationTasksAsync(directoryInfo).Result);
                        return;
                    }

                    string itemName = Path.GetRelativePath(_baseDirectoryPath, fileSystemInfo.FullName);

                    try
                    {
                        var job = _validator.ValidateWithDetailedReportAsync(fileSystemInfo.FullName).Result.Jobs.Job;

                        ValidationReport validationReport = job.ValidationReport;
                        string reportedPdfAProfile = validationReport.ProfileName.Split(' ')[0];

                        bool itemIsValid = validationReport.IsCompliant &&
                                           _approvedPdfAProfiles.Contains(reportedPdfAProfile);

                        tasks.Add(new PdfAValidationItem
                        {
                            ItemName = itemName,
                            PdfAProfile = reportedPdfAProfile,
                            ValidationOutcome = itemIsValid ? Valid : Invalid
                        });
                    }
                    catch
                    {
                        tasks.Add(new PdfAValidationItem
                        {
                            ItemName = itemName,
                            PdfAProfile = "N/A",
                            ValidationOutcome = Error,
                        });
                    }

                    _statusEventHandler.RaiseEventFormatValidationProgressUpdated();
                });
            });


            return tasks;
        }

        public void Dispose()
        {
            _validator.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
