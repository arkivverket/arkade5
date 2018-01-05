using System.Collections.Generic;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfComments : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 0); // TODO: Assign correct test number

        private ArchivePart _currentArchivePart = new ArchivePart();
        private readonly List<ArchivePart> _archiveParts = new List<ArchivePart>();
        private bool _baseRegistrationAttributeIsFound;

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfComments;
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
                testResults.Add(new TestResult(ResultType.Success,
                    new Location(""),
                    string.Format(Noark5Messages.NumberOfCommentsInFoldersMessage,
                        _currentArchivePart.NumberOfCommentsInFolder
                    )));

                testResults.Add(new TestResult(ResultType.Success,
                    new Location(""),
                    string.Format(Noark5Messages.NumberOfCommentsInBaseRegistrationMessage,
                        _currentArchivePart.NumberOfCommentsInBaseRegistration
                    )));

                testResults.Add(new TestResult(ResultType.Success,
                    new Location(""),
                    string.Format(Noark5Messages.NumberOfCommentsInDocumentDescriptionMessage,
                        _currentArchivePart.NumberOfCommentsInDocumentDescription
                    )));
            }
            else

            {
                foreach (ArchivePart archivePart in _archiveParts)
                {
                    if (archivePart.NumberOfCommentsInFolder > 0)
                    {
                        testResults.Add(new TestResult(ResultType.Success, new Location(""),
                            string.Format(Noark5Messages.NumberOfCommentsInFoldersMessage_ForArchivePart,
                                archivePart.SystemId,
                                archivePart.NumberOfCommentsInFolder)));
                    }

                    if (archivePart.NumberOfCommentsInDocumentDescription > 0)
                    {
                        testResults.Add(new TestResult(ResultType.Success, new Location(""),
                            string.Format(Noark5Messages.NumberOfCommentsInDocumentDescriptionMessage_ForArchivePart,
                                archivePart.SystemId,
                                archivePart.NumberOfCommentsInDocumentDescription)));
                    }

                    if (archivePart.NumberOfCommentsInBaseRegistration > 0)
                    {
                        testResults.Add(new TestResult(ResultType.Success, new Location(""),
                            string.Format(Noark5Messages.NumberOfCommentsInBaseRegistrationMessage_ForArchivePart,
                                archivePart.SystemId,
                                archivePart.NumberOfCommentsInBaseRegistration)));
                    }
                }
            }

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("merknad", "mappe"))
                _currentArchivePart.NumberOfCommentsInFolder++;

            if (eventArgs.Path.Matches("merknad", "dokumentbeskrivelse"))
                _currentArchivePart.NumberOfCommentsInDocumentDescription++;

            if (eventArgs.Path.Matches("merknad", "registrering"))
            {
                if (_baseRegistrationAttributeIsFound)
                    _currentArchivePart.NumberOfCommentsInBaseRegistration++;
            }
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (Noark5TestHelper.IdentifiesBaseRegistrationInRegistration(eventArgs))
                _baseRegistrationAttributeIsFound = true;
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
            // TODO: Handle non-baseregistration-type subregistrations?
            if (eventArgs.NameEquals("registrering"))
                _baseRegistrationAttributeIsFound = false;
        }

        private class ArchivePart
        {
            public string SystemId { get; set; }
            public int NumberOfCommentsInFolder { get; set; }
            public int NumberOfCommentsInDocumentDescription { get; set; }
            public int NumberOfCommentsInBaseRegistration { get; set; }
        }
    }
}