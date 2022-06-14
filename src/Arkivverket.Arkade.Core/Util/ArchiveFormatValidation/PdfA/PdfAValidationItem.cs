using Arkivverket.Arkade.Core.Resources;
using CsvHelper.Configuration;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    internal class PdfAValidationItem
    {
        public string ItemName { get; set; }
        public string PdfAProfile { get; set; }
        public ArchiveFormatValidationResult ValidationOutcome { get; set; }
    }

    internal sealed class PdfAValidationItemMap : ClassMap<PdfAValidationItem>
    {
        public PdfAValidationItemMap()
        {
            Map(m => m.ItemName).Name(PdfAValidationResultFileContent.HeaderFileName);
            Map(m => m.PdfAProfile).Name(PdfAValidationResultFileContent.HeaderPdfAProfile);
            Map(m => m.ValidationOutcome).Name(PdfAValidationResultFileContent.HeaderValidationResult)
                .TypeConverter<ArchiveFormatValidationResultConverter>();
        }
    }
}
