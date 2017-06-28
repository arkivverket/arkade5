using System;

namespace Arkivverket.Arkade.Metadata
{
    public static class MetsTranslationHelper
    {
        public static bool IsValidSystemType(string systemType)
        {
            // TODO: Use Enum ExternalModels.Mets.type (not ExternalModels.Info.type) when/if supported in built in mets schema

            return Enum.IsDefined(typeof(ExternalModels.Info.type), systemType);
        }

        public static bool IsSystemTypeNoark5(string systemType)
        {
            // TODO: Use Enum ExternalModels.Mets.type (not ExternalModels.Info.type) when/if supported in built in mets schema

            ExternalModels.Info.type enumSystemType;

            bool isParsableSystemType = Enum.TryParse(systemType, out enumSystemType);

            return isParsableSystemType && enumSystemType == ExternalModels.Info.type.Noark5;
        }
    }
}
