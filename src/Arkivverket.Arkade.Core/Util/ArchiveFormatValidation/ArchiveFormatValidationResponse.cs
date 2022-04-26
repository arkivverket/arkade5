using System;
using System.Collections.Generic;
using System.Linq;

namespace Arkivverket.Arkade.Core.Util.ArchiveFormatValidation;

public class ArchiveFormatValidationResponse
{
    public bool IsBatchValidation { get; private set; }

    private readonly ArchiveFormatValidationReport[] _validationReports;

    public ArchiveFormatValidationResponse(ArchiveFormatValidationReport validationReport)
    {
        IsBatchValidation = false;

        _validationReports = new[] { validationReport };
    }

    public ArchiveFormatValidationResponse(ArchiveFormatValidationReport[] validationReports)
    {
        IsBatchValidation = true;

        _validationReports = validationReports;
    }

    public ArchiveFormatValidationResult GetOverallResult()
    {
        if (_validationReports.Any(r => r.ValidationResult == ArchiveFormatValidationResult.Invalid))
            return ArchiveFormatValidationResult.Invalid;

        if (_validationReports.Any(r => r.ValidationResult == ArchiveFormatValidationResult.Error))
            return ArchiveFormatValidationResult.Error;

        return ArchiveFormatValidationResult.Valid;
    }

    public ArchiveFormatValidationReport GetReport()
    {
        if (IsBatchValidation)
            throw new Exception("Method GetReport should not be used with batch validation." +
                                " Call method GetReports instead.");

        return _validationReports.First();
    }

    public IEnumerable<ArchiveFormatValidationReport> GetReports()
    {
        return _validationReports;
    }
}
