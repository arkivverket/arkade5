using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_37_NumberOfCrossReferences : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 37);

        private N5_37_ArchivePart _currentArchivePart;
        private readonly List<N5_37_ArchivePart> _archiveParts = new();

        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override TestResultSet GetTestResults()
        {
            bool multipleArchiveParts = _archiveParts.Count > 1;

            int totalNumberOfCrossReferences = _archiveParts.Sum(CountTotalNumberOfCrossReferences);

            var testResultSets = new TestResultSet
            {
                TestsResults = new List<TestResult>
                {
                    new(ResultType.Success, new Location(""), string.Format(
                        Noark5Messages.TotalResultNumber, totalNumberOfCrossReferences))
                }
            };

            if (totalNumberOfCrossReferences == 0)
                return testResultSets;

            foreach (N5_37_ArchivePart archivePart in _archiveParts)
            {
                var testResults = new List<TestResult>();

                if (archivePart.ClassReferenceCount > 0)
                    testResults.Add(new TestResult(ResultType.Success, new Location(""), string.Format(
                        Noark5Messages.NumberOfCrossReferencesToClassMessage,
                        archivePart.ClassReferenceCount)));

                if (archivePart.FolderReferenceCount > 0)
                    testResults.Add(new TestResult(ResultType.Success, new Location(""), string.Format(
                        Noark5Messages.NumberOfCrossReferencesToFolderMessage,
                        archivePart.FolderReferenceCount)));

                if (archivePart.BasicRegistrationReferenceCount > 0)
                    testResults.Add(new TestResult(ResultType.Success, new Location(""), string.Format(
                        Noark5Messages.NumberOfCrossReferencesToBasicRegistrationMessage,
                        archivePart.BasicRegistrationReferenceCount)));
                if (multipleArchiveParts)
                {
                    testResults.Insert(0, new TestResult(ResultType.Success, new Location(string.Empty), string.Format(
                        Noark5Messages.NumberOf, CountTotalNumberOfCrossReferences(archivePart))));

                    testResultSets.TestResultSets.Add(new TestResultSet
                    {
                        Name = archivePart.ToString(),
                        TestsResults = testResults,
                    });
                }
                else
                    testResultSets.TestsResults.AddRange(testResults);
                
            }

            return testResultSets;
        }

        private int CountTotalNumberOfCrossReferences(N5_37_ArchivePart currentArchivePart)
        {
            int totalNumberOfCrossReferencesResult = new[]
            {
                currentArchivePart.ClassReferenceCount,
                currentArchivePart.BasicRegistrationReferenceCount,
                currentArchivePart.FolderReferenceCount
            }.Sum();

            return totalNumberOfCrossReferencesResult;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("referanseTilKlasse"))
                _currentArchivePart.ClassReferenceCount++;

            if (eventArgs.NameEquals("referanseTilMappe"))
                _currentArchivePart.FolderReferenceCount++;

            if (eventArgs.NameEquals("referanseTilRegistrering"))
                _currentArchivePart.BasicRegistrationReferenceCount++;
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart = new N5_37_ArchivePart { SystemId = eventArgs.Value };
                _archiveParts.Add(_currentArchivePart);
            }

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }


        private class N5_37_ArchivePart : ArchivePart
        {
            public int ClassReferenceCount;
            public int FolderReferenceCount;
            public int BasicRegistrationReferenceCount;
        }
    }
}
