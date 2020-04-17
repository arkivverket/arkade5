using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Testing.Noark5;
using Arkivverket.Arkade.Core.Testing.Noark5.Structure;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Noark5
{
    public class Noark5TestFactory
    {
        private readonly Archive _archive;
        private static SortedDictionary<TestId, Type> _arkadeTestImplementationsByTestId;

        public Noark5TestFactory(Archive archive = null)
        {
            _archive = archive;

            _arkadeTestImplementationsByTestId = GetArkadeTestImplementationsByTestId();
        }

        public List<TestId> GetTestIds()
        {
            return _arkadeTestImplementationsByTestId.Keys.ToList();
        }

        public IArkadeTest Create(TestId testId)
        {
            Type arkadeTestImplementation = _arkadeTestImplementationsByTestId[testId];

            bool isConstructedWithArchive = arkadeTestImplementation.GetConstructor(new[] {typeof(Archive)}) != null;

            var arkadeTestInstance = (IArkadeTest) (isConstructedWithArchive
                ? Activator.CreateInstance(arkadeTestImplementation, _archive)
                : Activator.CreateInstance(arkadeTestImplementation));

            return arkadeTestInstance;
        }

        private static SortedDictionary<TestId, Type> GetArkadeTestImplementationsByTestId()
        {
            return new SortedDictionary<TestId, Type>
            {
                {new TestId(TestId.TestKind.Noark5, 1), typeof(N5_01_ValidateStructureFileExists)},
                {new TestId(TestId.TestKind.Noark5, 2), typeof(N5_02_ValidateAddmlDataobjectsChecksums)},
                {new TestId(TestId.TestKind.Noark5, 3), typeof(N5_03_ValidateXmlWithSchema)},
                {new TestId(TestId.TestKind.Noark5, 4), typeof(N5_04_NumberOfArchives)},
                {new TestId(TestId.TestKind.Noark5, 5), typeof(N5_05_NumberOfArchiveParts)},
                {new TestId(TestId.TestKind.Noark5, 6), typeof(N5_06_StatusOfArchiveParts)},
                {new TestId(TestId.TestKind.Noark5, 7), typeof(N5_07_NumberOfClassificationSystems)},
                {new TestId(TestId.TestKind.Noark5, 8), typeof(N5_08_NumberOfClasses)},
                {new TestId(TestId.TestKind.Noark5, 9), typeof(N5_09_NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations)},
                {new TestId(TestId.TestKind.Noark5, 10), typeof(N5_10_NumberOfFolders)},
                {new TestId(TestId.TestKind.Noark5, 11), typeof(N5_11_NumberOfFoldersPerYear)},
                {new TestId(TestId.TestKind.Noark5, 12), typeof(N5_12_ControlNoSuperclassesHasFolders)},
                {new TestId(TestId.TestKind.Noark5, 13), typeof(N5_13_NumberOfFoldersPerClass)},
                {new TestId(TestId.TestKind.Noark5, 14), typeof(N5_14_NumberOfFoldersWithoutRegistrationsOrSubfolders)},
                {new TestId(TestId.TestKind.Noark5, 15), typeof(N5_15_NumberOfEachCaseFolderStatus)},
                {new TestId(TestId.TestKind.Noark5, 16), typeof(N5_16_NumberOfRegistrations)},
                {new TestId(TestId.TestKind.Noark5, 17), typeof(N5_17_NumberOfEachJournalPostType)},
                {new TestId(TestId.TestKind.Noark5, 18), typeof(N5_18_NumberOfRegistrationsPerYear)},
                {new TestId(TestId.TestKind.Noark5, 19), typeof(N5_19_ControlNoSuperclassesHasRegistrations)},
                {new TestId(TestId.TestKind.Noark5, 20), typeof(N5_20_NumberOfRegistrationsPerClass)},
                {new TestId(TestId.TestKind.Noark5, 21), typeof(N5_21_NumberOfRegistrationsWithoutDocumentDescription)},
                {new TestId(TestId.TestKind.Noark5, 22), typeof(N5_22_NumberOfEachJournalStatus)},
                {new TestId(TestId.TestKind.Noark5, 23), typeof(N5_23_NumberOfDocumentDescriptions)},
                {new TestId(TestId.TestKind.Noark5, 24), typeof(N5_24_NumberOfDocumentDescriptionsWithoutDocumentObject)},
                {new TestId(TestId.TestKind.Noark5, 25), typeof(N5_25_NumberOfEachDocumentStatus)},
                {new TestId(TestId.TestKind.Noark5, 26), typeof(N5_26_NumberOfDocumentObjects)},
                {new TestId(TestId.TestKind.Noark5, 27), typeof(N5_27_FirstAndLastRegistrationCreationDates)},
                {new TestId(TestId.TestKind.Noark5, 28), typeof(N5_28_ValidateNumberOfDocumentfiles)},
                {new TestId(TestId.TestKind.Noark5, 29), typeof(N5_29_NumberOfEachDocumentFormat)},
                {new TestId(TestId.TestKind.Noark5, 30), typeof(N5_30_DocumentFilesChecksumControl)},
                {new TestId(TestId.TestKind.Noark5, 32), typeof(N5_32_ControlDocumentFilesExists)},
                {new TestId(TestId.TestKind.Noark5, 33), typeof(N5_33_DocumentfilesReferenceControl)},
                {new TestId(TestId.TestKind.Noark5, 34), typeof(N5_34_NumberOfMultiReferencedDocumentFiles)},
                {new TestId(TestId.TestKind.Noark5, 35), typeof(N5_35_NumberOfCaseParts)},
                {new TestId(TestId.TestKind.Noark5, 36), typeof(N5_36_NumberOfComments)},
                {new TestId(TestId.TestKind.Noark5, 37), typeof(N5_37_NumberOfCrossReferences)},
                {new TestId(TestId.TestKind.Noark5, 38), typeof(N5_38_NumberOfPrecedents)},
                {new TestId(TestId.TestKind.Noark5, 39), typeof(N5_39_NumberOfCorrespondenceParts)},
                {new TestId(TestId.TestKind.Noark5, 40), typeof(N5_40_NumberOfDepreciations)},
                {new TestId(TestId.TestKind.Noark5, 41), typeof(N5_41_NumberOfDocumentFlows)},
                {new TestId(TestId.TestKind.Noark5, 42), typeof(N5_42_NumberOfRestrictions)},
                {new TestId(TestId.TestKind.Noark5, 43), typeof(N5_43_NumberOfClassifications)},
                {new TestId(TestId.TestKind.Noark5, 44), typeof(N5_44_NumberOfDisposalResolutions)},
                {new TestId(TestId.TestKind.Noark5, 45), typeof(N5_45_NumberOfDisposalsExecuted)},
                {new TestId(TestId.TestKind.Noark5, 46), typeof(N5_46_NumberOfConversions)},
                {new TestId(TestId.TestKind.Noark5, 47), typeof(N5_47_SystemIdUniqueControl)},
                {new TestId(TestId.TestKind.Noark5, 48), typeof(N5_48_ArchivepartReferenceControl)},
                {new TestId(TestId.TestKind.Noark5, 51), typeof(N5_51_ClassReferenceControl)},
                {new TestId(TestId.TestKind.Noark5, 59), typeof(N5_59_NumberOfJournalPosts)},
                {new TestId(TestId.TestKind.Noark5, 60), typeof(N5_60_ArchiveStartAndEndDateControl)},
                {new TestId(TestId.TestKind.Noark5, 61), typeof(N5_61_NumberOfChangesLogged)},
                {new TestId(TestId.TestKind.Noark5, 62), typeof(N5_62_ChangeLogArchiveReferenceControl)},
                {new TestId(TestId.TestKind.Noark5, 63), typeof(N5_63_ControlElementsHasContent)}
            };
        }
    }
}
