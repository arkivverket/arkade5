using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;

namespace Arkivverket.Arkade.Tests
{
    public class TestProvider
    {

        public List<BaseTest> GetTestsForArchiveExtraction(ArchiveExtraction archiveExtraction)
        {
            if (archiveExtraction.ArchiveType.Equals(ArchiveType.Noark5))
            {
                return new Noark5TestProvider().GetTests();
            }

            return new List<BaseTest>();
        }

    }
}
