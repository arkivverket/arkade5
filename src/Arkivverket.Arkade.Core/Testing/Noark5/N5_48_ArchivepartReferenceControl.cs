using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_48_ArchivepartReferenceControl : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 48);

        private ArchivePart _currentArchivePart = new();
        private readonly List<ArchivePart> _archiveParts = new();
        private readonly Dictionary<ArchivePart, List<Referrer>> _archivePartReferrersPerArchivePart = new();
        private readonly Stack<Referrer> _possibleReferrers = new();

        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override TestResultSet GetTestResults()
        {
            bool multipleArchiveParts = _archivePartReferrersPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet();
            var totalNumberOfInvalidReferences = 0;
            
            foreach ((ArchivePart archivePart, List<Referrer> archivePartReferrers) in
                _archivePartReferrersPerArchivePart)
            {
                int numberOfInvalidReferences = archivePartReferrers.Count(HasInvalidReference);

                var testResults = new List<TestResult>();

                testResults.AddRange
                (archivePartReferrers.Where(HasInvalidReference).Select
                    (referrer => new TestResult
                        (
                            ResultType.Error,
                            new Location(ArkadeConstants.ArkivuttrekkXmlFileName, referrer.XmlLineNumber),
                            string.Format(Noark5Messages.ArchivepartReferenceControlMessage,
                                referrer.Element, referrer.SystemId ?? "?", referrer.Reference)
                        )
                    )
                );

                totalNumberOfInvalidReferences += numberOfInvalidReferences;

                if (multipleArchiveParts)
                {
                    if (numberOfInvalidReferences > 0)
                        testResults.Insert(0, new TestResult(ResultType.Error, new Location(string.Empty),
                            string.Format(Noark5Messages.NumberOf, numberOfInvalidReferences)));

                    testResultSet.TestResultSets.Add(new TestResultSet
                    {
                        Name = archivePart.ToString(),
                        TestsResults = testResults
                    });
                }
                else
                    testResultSet.TestsResults.AddRange(testResults);
            }

            if (totalNumberOfInvalidReferences > 0)
                testResultSet.TestsResults.Insert(0, new TestResult(ResultType.Error, new Location(string.Empty),
                    string.Format(Noark5Messages.TotalResultNumber, totalNumberOfInvalidReferences)));

            return testResultSet;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (IsPossibleReferrer(eventArgs.Name))
                _possibleReferrers.Push(new Referrer {Element = eventArgs.Name, XmlLineNumber = eventArgs.LineNumber});
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
                    _currentArchivePart.SystemId = eventArgs.Value;

                if (IsPossibleReferrer(elementWithSystemId))
                    _possibleReferrers.Peek().SystemId = systemId;
            }

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;

            if (eventArgs.Path.Matches("referanseArkivdel"))
            {
                var elementWithReference = eventArgs.Path.GetParent();

                if (IsPossibleReferrer(elementWithReference))
                    _possibleReferrers.Peek().Reference = eventArgs.Value;
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (IsPossibleReferrer(eventArgs.Name))
            {
                Referrer examinedReferrer = _possibleReferrers.Pop();

                if (examinedReferrer.Reference != null)
                {
                    if (_archivePartReferrersPerArchivePart.ContainsKey(_currentArchivePart))
                        _archivePartReferrersPerArchivePart[_currentArchivePart].Add(examinedReferrer);
                    else
                        _archivePartReferrersPerArchivePart.Add(_currentArchivePart, new List<Referrer>
                        {
                            examinedReferrer
                        });
                }
            }

            if (eventArgs.NameEquals("arkivdel"))
            {
                _archiveParts.Add(_currentArchivePart);
                _currentArchivePart = new ArchivePart();
            }
        }

        private static bool IsPossibleReferrer(string elementName)
        {
            return elementName.Equals("mappe") ||
                   elementName.Equals("registrering") ||
                   elementName.Equals("dokumentbeskrivelse");
        }

        private bool HasInvalidReference(Referrer archivepartReferrer)
        {
            ArchivePart archivePart = _archiveParts.Find(a => a.SystemId.Equals(archivepartReferrer.Reference));
            return archivePart == null || !_archivePartReferrersPerArchivePart.ContainsKey(archivePart);
        }

        private class Referrer
        {
            public string Element { get; set; }
            public string SystemId { get; set; }
            public string Reference { get; set; }
            public int XmlLineNumber { get; init; }
        }
    }
}
