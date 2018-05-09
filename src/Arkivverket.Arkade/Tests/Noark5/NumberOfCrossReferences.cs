using System.Collections.Generic;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfCrossReferences : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 37);

        private ArchivePart _currentArchivePart = new ArchivePart();
        private readonly List<ArchivePart> _archiveParts = new List<ArchivePart>();

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5TestNames.NumberOfCrossReferences;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

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
            }

            else
            {
                foreach (ArchivePart archivePart in _archiveParts)
                {
                    if (archivePart.ClassReferenceCount > 0)
                    {
                        testResults.Add(new TestResult(ResultType.Success, new Location(""),
                            string.Format(Noark5Messages.NumberOfCrossReferencesToClassMessage_ForArchivePart,
                                archivePart.SystemId,
                                archivePart.ClassReferenceCount)));
                    }

                    if (archivePart.FolderReferenceCount > 0)
                    {
                        testResults.Add(new TestResult(ResultType.Success, new Location(""),
                            string.Format(Noark5Messages.NumberOfCrossReferencesToFolderMessage_ForArchivePart,
                                archivePart.SystemId,
                                archivePart.FolderReferenceCount)));
                    }

                    if (archivePart.BasicRegistrationReferenceCount > 0)
                    {
                        testResults.Add(new TestResult(ResultType.Success, new Location(""),
                            string.Format(
                                Noark5Messages.NumberOfCrossReferencesToBasicRegistrationMessage_ForArchivePart,
                                archivePart.SystemId,
                                archivePart.BasicRegistrationReferenceCount)));
                    }
                }
            }

            return testResults;
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
                _currentArchivePart = new ArchivePart {SystemId = eventArgs.Value};
                _archiveParts.Add(_currentArchivePart);
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }


        private class ArchivePart
        {
            public string SystemId { get; set; }
            public int ClassReferenceCount;
            public int FolderReferenceCount;
            public int BasicRegistrationReferenceCount;
        }
    }
}
