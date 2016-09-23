using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;

namespace Arkivverket.Arkade.Tests
{
    public class TestProvider
    {
        private readonly Noark5TestProvider _noark5TestProvider;

        public TestProvider(Noark5TestProvider noark5TestProvider)
        {
            _noark5TestProvider = noark5TestProvider;
        }

        public List<BaseTest> GetTestsForArchiveExtraction(Archive archiveExtraction)
        {
            if (archiveExtraction.ArchiveType.Equals(ArchiveType.Noark5))
            {
                return _noark5TestProvider.GetTests();
            }

            return new List<BaseTest>();
        }
    }
}