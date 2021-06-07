using System.Globalization;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base
{
    public static class ArkadeTestNameProvider
    {
        public static string GetDisplayName(IArkadeTest arkadeTest)
        {
            return GetDisplayName(arkadeTest.GetId(), ArkadeTestDisplayNames.Culture);
        }

        public static string GetDisplayName(TestId testId, SupportedLanguage language)
        {
            return GetDisplayName(testId, CultureInfo.CreateSpecificCulture(language.ToString()));
        }

        private static string GetDisplayName(TestId testId, CultureInfo culture)
        {
            string versionKeySuffix = testId.Version is "5.0" ? $"v{testId.Version}" : string.Empty;

            string resourceDisplayNameKey = $"{testId}{versionKeySuffix}".Replace('.', '_');

            string testName = ArkadeTestDisplayNames.ResourceManager.GetString(resourceDisplayNameKey, culture);

            return string.Format(ArkadeTestDisplayNames.DisplayNameFormat, testId, testName);
        }
    }
}
