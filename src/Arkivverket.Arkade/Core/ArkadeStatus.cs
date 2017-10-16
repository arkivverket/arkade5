namespace Arkivverket.Arkade.Core
{
    public static class ArkadeStatus
    {
        public static bool RestartIsNeeded => ArkadeProcessingAreaLocationHasPendingChanges();

        private static bool ArkadeProcessingAreaLocationHasPendingChanges()
        {
            string currentLocation = ArkadeProcessingArea.Location?.FullName ?? string.Empty;
            string userSetLocation = Properties.Settings.Default.ArkadeProcessingAreaLocation;

            return !currentLocation.Equals(userSetLocation);
        }
    }
}
