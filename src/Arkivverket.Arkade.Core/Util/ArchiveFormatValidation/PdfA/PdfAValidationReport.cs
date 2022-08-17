using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    internal class PdfAValidationReport
    {
        public long TotalNumberOfFiles { get; set; }
        public long NumberOfValidFiles { get; set; }
        public long NumberOfInvalidFiles { get; set; }
        public long NumberOfUndeterminedFiles { get; set; }
        public List<PdfAValidationItem> ValidationItems { get; } = new();

        public void Merge(PdfAValidationReport otherReport)
        {
            TotalNumberOfFiles += otherReport.TotalNumberOfFiles;
            NumberOfValidFiles += otherReport.NumberOfValidFiles;
            NumberOfInvalidFiles += otherReport.NumberOfInvalidFiles;
            NumberOfUndeterminedFiles += otherReport.NumberOfUndeterminedFiles;
            ValidationItems.AddRange(otherReport.ValidationItems);
        }
    }
}
