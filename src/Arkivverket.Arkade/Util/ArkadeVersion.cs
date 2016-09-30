using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Util
{
    public class ArkadeVersion
    {

        public static string GetVersion()
        {
            // TODO jostein: Is this the correct way of getting the application version?
            Assembly assembly = Assembly.GetEntryAssembly();
            string arkadeVersion = assembly?.GetName()?.Version?.ToString();

            return arkadeVersion != null ? arkadeVersion : "unknown";
        }

    }
}
