using System;
using System.Threading;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.GUI.Properties;

namespace Arkivverket.Arkade.GUI.Util
{
    public static class LanguageSettingHelper
    {
        public static SupportedLanguage GetUILanguage()
        {
            if (!Enum.TryParse(Settings.Default.SelectedUILanguage, out SupportedLanguage uiLanguage))
            {
                string osLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

                if (!Enum.TryParse(osLanguage, out uiLanguage))
                {
                    uiLanguage = osLanguage == "nn" ? SupportedLanguage.nb : SupportedLanguage.en;
                }
            }

            return uiLanguage;
        }

        public static SupportedLanguage GetOutputLanguage()
        {
            if (!Enum.TryParse(Settings.Default.SelectedOutputLanguage, out SupportedLanguage outputLanguage))
            {
                outputLanguage = SupportedLanguage.nb;
            }

            return outputLanguage;
        }
    }
}
