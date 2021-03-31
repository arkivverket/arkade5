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
            string resourceDisplayNameKey = testId.ToString().Replace('.', '_');

            if (testId.Version.Equals("5.5"))
                resourceDisplayNameKey = $"{resourceDisplayNameKey}v5_5";

            string testName = ArkadeTestDisplayNames.ResourceManager.GetString(resourceDisplayNameKey, culture);

            return string.Format(ArkadeTestDisplayNames.DisplayNameFormat, testId, testName);
        }
    }
}
