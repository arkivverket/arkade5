using Arkivverket.Arkade.Core.Base.Addml.Processes;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base
{
    public static class ArkadeTestNameProvider
    {
        public static string GetDisplayName(IArkadeTest arkadeTest)
        {
            TestId testId = arkadeTest.GetId();

            string namePart = GetNamePart(testId);

            string displayName = namePart != null
                ? string.Format(ArkadeTestDisplayNames.DisplayNameFormat, testId, namePart)
                : GetFallBackDisplayName(arkadeTest);

            return displayName;
        }

        private static string GetNamePart(TestId testId)
        {
            string resourceDisplayNameKey = testId.ToString().Replace('.', '_');

            if (testId.Version.Equals("5.5"))
                resourceDisplayNameKey = $"{resourceDisplayNameKey}v5_5";

            return ArkadeTestDisplayNames.ResourceManager.GetString(resourceDisplayNameKey);
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
