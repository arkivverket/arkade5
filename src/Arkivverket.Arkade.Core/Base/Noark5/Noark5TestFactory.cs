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
                {TestId.Create("N5.01"), typeof(N5_01_ValidateStructureFileExists)},
                {TestId.Create("N5.02"), typeof(N5_02_ValidateAddmlDataobjectsChecksums)},
                {TestId.Create("N5.03"), typeof(N5_03_ValidateXmlWithSchema)},
                {TestId.Create("N5.04"), typeof(N5_04_NumberOfArchives)},
                {TestId.Create("N5.05"), typeof(N5_05_NumberOfArchiveParts)},
                {TestId.Create("N5.06"), typeof(N5_06_StatusOfArchiveParts)},
                {TestId.Create("N5.07"), typeof(N5_07_NumberOfClassificationSystems)},
                {TestId.Create("N5.08"), typeof(N5_08_NumberOfClasses)},
                {TestId.Create("N5.09"), typeof(N5_09_NumberOfClassesInMainClassificationSystemWithoutSubClassesFoldersOrRegistrations)},
                {TestId.Create("N5.10"), typeof(N5_10_NumberOfFolders)},
                {TestId.Create("N5.11"), typeof(N5_11_NumberOfFoldersPerYear)},
                {TestId.Create("N5.12"), typeof(N5_12_ControlNoSuperclassesHasFolders)},
                {TestId.Create("N5.13"), typeof(N5_13_NumberOfFoldersPerClass)},
                {TestId.Create("N5.14"), typeof(N5_14_NumberOfFoldersWithoutRegistrationsOrSubfolders)},
                {TestId.Create("N5.15"), typeof(N5_15_NumberOfEachCaseFolderStatus)},
                {TestId.Create("N5.16"), typeof(N5_16_NumberOfRegistrations)},
                {TestId.Create("N5.17"), typeof(N5_17_NumberOfEachJournalPostType)},
                {TestId.Create("N5.18"), typeof(N5_18_NumberOfRegistrationsPerYear)},
                {TestId.Create("N5.19"), typeof(N5_19_ControlNoSuperclassesHasRegistrations)},
                {TestId.Create("N5.20"), typeof(N5_20_NumberOfRegistrationsPerClass)},
                {TestId.Create("N5.21"), typeof(N5_21_NumberOfRegistrationsWithoutDocumentDescription)},
                {TestId.Create("N5.22"), typeof(N5_22_NumberOfEachJournalStatus)},
                {TestId.Create("N5.23"), typeof(N5_23_NumberOfDocumentDescriptions)},
                {TestId.Create("N5.24"), typeof(N5_24_NumberOfDocumentDescriptionsWithoutDocumentObject)},
                {TestId.Create("N5.25"), typeof(N5_25_NumberOfEachDocumentStatus)},
                {TestId.Create("N5.26"), typeof(N5_26_NumberOfDocumentObjects)},
                {TestId.Create("N5.27"), typeof(N5_27_FirstAndLastRegistrationCreationDates)},
                {TestId.Create("N5.28"), typeof(N5_28_ValidateNumberOfDocumentfiles)},
                {TestId.Create("N5.29"), typeof(N5_29_NumberOfEachDocumentFormat)},
                {TestId.Create("N5.30"), typeof(N5_30_DocumentFilesChecksumControl)},
                {TestId.Create("N5.32"), typeof(N5_32_ControlDocumentFilesExists)},
                {TestId.Create("N5.33"), typeof(N5_33_DocumentfilesReferenceControl)},
                {TestId.Create("N5.34"), typeof(N5_34_NumberOfMultiReferencedDocumentFiles)},
                {TestId.Create("N5.35"), typeof(N5_35_NumberOfCaseParts)},
                {TestId.Create("N5.36"), typeof(N5_36_NumberOfComments)},
                {TestId.Create("N5.37"), typeof(N5_37_NumberOfCrossReferences)},
                {TestId.Create("N5.38"), typeof(N5_38_NumberOfPrecedents)},
                {TestId.Create("N5.39"), typeof(N5_39_NumberOfCorrespondenceParts)},
                {TestId.Create("N5.40"), typeof(N5_40_NumberOfDepreciations)},
                {TestId.Create("N5.41"), typeof(N5_41_NumberOfDocumentFlows)},
                {TestId.Create("N5.42"), typeof(N5_42_NumberOfRestrictions)},
                {TestId.Create("N5.43"), typeof(N5_43_NumberOfClassifications)},
                {TestId.Create("N5.44"), typeof(N5_44_NumberOfDisposalResolutions)},
                {TestId.Create("N5.45"), typeof(N5_45_NumberOfDisposalsExecuted)},
                {TestId.Create("N5.46"), typeof(N5_46_NumberOfConversions)},
                {TestId.Create("N5.47"), typeof(N5_47_SystemIdUniqueControl)},
                {TestId.Create("N5.48"), typeof(N5_48_ArchivepartReferenceControl)},
                {TestId.Create("N5.51"), typeof(N5_51_ClassReferenceControl)},
                {TestId.Create("N5.59"), typeof(N5_59_NumberOfJournalPosts)},
                {TestId.Create("N5.60"), typeof(N5_60_ArchiveStartAndEndDateControl)},
                {TestId.Create("N5.61"), typeof(N5_61_NumberOfChangesLogged)},
                {TestId.Create("N5.62"), typeof(N5_62_ChangeLogArchiveReferenceControl)},
                {TestId.Create("N5.63"), typeof(N5_63_ControlElementsHasContent)}
            };
        }
    }
}
