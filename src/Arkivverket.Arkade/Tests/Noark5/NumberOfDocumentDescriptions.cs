using System.Collections.Generic;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #18
    /// </summary>
    public class NumberOfDocumentDescriptions : Noark5XmlReaderBaseTest
    {
        private string _currentArchivePartSystemId;
        private int _totalnumberOfDocumentDescriptions;
        private readonly Dictionary<string, int> _documentDescriptionsPerArchivePart = new Dictionary<string, int>();

        public override string GetName()
        {
            return Noark5Messages.NumberOfDocumentDescriptions;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>
            {
                new TestResult(ResultType.Success, new Location(string.Empty),
                    _totalnumberOfDocumentDescriptions.ToString())
            };

            if (_documentDescriptionsPerArchivePart.Count > 1)
            {
                foreach (KeyValuePair<string, int> documentDescriptionCount in _documentDescriptionsPerArchivePart)
                {
                    var testResult = new TestResult(ResultType.Success, new Location(string.Empty),
                        string.Format(Noark5Messages.NumberOfDocumentDescriptionsPerArchivePartMessage,
                            documentDescriptionCount.Key, documentDescriptionCount.Value));

                    testResults.Add(testResult);
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
                _currentArchivePartSystemId = eventArgs.Value;
                _documentDescriptionsPerArchivePart.Add(_currentArchivePartSystemId, 0);
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("dokumentbeskrivelse"))
            {
                _totalnumberOfDocumentDescriptions++;

                if (_documentDescriptionsPerArchivePart.Count >0)
                {
                    if (_documentDescriptionsPerArchivePart.ContainsKey(_currentArchivePartSystemId))
                        _documentDescriptionsPerArchivePart[_currentArchivePartSystemId]++;
                }
              
            }
        }
    }
}