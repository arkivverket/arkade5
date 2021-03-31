using System.Globalization;
using System.Threading;

namespace Arkivverket.Arkade.Core.Languages
{
    public static class LanguageManager
    {
        internal static void SetResourcesLanguageForTesting(SupportedLanguage language)
        {
            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture(language.ToString());
            
            Resources.ArkadeTestDisplayNames.Culture = cultureInfo;
            Resources.AddmlMessages.Culture = cultureInfo;
            Resources.ExceptionMessages.Culture = cultureInfo;
            Resources.Messages.Culture = cultureInfo;
            Resources.Noark5Messages.Culture = cultureInfo;
            Resources.Noark5TestDescriptions.Culture = cultureInfo;
            Resources.OutputFileNames.Culture = cultureInfo;
            Resources.Report.Culture = cultureInfo;
        }

        internal static void SetResourceLanguageForPackageCreation(SupportedLanguage language)
        {
            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture(language.ToString());

            Resources.ArkadeTestDisplayNames.Culture = cultureInfo;
            Resources.AddmlMessages.Culture = cultureInfo;
            Resources.ExceptionMessages.Culture = cultureInfo;
            Resources.FormatAnalysisResultFileContent.Culture = cultureInfo;
            Resources.Messages.Culture = cultureInfo;
            Resources.OutputFileNames.Culture = cultureInfo;
        }

        internal static void SetResourceLanguageForStandalonePronomAnalysis(SupportedLanguage language)
        {
            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture(language.ToString());

            Resources.FormatAnalysisResultFileContent.Culture = cultureInfo;
            Resources.OutputFileNames.Culture = cultureInfo;
        }
    }

    public enum SupportedLanguage
    {
        en,
        nb,
    }
}
