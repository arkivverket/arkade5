using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base
{
    public static class ArkadeTestNameProvider
    {
        public static string GetDisplayName(IArkadeTest arkadeTest)
        {
            TestId testId = arkadeTest.GetId();

            string testName = GetTestName(testId);

            return string.Format(ArkadeTestDisplayNames.DisplayNameFormat, testId, testName);
        }

        private static string GetTestName(TestId testId)
        {
            string resourceDisplayNameKey = testId.ToString().Replace('.', '_');

            if (testId.Version.Equals("5.5"))
                resourceDisplayNameKey = $"{resourceDisplayNameKey}v5_5";

            return ArkadeTestDisplayNames.ResourceManager.GetString(resourceDisplayNameKey);
        }
    }
}
