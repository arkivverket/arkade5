using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    /// <inheritdoc />
    public class NumberOfDepreciations : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 40);

        private int _totalNumberOfDeprecations;
        private readonly List<Archivepart> _archiveParts = new List<Archivepart>();
        private Archivepart _currentArchivePart;

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

            testResults.Add(new TestResult(ResultType.Success, new Location(""),
                string.Format(Noark5Messages.TotalResultNumber, _totalNumberOfDeprecations.ToString())));

            foreach (Archivepart archivePart in _archiveParts)
            {
                foreach (KeyValuePair<string, int> pair in archivePart.TypeOfDepreciation)
                {
                    var numberOf = pair.Value;
                    var type = pair.Key;

                    if (_archiveParts.Count == 1)
                    {
                        testResults.Add(new TestResult(ResultType.Success, new Location(""),
                            $"{type}: {numberOf}"));

                    }
                    else
                    {
                        var testresult = new TestResult(ResultType.Success, new Location(string.Empty),
                            string.Format(Noark5Messages.NumberOfDepreciationsMessage_ForArchivePart,
                                archivePart.SystemId,
                                type,
                                numberOf));

                        testResults.Add(testresult);
                    }
                }
            }
            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
            {
                _currentArchivePart = new Archivepart { SystemId = eventArgs.Value };
                _archiveParts.Add(_currentArchivePart);
            }

            if (eventArgs.Path.Matches("avskrivningsmaate", "avskrivning"))
            {
                _totalNumberOfDeprecations++;

                string type = eventArgs.Value;

                if (_currentArchivePart.TypeOfDepreciation.ContainsKey(type))
                {
                    _currentArchivePart.TypeOfDepreciation[type]++;
                }
                else
                {
                    _currentArchivePart.TypeOfDepreciation.Add(type, 1);
                }
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        private class Archivepart
        {
            public string SystemId { get; set; }
            public readonly Dictionary<string, int> TypeOfDepreciation = new Dictionary<string, int>();
        }
    }
}
