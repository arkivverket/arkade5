using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #35
    /// </summary>
    public class NumberOfDisposalResolutions : Noark5XmlReaderBaseTest
    {
        private readonly List<DisposalResolution> _disposalResolutions = new List<DisposalResolution>();
        private string _currentArchivePartSystemId;

        public override string GetName()
        {
            return Noark5Messages.NumberOfDisposalResolutions;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            // Group disposal resolutions by parent element name and by archive part:
            var disposalResolutionQuery = from disposalResolution in _disposalResolutions
                group disposalResolution by new
                {
                    disposalResolution.ArchivePartSystemId,
                    disposalResolution.ParentElementName
                }
                into grouped
                select new
                {
                    grouped.Key.ArchivePartSystemId,
                    grouped.Key.ParentElementName,
                    Count = grouped.Count()
                };

            bool multipleArchiveParts = _disposalResolutions.GroupBy(c => c.ArchivePartSystemId).Count() > 1;

            foreach (var item in disposalResolutionQuery)
            {
                var message = new StringBuilder(
                    string.Format(Noark5Messages.NumberOfDisposalResolutionsMessage, item.ParentElementName, item.Count));

                if (multipleArchiveParts)
                    message.Insert(0,
                        string.Format(Noark5Messages.ArchivePartSystemId, item.ArchivePartSystemId) + " - ");

                testResults.Add(new TestResult(ResultType.Success, new Location(""), message.ToString()));
            }

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("kassasjon"))
            {
                _disposalResolutions.Add(new DisposalResolution
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

        private class DisposalResolution
        {
            public string ArchivePartSystemId { get; set; }
            public string ParentElementName { get; set; }
        }
    }
}
