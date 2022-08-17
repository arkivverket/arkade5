using System;
using System.IO;
using Arkivverket.Arkade.Core.Base;

namespace Arkivverket.Arkade.Core.Tests.Testing.Noark5
{
    public class TestUtil
    {
        public static string TestDataDirectory =  Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");

        public static Archive CreateArchiveExtraction(string testdataDirectory)
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\{testdataDirectory}";
            return new Base.ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryExternalContent(workingDirectory)
                .Build();
        }

        public static Archive CreateArchiveExtractionV5_5(string testdataDirectory)
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\{testdataDirectory}";
            return new Base.ArchiveBuilder()
                .WithArchiveType(ArchiveType.Noark5)
                .WithWorkingDirectoryExternalContent(workingDirectory)
                .WithArchiveDetails("5.0")
                .Build();
        }

        public static string ReadFromFileInTestDataDir(string fileName)
        {
            return File.ReadAllText(Path.Combine(TestDataDirectory, fileName));
        }

        public static Stream ReadFileStreamFromTestDataDir(string fileName)
        {
            return File.OpenRead(Path.Combine(TestDataDirectory, fileName));
        }

    }
}
