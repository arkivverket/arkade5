using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Tests.Noark5;

namespace Arkivverket.Arkade.Tests
{
    public class TestProvider : ITestProvider
    {
        private readonly Noark5TestProvider _noark5TestProvider;

        public TestProvider(Noark5TestProvider noark5TestProvider)
        {
            _noark5TestProvider = noark5TestProvider;
        }

        public List<INoark5Test> GetTestsForArchive(Archive archive)
        {
            if (archive.ArchiveType.Equals(ArchiveType.Noark5))
            {
                return _noark5TestProvider.GetTests(archive);
            }

            return new List<INoark5Test>();
        }
        
    }
}