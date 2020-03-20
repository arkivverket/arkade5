using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_37_NumberOfCrossReferences : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 37);

        private N5_37_ArchivePart _currentArchivePart;
        private readonly List<N5_37_ArchivePart> _archiveParts = new List<N5_37_ArchivePart>();

        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();
            int totalNumberOfCrossReferences = 0;

            if (_archiveParts.Count == 1)
            {
                testResults.Add(new TestResult(ResultType.Success, new Location(""),
                    string.Format(Noark5Messages.NumberOfCrossReferencesToClassMessage,
                        _currentArchivePart.ClassReferenceCount)));

                testResults.Add(new TestResult(ResultType.Success, new Location(""),
                    string.Format(Noark5Messages.NumberOfCrossReferencesToFolderMessage,
                        _currentArchivePart.FolderReferenceCount)));

                testResults.Add(new TestResult(ResultType.Success, new Location(""),
                    string.Format(Noark5Messages.NumberOfCrossReferencesToBasicRegistrationMessage,
                        _currentArchivePart.BasicRegistrationReferenceCount)));

                totalNumberOfCrossReferences = CountTotalNumberOfCrossReferences(_currentArchivePart);
            }

            else
            {
                foreach (N5_37_ArchivePart archivePart in _archiveParts)
                {
                    if (archivePart.ClassReferenceCount > 0)
                    {
                        testResults.Add(new TestResult(ResultType.Success, new Location(""),
                            string.Format(Noark5Messages.NumberOfCrossReferencesToClassMessage_ForArchivePart,
                                archivePart.SystemId,
                                archivePart.Name,
                                archivePart.ClassReferenceCount)));
                    }

                    if (archivePart.FolderReferenceCount > 0)
                    {
                        testResults.Add(new TestResult(ResultType.Success, new Location(""),
                            string.Format(Noark5Messages.NumberOfCrossReferencesToFolderMessage_ForArchivePart,
                                archivePart.SystemId,
                                archivePart.Name,
                                archivePart.FolderReferenceCount)));
                    }

                    if (archivePart.BasicRegistrationReferenceCount > 0)
                    {
                        testResults.Add(new TestResult(ResultType.Success, new Location(""),
                            string.Format(
                                Noark5Messages.NumberOfCrossReferencesToBasicRegistrationMessage_ForArchivePart,
                                archivePart.SystemId,
                                archivePart.Name,
                                archivePart.BasicRegistrationReferenceCount)));
                    }

                    totalNumberOfCrossReferences += CountTotalNumberOfCrossReferences(archivePart);
                }
            }

            testResults.Insert(0, new TestResult(ResultType.Success, new Location(""),
                string.Format(Noark5Messages.TotalResultNumber, totalNumberOfCrossReferences.ToString())));

            return testResults;
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
