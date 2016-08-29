using System.Collections.Generic;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class Noark5TestProvider
    {

        public List<BaseTest> GetStructureTests()
        {
            return new List<BaseTest>
            {
               
            };
        }

        public List<BaseTest> GetContentTests()
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