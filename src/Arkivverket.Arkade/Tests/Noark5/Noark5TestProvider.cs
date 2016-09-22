using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5.Structure;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class Noark5TestProvider
    {
        private readonly IArchiveContentReader _archiveReader;

        public Noark5TestProvider(IArchiveContentReader archiveReader)
        {
            _archiveReader = archiveReader;
        }

        public List<BaseTest> GetStructureTests()
        {
            return new List<BaseTest>
            {
                new Structure.CheckWellFormedXml(_archiveReader),
                new ValidateAddmlDataobjectsChecksums(_archiveReader),
                new ValidateXmlWithSchema(_archiveReader)
            };
        }

        public List<BaseTest> GetContentTests()
        {
            return new List<BaseTest>
            {
                new CheckWellFormedXml(_archiveReader),
                new NumberOfArchives(_archiveReader),
                new NumberOfArchiveParts(_archiveReader),
                new StatusOfArchiveParts(_archiveReader),
                new NumberOfClasses(_archiveReader)
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