using System;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Metadata
{
    public static class MetsTranslationHelper
    {
        public static bool IsValidSystemType(string systemType)
        {
            return Enum.IsDefined(typeof(ArchiveType), systemType);
        }

        public static bool IsSystemTypeNoark5(string systemType)
        {
            ArchiveType archiveType;

            bool isParsableSystemType = Enum.TryParse(systemType, out archiveType);

            return isParsableSystemType && archiveType == ArchiveType.Noark5;
        }
    }
}
