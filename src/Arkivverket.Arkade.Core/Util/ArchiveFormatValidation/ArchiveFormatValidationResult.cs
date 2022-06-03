using System;
using Arkivverket.Arkade.Core.Resources;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using static Arkivverket.Arkade.Core.Util.ArchiveFormatValidation.ArchiveFormatValidationResult;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public enum ArchiveFormatValidationResult
    {
        Valid,
        Invalid,
        Error
    }

    public class ArchiveFormatValidationResultConverter : DefaultTypeConverter
    {
        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            var val = (ArchiveFormatValidationResult)value;

            return (val) switch
            {
                Valid => PdfAValidationResultFileContent.Validity_Valid,
                Invalid => PdfAValidationResultFileContent.Validity_Invalid,
                Error => PdfAValidationResultFileContent.Validity_Error,
                _ => throw new InvalidCastException(value + "could not be converted to enum ArchiveFormatValidationResult")
            };
        }
    }
}
