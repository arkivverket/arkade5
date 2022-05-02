using System.IO;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.GUI.Properties;

namespace Arkivverket.Arkade.GUI.Util
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

                return DirectoryIsWritable(definedLocation);
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

        private static bool DirectoryIsWritable(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
                return false;

            string tmpDirectory = Path.Combine(directory, "arkade-writeaccess-test");

            try
            {
                Directory.CreateDirectory(tmpDirectory);

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (Directory.Exists(tmpDirectory))
                    Directory.Delete(tmpDirectory);
            }
        }
    }
}
