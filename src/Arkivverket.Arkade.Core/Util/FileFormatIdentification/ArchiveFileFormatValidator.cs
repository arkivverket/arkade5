using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static Arkivverket.Arkade.Core.Resources.FormatAnalysisResultFileContent;

[assembly: InternalsVisibleTo("Arkivverket.Arkade.Core.Tests")]
namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    internal static class ArchiveFileFormatValidator
    {
        private static readonly DateTime DateTimeNow;
        private static Dictionary<string, bool> _validPuidsWithAdditionalRequirementsIndicator;

        static ArchiveFileFormatValidator()
        {
            DateTimeNow = DateTime.Now.Date;
        }

        public static void Initialize(IEnumerable<ArchiveFileFormat> archiveFileFormats)
        {
            _validPuidsWithAdditionalRequirementsIndicator = new Dictionary<string, bool>();
            GetValidPuidsWithAdditionalRequirementIndicator(archiveFileFormats);
        }

        public static string Validate(string puid)
        {
            if (!_validPuidsWithAdditionalRequirementsIndicator.ContainsKey(puid))
                return FormatIsNotValidValue;

            return _validPuidsWithAdditionalRequirementsIndicator[puid]
                ? FormatIsValidWithAdditionalRequirementsValue
                : FormatIsValidValue;
        }

        private static void GetValidPuidsWithAdditionalRequirementIndicator(
            IEnumerable<ArchiveFileFormat> archiveFileFormats)
        {
            foreach (ArchiveFileFormat archiveFileFormat in archiveFileFormats)
            {
                if (!IsValid(archiveFileFormat))
                    continue;

                bool hasAdditionalRequirements = !string.IsNullOrEmpty(archiveFileFormat.AdditionalRequirements);
                
                foreach (string puid in archiveFileFormat.Puid)
                {
                    _validPuidsWithAdditionalRequirementsIndicator.TryAdd(puid, hasAdditionalRequirements);
                }
            }
        }

        private static bool IsValid(ArchiveFileFormat archiveFileFormat)
        {
            DateTime? validFrom = archiveFileFormat.ValidFrom;
            DateTime? validTo = archiveFileFormat.ValidTo;

            return !(validFrom > DateTimeNow) && !(validTo < DateTimeNow);
        }
    }
}