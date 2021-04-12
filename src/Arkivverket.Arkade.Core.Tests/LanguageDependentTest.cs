using System.Globalization;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Tests
{
    public abstract class LanguageDependentTest
    {
        private readonly string _unitTestingResourceLanguage = SupportedLanguage.nb.ToString(); // Norsk bokmål

        protected LanguageDependentTest()
        {
            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture(_unitTestingResourceLanguage);

            Messages.Culture = cultureInfo;
            AddmlMessages.Culture = cultureInfo;
            Noark5Messages.Culture = cultureInfo;
            Noark5TestDescriptions.Culture = cultureInfo;
            ArkadeTestDisplayNames.Culture = cultureInfo;
            ExceptionMessages.Culture = cultureInfo;
            OutputFileNames.Culture = cultureInfo;
            Resources.Report.Culture = cultureInfo;
            FormatAnalysisResultFileContent.Culture = cultureInfo;
        }
    }
}
