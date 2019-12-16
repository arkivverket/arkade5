using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_15_NumberOfEachCaseFolderStatus : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 15);

        private string _currentArchivePartSystemId;
        private string _currentArchiveTitle;
        private CaseFolderInArchivePart _currentCaseFolder; // TODO: Support nested case folders?
        private readonly List<CaseFolderInArchivePart> _folders = new List<CaseFolderInArchivePart>();

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

            var folderQuery = from folder in _folders
                group folder by new
                {
                    folder.SystemId,
                    folder.Name,
                    Status = folder.CaseStatus
                }
                into grouped
                select new
                {
                    grouped.Key.SystemId,
                    grouped.Key.Name,
                    grouped.Key.Status,
                    Count = grouped.Count()
                };

            int numberOfUniqueCaseFolderStatuses = _folders.GroupBy(x => x.CaseStatus).Count();

            bool multipleArchiveParts = _folders.GroupBy(j => j.SystemId).Count() > 1;

            foreach (var item in folderQuery)
            {
                var message = new StringBuilder(
                    string.Format(Noark5Messages.NumberOfEachCaseFolderStatusMessage, item.Status, item.Count));

                if (multipleArchiveParts)
                    message.Insert(0,
                        string.Format(Noark5Messages.ArchivePartSystemId, item.SystemId, item.Name) + " - ");

                ResultType resultType = item.Status.Equals("Avsluttet") || item.Status.Equals("Utgår")
                    ? ResultType.Success
                    : ResultType.Error;

                testResults.Add(new TestResult(resultType, new Location(string.Empty), message.ToString()));
            }

            testResults.Insert(0, new TestResult(ResultType.Success, new Location(""), string.Format(Noark5Messages.TotalResultNumber,
                numberOfUniqueCaseFolderStatuses)));

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (Noark5TestHelper.IdentifiesCasefolder(eventArgs))
                _currentCaseFolder = new CaseFolderInArchivePart { SystemId = _currentArchivePartSystemId, Name = _currentArchiveTitle};
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _currentArchivePartSystemId = eventArgs.Value;

            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchiveTitle = eventArgs.Value;

            if (eventArgs.Path.Matches("saksstatus", "mappe") && _currentCaseFolder != null)
                _currentCaseFolder.CaseStatus = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            // TODO: Handle non-casefolder-type subfolders?

            if (eventArgs.NameEquals("mappe") && _currentCaseFolder != null)
            {
                _folders.Add(_currentCaseFolder);
                _currentCaseFolder = null;
            }
        }

        private class CaseFolderInArchivePart : ArchivePart
        {
            public string CaseStatus { get; set; }
        }
    }
}
