using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Arkivverket.Arkade.Core.Tests")]
namespace Arkivverket.Arkade.Core.Util.FileFormatIdentification
{
    internal static class ArchiveFileFormatValidator
    {
        public static HashSet<string> GetValidPuids(IEnumerable<ArchiveFileFormat> archiveFileFormats)
        {
            DateOnly dateOnlyNow = DateOnly.FromDateTime(DateTime.Now);
            return archiveFileFormats.Where(aff => IsValid(aff, dateOnlyNow)).SelectMany(aff => aff.Puid).ToHashSet();
        }

        private static bool IsValid(ArchiveFileFormat archiveFileFormat, DateOnly dateOnlyNow)
        {
            DateOnly? validFrom = archiveFileFormat.ValidFrom;
            DateOnly? validTo = archiveFileFormat.ValidTo;

            return !(validFrom > dateOnlyNow) && !(validTo < dateOnlyNow);
        }
    }
}