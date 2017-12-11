using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfClassifications : Noark5XmlReaderBaseTest
    {
        private readonly List<Classification> _classifications = new List<Classification>();
        private string _currentArchivePartSystemId;

        public override string GetName()
        {
            return Noark5Messages.NumberOfClassifications;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            // Group classifications by parent element name and by archive part:
            var classificationQuery = from classification in _classifications
                group classification by new
                {
                    classification.ArchivePartSystemId,
                    classification.ParentElementName
                }
                into grouped
                select new
                {
                    grouped.Key.ArchivePartSystemId,
                    grouped.Key.ParentElementName,
                    Count = grouped.Count()
                };

            bool multipleArchiveParts = _classifications.GroupBy(c => c.ArchivePartSystemId).Count() > 1;

            foreach (var item in classificationQuery)
            {
                var message = new StringBuilder(
                    string.Format(Noark5Messages.NumberOfClassificationsMessage, item.ParentElementName, item.Count));

                if (multipleArchiveParts)
                    message.Insert(0,
                        string.Format(Noark5Messages.ArchivePartSystemId, item.ArchivePartSystemId) + " - ");

                testResults.Add(new TestResult(ResultType.Success, new Location(""), message.ToString()));
            }

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("gradering") && eventArgs.Path.GetParent() != "gradering")
            {
                _classifications.Add(new Classification
                {
                    ArchivePartSystemId = _currentArchivePartSystemId,
                    ParentElementName = eventArgs.Path.GetParent()
                });
            }
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePartSystemId = eventArgs.Value;
        }

        private class Classification
        {
            public string ArchivePartSystemId { get; set; }
            public string ParentElementName { get; set; }
        }
    }
}
