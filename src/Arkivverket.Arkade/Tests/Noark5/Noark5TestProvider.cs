using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5.Structure;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class Noark5TestProvider: ITestProvider
    {
        private readonly IArchiveContentReader _archiveReader;

        public Noark5TestProvider(IArchiveContentReader archiveReader)
        {
            _archiveReader = archiveReader;
        }

        public List<IArkadeStructureTest> GetStructureTests()
        {
            return new List<IArkadeStructureTest>
            {
                new ValidateAddmlDataobjectsChecksums(),
                new ValidateXmlWithSchema(_archiveReader),
                new ValidateNumberOfDocumentfiles(),
            };
        }

        public List<INoark5Test> GetContentTests(Archive archive)
        {
            return new List<INoark5Test>
            {
                new NumberOfArchives(),
                new NumberOfArchiveParts(),
                new StatusOfArchiveParts(),
                new NumberOfClasses(),
                new NumberOfFolders(),
                new NumberOfClassificationSystems(),
                new NumberOfRegistrations(),
                new NumberOfRegistrationsPerYear(),
                new NumberOfRegistrationsWithoutDocumentDescription(),
                new NumberOfDocumentDescriptions(),
                new NumberOfDocumentDescriptionsWithoutDocumentObject(),
                new NumberOfDocumentObjects(),
                new NumberOfClassesInMainClassificationSystemWithoutSubClassesorFolders(),
                new NumberOfCaseParts(),
                new ControlDocumentFilesExists(archive),
                new NumberOfComments(),
                new NumberOfCrossReferences(),
                new NumberOfPrecedents(),
                new NumberOfCorrespondenceParts(),
                new NumberOfDepreciations(),
                new NumberOfDocumentFlows(),
                new NumberOfFoldersPerYear(),
                new NumberOfFoldersWithoutRegistrationsOrSubfolders(),
                new FirstAndLastRegistrationCreationDates(),
                new NumberOfJournalPosts(archive),
                new NumberOfFoldersPerClass(),
                new NumberOfCaseStatusesPerArchivePart(),
                new DocumentfilesReferenceControl(archive),
                new NumberOfEachJournalPostType(),
                new NumberOfRegistrationsPerClass(),
                new NumberOfEachJournalStatus(),
                new NumberOfEachDocumentStatus(),
                new NumberOfEachDocumentFormat(),
                new NumberOfMultiReferencedDocumentFiles(),
                new ControlNoSuperclassesHasFolders(),
                new NumberOfClassifications(),
                new NumberOfDisposalResolutions(archive),
                new NumberOfDisposalsExecuted(archive),
                new NumberOfRestrictions(archive),
                new NumberOfConversions(),
                new ArchiveStartAndEndDateControl(archive),
                new NumberOfChangesLogged(archive),
                new ChangeLogArchiveReferenceControl(archive),
                new ArchivepartReferenceControl(),
                new SystemIdUniqueControl(),
                new ControlNoSuperclassesHasRegistrations(),
                new ClassReferenceControl(),
                new DocumentFilesChecksumControl(archive),
            };
        }
    }
}