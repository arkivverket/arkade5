using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base
{
    public static class ArkadeTestInfoProvider
    {
        public static string GetDisplayName(IArkadeTest arkadeTest)
        {
            string resourceDisplayNameKey = arkadeTest.GetId().ToString().Replace('.', '_');

            if (arkadeTest.GetId().Version.Equals("5.5"))
            {
               resourceDisplayNameKey = $"{resourceDisplayNameKey}v5_5";
            }
            
            string resourceDisplayName = ArkadeTestDisplayNames.ResourceManager.GetString(resourceDisplayNameKey);

            return resourceDisplayName ?? GetFallBackDisplayName(arkadeTest);
        }

        private static string GetFallBackDisplayName(IArkadeTest arkadeTest)
        {
            try
            {
                return ((AddmlProcess) arkadeTest).GetName(); // Process name
            }
            catch
            {
                return arkadeTest.GetType().Name; // Class name
            }
        }
    }
}
