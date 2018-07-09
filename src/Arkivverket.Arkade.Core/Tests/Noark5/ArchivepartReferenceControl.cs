using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Tests.Noark5
{
    public class ArchivepartReferenceControl : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 48);

        private readonly List<string> _archivepartSystemIds = new List<string>();
        private readonly Stack<Referrer> _possibleReferrers = new Stack<Referrer>();
        private readonly List<Referrer> _archivepartReferrers = new List<Referrer>();

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5TestNames.ArchivepartReferenceControl;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            foreach (Referrer referrer in _archivepartReferrers)
            {
                if (HasInvalidReference(referrer))
                    testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                        string.Format(Noark5Messages.ArchivepartReferenceControlMessage,
                            referrer.Element, referrer.SystemId ?? "?", referrer.Reference)));
            }

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (IsPossibleRefferer(eventArgs.Name))
                _possibleReferrers.Push(new Referrer { Element = eventArgs.Name });
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID"))
            {
                var elementWithSystemId = eventArgs.Path.GetParent();
                var systemId = eventArgs.Value;

                if (elementWithSystemId.Equals("arkivdel"))
                    _archivepartSystemIds.Add(systemId);

                if (IsPossibleRefferer(elementWithSystemId))
                    _possibleReferrers.Peek().SystemId = systemId;
            }

            if (eventArgs.Path.Matches("referanseArkivdel"))
            {
                var elementWithReference = eventArgs.Path.GetParent();

                if (IsPossibleRefferer(elementWithReference))
                {
                    var reference = eventArgs.Value;
                    _possibleReferrers.Peek().Reference = reference;
                }
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (IsPossibleRefferer(eventArgs.Name))
            {
                Referrer examinedReferrer = _possibleReferrers.Pop();

                if (examinedReferrer.Reference != null)
                    _archivepartReferrers.Add(examinedReferrer);
            }
        }

        private static bool IsPossibleRefferer(string elementName)
        {
            return elementName.Equals("mappe") ||
                   elementName.Equals("registrering") ||
                   elementName.Equals("dokumentbeskrivelse");
        }

        private bool HasInvalidReference(Referrer archivepartReferrer)
        {
            return !_archivepartSystemIds.Contains(archivepartReferrer.Reference);
        }

        private class Referrer
        {
            public string Element { get; set; }
            public string SystemId { get; set; }
            public string Reference { get; set; }
        }
    }
}
