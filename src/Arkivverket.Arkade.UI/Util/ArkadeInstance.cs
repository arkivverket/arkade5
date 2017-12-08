using System.Diagnostics;

namespace Arkivverket.Arkade.UI.Util
{
    public static class ArkadeInstance
    {
        public static Process Process => Process.GetCurrentProcess();
        public static bool IsOnlyInstance => Process.GetProcessesByName(Process.ProcessName).Length == 1;
    }
}
