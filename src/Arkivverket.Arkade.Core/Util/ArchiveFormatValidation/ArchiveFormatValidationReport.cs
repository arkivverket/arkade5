using System;
using System.IO;
using Arkivverket.Arkade.Core.Resources;
using static System.Environment;
using static Arkivverket.Arkade.Core.Util.ArchiveFormatValidation.ArchiveFormatValidationResult;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation
{
    public class ArchiveFormatValidationReport
    {
        public readonly FileSystemInfo ValidatedItem;
        public readonly ArchiveFormat ValidationFormat;
        public readonly ArchiveFormatValidationResult ValidationResult;
        public readonly string ValidationInfo;

        public ArchiveFormatValidationReport(FileSystemInfo validatedItem, ArchiveFormat validationFormat,
            ArchiveFormatValidationResult validationResult, string validationInfo = "")
        {
            ValidationResult = validationResult;
            ValidationFormat = validationFormat;
            ValidatedItem = validatedItem;
            ValidationInfo = validationInfo;
        }

        public string ValidationSummary()
        {
            if (ValidatedItem is DirectoryInfo && ValidationFormat is ArchiveFormat.PdfA)
            {
                return string.Format(
                    ArchiveFormatValidationMessages.DirectoryValidationResultMessage, ValidatedItem.Name, ValidationInfo
                );
            }
            return ValidationResult switch
            {
                Valid => string.Format(
                    ArchiveFormatValidationMessages.ItemConformsWithFormat, ValidatedItem.Name, ValidationInfo
                ),
                Invalid => string.Format(
                    ArchiveFormatValidationMessages.ItemDoesNotConformWithFormat, ValidatedItem.Name, ValidationInfo
                ),
                Error => string.Format(
                    ArchiveFormatValidationMessages.FileFormatValidationErrorMessage, ValidatedItem.Name, ValidationInfo
                ),
                _ => throw new ArgumentOutOfRangeException($"No summary for {ValidationResult}")
            };
        }

        public override string ToString()
        {
            return $@"{NewLine}"
                   + $@" Archive format validation report:{NewLine}{NewLine}"
                   + $@" Validated item: {ValidatedItem.FullName}{NewLine}"
                   + $@" Validation format: {ValidationFormat.GetDescription()}{NewLine}"
                   + $@" Validation result: {ValidationResult}{NewLine}"
                   + $@" Validation info: {ValidationInfo}{NewLine}"
                   + $@" Validation summary: {ValidationSummary()}{NewLine}";
        }
    }
}
