using System.IO;
using System.Reflection;

namespace Arkivverket.Arkade.Core
{
    public class ResourceUtil
    {
        public static string ReadResource(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}