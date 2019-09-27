using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5.Structure;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class Noark5TestProvider: ITestProvider
    {
        public List<IArkadeStructureTest> GetStructureTests()
        {
            return new List<IArkadeStructureTest>
            {
                new N5_01_ValidateStructureFileExists(),
                new N5_02_ValidateAddmlDataobjectsChecksums(),
                new N5_03_ValidateXmlWithSchema(),
                new N5_28_ValidateNumberOfDocumentfiles(),
            };
        }

        public List<INoark5Test> GetContentTests(Archive archive)
        {
            return new List<INoark5Test>
            {
                new N5_04_NumberOfArchives(),
                new N5_05_NumberOfArchiveParts(),
                new N5_06_StatusOfArchiveParts(),
                new N5_08_NumberOfClasses(),
                new N5_10_NumberOfFolders(archive),
                new N5_07_NumberOfClassificationSystems(),
                new N5_16_NumberOfRegistrations(),
                new N5_18_NumberOfRegistrationsPerYear(),
                new N5_21_NumberOfRegistrationsWithoutDocumentDescription(),
                new N5_23_NumberOfDocumentDescriptions(),
                new N5_24_NumberOfDocumentDescriptionsWithoutDocumentObject(),
                new N5_26_NumberOfDocumentObjects(),
                new N5_09_NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations(),
                new N5_35_NumberOfCaseParts(archive),
                new N5_32_ControlDocumentFilesExists(archive),
                new N5_36_NumberOfComments(),
                new N5_37_NumberOfCrossReferences(),
                new N5_38_NumberOfPrecedents(),
                new N5_39_NumberOfCorrespondenceParts(),
                new N5_40_NumberOfDepreciations(),
                new N5_41_NumberOfDocumentFlows(),
                new N5_11_NumberOfFoldersPerYear(),
                new N5_14_NumberOfFoldersWithoutRegistrationsOrSubfolders(),
                new N5_27_FirstAndLastRegistrationCreationDates(),
                new N5_59_NumberOfJournalPosts(archive),
                new N5_13_NumberOfFoldersPerClass(),
                new N5_33_DocumentfilesReferenceControl(archive),
                new N5_17_NumberOfEachJournalPostType(),
                new N5_20_NumberOfRegistrationsPerClass(),
                new N5_22_NumberOfEachJournalStatus(),
                new N5_15_NumberOfEachCaseFolderStatus(),
                new N5_25_NumberOfEachDocumentStatus(),
                new N5_29_NumberOfEachDocumentFormat(),
                new N5_34_NumberOfMultiReferencedDocumentFiles(),
                new N5_12_ControlNoSuperclassesHasFolders(),
                new N5_43_NumberOfClassifications(),
                new N5_44_NumberOfDisposalResolutions(archive),
                new N5_45_NumberOfDisposalsExecuted(archive),
                new N5_42_NumberOfRestrictions(archive),
                new N5_46_NumberOfConversions(),
                new N5_60_ArchiveStartAndEndDateControl(archive),
                new N5_61_NumberOfChangesLogged(archive),
                new N5_62_ChangeLogArchiveReferenceControl(archive),
                new N5_48_ArchivepartReferenceControl(),
                new N5_47_SystemIdUniqueControl(),
                new N5_19_ControlNoSuperclassesHasRegistrations(),
                new N5_51_ClassReferenceControl(),
                new N5_30_DocumentFilesChecksumControl(archive),
                new N5_63_ControlElementsHasContent(),
            };
        }
    }
}