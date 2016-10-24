using System;
using System.IO;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class TestUtil
    {
        public static Archive CreateArchiveExtraction(string testdataDirectory)
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\{testdataDirectory}";
            return new Core.ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectory(workingDirectory)
                .Build();
        }

        public static string ReadFromFileInTestDataDir(string fileName)
        {
            char s = Path.DirectorySeparatorChar;
            string dir = AppDomain.CurrentDomain.BaseDirectory + s + ".." + s + ".." + s + "TestData" + s + fileName;
            return File.ReadAllText(dir);
        }

    }
}
