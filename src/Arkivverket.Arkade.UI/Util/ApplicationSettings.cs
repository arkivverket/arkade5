using System;
using System.IO;

namespace Arkivverket.Arkade.UI.Util
{
    public static class ApplicationSettings
    {

        /// <summary>
        /// Example path: C:\Users\USERNAME\AppData\Local\Arkade
        /// </summary>
        /// <returns></returns>
        public static DirectoryInfo ApplicationDataDirectory()
        {
            return new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Arkade"));
        }

    }
}
