using System;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class TestUtil
    {
        public static Archive CreateArchiveExtraction(string testdataDirectory)
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\{testdataDirectory}";
            var archiveExtraction = new Archive(Uuid.Of("uuid"), workingDirectory);
            archiveExtraction.ArchiveType = ArchiveType.Noark5;
            return archiveExtraction;
        }
    }
}
