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
         //       new Structure.CheckWellFormedArchiveStructureXml(_archiveReader),
         //       new ValidateAddmlDataobjectsChecksums(_archiveReader),
         //       new ValidateXmlWithSchema(_archiveReader)
            };
        }

        public List<ITest> GetContentTests(Archive archive)
        {
            return new List<ITest>
            {
            //    new CheckWellFormedContentDescriptionXml(_archiveReader),
                new NumberOfArchives(),
                new NumberOfArchiveParts(),
                new StatusOfArchiveParts(),
                new NumberOfClasses(),
                new NumberOfFolders(),
                new NumberOfClassificationSystems(),
            //    new NumberOfClassesInMainClassificationSystemWithoutSubClassesorFolders(_archiveReader),
                new ControlDocumentFilesExists(archive)
            };
        }

        public List<ITest> GetTests(Archive archive)
        {
            var tests = GetStructureTests();
            tests.AddRange(GetContentTests(archive));
            return tests;
        }
    }
}