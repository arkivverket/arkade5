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

        public List<INoark5Test> GetStructureTests()
        {
            return new List<INoark5Test>
            {
         //       new Structure.CheckWellFormedArchiveStructureXml(_archiveReader),
         //       new ValidateAddmlDataobjectsChecksums(_archiveReader),
         //       new ValidateXmlWithSchema(_archiveReader)
            };
        }

        public List<INoark5Test> GetContentTests(Archive archive)
        {
            return new List<INoark5Test>
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

        public List<INoark5Test> GetTests(Archive archive)
        {
            var tests = GetStructureTests();
            tests.AddRange(GetContentTests(archive));
            return tests;
        }
    }
}