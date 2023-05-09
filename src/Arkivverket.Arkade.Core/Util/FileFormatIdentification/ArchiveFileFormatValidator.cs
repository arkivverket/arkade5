using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static Arkivverket.Arkade.Core.Resources.FormatAnalysisResultFileContent;

[assembly: InternalsVisibleTo("Arkivverket.Arkade.Core.Tests")]
namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    internal static class ArchiveFileFormatValidator
    {
        private static readonly DateOnly DateOnlyNow;
        private static readonly Dictionary<string, bool> ValidPuidsWithAdditionalRequirementsIndicator;

        static ArchiveFileFormatValidator()
        {
            DateOnlyNow = DateOnly.FromDateTime(DateTime.Now);
            ValidPuidsWithAdditionalRequirementsIndicator = new Dictionary<string, bool>();
        }

        public static void Initialize(IEnumerable<ArchiveFileFormat> archiveFileFormats)
        {
            GetValidPuidsWithAdditionalRequirementIndicator(archiveFileFormats);
        }

        public static string Validate(string puid)
        {
            if (!ValidPuidsWithAdditionalRequirementsIndicator.ContainsKey(puid))
                return FormatIsNotValidValue;

            return ValidPuidsWithAdditionalRequirementsIndicator[puid]
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
                    ValidPuidsWithAdditionalRequirementsIndicator.TryAdd(puid, hasAdditionalRequirements);
                }
            }
        }

        private static bool IsValid(ArchiveFileFormat archiveFileFormat)
        {
            DateOnly? validFrom = archiveFileFormat.ValidFrom;
            DateOnly? validTo = archiveFileFormat.ValidTo;

            return !(validFrom > DateOnlyNow) && !(validTo < DateOnlyNow);
        }
    }
}