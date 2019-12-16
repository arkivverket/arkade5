using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_43_NumberOfClassifications : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 43);

        private readonly List<Classification> _classifications = new List<Classification>();
        private ArchivePart _currentArchivePart = new ArchivePart();

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
            int totalNumberOfClassifications = 0;

            // Group classifications by parent element name and by archive part:
            var classificationQuery = from classification in _classifications
                group classification by new
                {
                    classification.ArchivePart.SystemId,
                    classification.ArchivePart.Name,
                    classification.ParentElementName
                }
                into grouped
                select new
                {
                    grouped.Key.SystemId,
                    grouped.Key.Name,
                    grouped.Key.ParentElementName,
                    Count = grouped.Count()
                };

            bool multipleArchiveParts = _classifications.GroupBy(c => c.ArchivePart.SystemId).Count() > 1;

            foreach (var item in classificationQuery)
            {
                var message = new StringBuilder(
                    string.Format(Noark5Messages.NumberOfClassificationsMessage, item.ParentElementName, item.Count));

                if (multipleArchiveParts)
                    message.Insert(0,
                        string.Format(Noark5Messages.ArchivePartSystemId, item.SystemId, item.Name) + " - ");

                testResults.Add(new TestResult(ResultType.Success, new Location(""), message.ToString()));

                totalNumberOfClassifications += item.Count;
            }

            testResults.Insert(0, new TestResult(ResultType.Success, new Location(""), 
                string.Format(Noark5Messages.TotalResultNumber, totalNumberOfClassifications.ToString())));

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("gradering") && eventArgs.Path.GetParent() != "gradering")
            {
                _classifications.Add(new Classification
                {
                    ArchivePart = _currentArchivePart,
                    ParentElementName = eventArgs.Path.GetParent()
                });
            }
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if(eventArgs.NameEquals("arkivdel"))
                _currentArchivePart = new ArchivePart();
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePart.SystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;
        }

        private class Classification
        {
            public ArchivePart ArchivePart { get; set; }
            public string ParentElementName { get; set; }
        }

    }
}
