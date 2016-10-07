using System.Reflection;

namespace Arkivverket.Arkade.Util
{
    public class ArkadeVersion
    {

        public static string Version
        {
            get
            {
                return Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString();
            }
        }

    }
}
