using System;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class TestUtil
    {
        public static ArchiveExtraction CreateArchiveExtraction(string testdataDirectory)
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\{testdataDirectory}";
            var archiveExtraction = new ArchiveExtraction("uuid", workingDirectory);
            archiveExtraction.ArchiveType = ArchiveType.Noark5;
            return archiveExtraction;
        }
    }
}
