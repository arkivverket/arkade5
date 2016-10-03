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

        public List<ITest> GetStructureTests()
        {
            return new List<ITest>
            {
                new Structure.CheckWellFormedArchiveStructureXml(_archiveReader),
                new ValidateAddmlDataobjectsChecksums(_archiveReader),
                new ValidateXmlWithSchema(_archiveReader)
            };
        }

        public List<ITest> GetContentTests()
        {
            return new List<ITest>
            {
                new CheckWellFormedContentDescriptionXml(_archiveReader),
                new NumberOfArchives(_archiveReader),
                new NumberOfArchiveParts(_archiveReader),
                new StatusOfArchiveParts(_archiveReader),
                new NumberOfClasses(_archiveReader),
                new NumberOfFolders(_archiveReader),
                new NumberOfClassificationSystems(_archiveReader),
                new NumberOfClassesInMainClassificationSystemWithoutSubClassesorFolders(_archiveReader)
            };
        }

        public List<ITest> GetTests()
        {
            var tests = GetStructureTests();
            tests.AddRange(GetContentTests());
            return tests;
        }
    }
}