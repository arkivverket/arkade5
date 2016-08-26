using System.Collections.Generic;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class Noark5TestProvider
    {
        public List<BaseTest> GetTests()
        {
            return new List<BaseTest>
            {
                new CheckWellFormedXml(),
                new NumberOfArchives(),
                new NumberOfArchiveParts()
            };
        }
    }
}