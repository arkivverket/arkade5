using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.UI.Properties;

namespace Arkivverket.Arkade.UI.Util
{
    public static class ArkadeProcessingAreaLocationSetting
    {
        public static string Get()
        {
            Settings.Default.Reload();
            
            return Settings.Default.ArkadeProcessingAreaLocation;
        }

        public static void Set(string locationSetting)
        {
            Settings.Default.ArkadeProcessingAreaLocation = locationSetting;

            Settings.Default.Save();
        }

        public static bool IsValid()
        {
            try
            {
                string definedLocation = Get();

                return Directory.Exists(definedLocation);
            }
            catch
            {
                return false; // Invalid path string in settings 
            }
        }

        public static bool IsApplied()
        {
            string appliedLocation = ArkadeProcessingArea.Location?.FullName ?? string.Empty;
            
            string definedLocation = Get();

            return appliedLocation.Equals(definedLocation);
        }
    }
}
