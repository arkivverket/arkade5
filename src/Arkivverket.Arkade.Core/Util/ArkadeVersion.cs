using System;
using System.Reflection;
using Serilog;

namespace Arkivverket.Arkade.Core.Util
{
    public class ArkadeVersion
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IReleaseInfoReader _releaseInfoReader;

        public ArkadeVersion(IReleaseInfoReader releaseInfoReader)
        {
            _releaseInfoReader = releaseInfoReader;
        }

        public static string Current => GetCurrent().ToString(3);

        public static Version GetCurrent()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        public Version GetLatest()
        {
            try
            {
                Version latestVersion = _releaseInfoReader.GetLatestVersion();

                LocalInfo.SetTimeLastCheckForUpdate(DateTime.Now);

                return latestVersion;
            }
            catch(Exception e)
            {
                Log.Warning("Could not get information about latest version: " + e.Message);

                return null;
            }
        }

        public bool UpdateIsAvailable()
        {
            return GetCurrent().CompareTo(GetLatest()) < 0;
        }

        public DateTime? GetTimeLastCheckForUpdate()
        {
            return LocalInfo.GetTimeLastCheckForUpdate();
        }
    }
}
