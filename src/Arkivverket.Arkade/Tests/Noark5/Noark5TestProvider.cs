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
                new Structure.CheckWellFormedXml(),
                new ValidateAddmlDataobjectsChecksums(),
                new ValidateXmlWithSchema()
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

        public List<BaseTest> GetTests()
        {
            var tests = GetStructureTests();
            tests.AddRange(GetContentTests());
            return tests;
        }
    }
}