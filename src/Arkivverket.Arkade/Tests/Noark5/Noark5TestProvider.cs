using System.Collections.Generic;
using Arkivverket.Arkade.Tests.Noark5.Structure;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class Noark5TestProvider
    {

        public List<BaseTest> GetStructureTests()
        {
            return new List<BaseTest>
            {
               new ValidateAddmlDataobjectsChecksums()
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