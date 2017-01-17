using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using System.Collections.Generic;
using System.Linq;


namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #09
    /// </summary>
    public class NumberOfFoldersClassified : Noark5XmlReaderBaseTest
    {
        private readonly List<Klasse> _foldersByKlasse = new List<Klasse>();
        private string _currentKlasseSystemId;
        private string _currentKlasseTitle;
        private int _currentNumberOfFolders;

        public override string GetName()
        {
            return Noark5Messages.NumberOfFoldersClassified;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            foreach (var foldersAtKlasse in _foldersByKlasse)
            {
                testResults.Add(new TestResult(ResultType.Success, new Location(""), "Klasse sin systemId: " + foldersAtKlasse.SystemId + ", tittel: " + foldersAtKlasse.Title + ": " + foldersAtKlasse.NumberOfFolders));
            }

            return testResults;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "klasse"))
            {
                _currentKlasseSystemId = eventArgs.Value;
            }
            if (eventArgs.Path.Matches("tittel", "klasse"))
            {
                _currentKlasseTitle = eventArgs.Value;
            }
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("mappe", "klasse"))
            {
                _currentNumberOfFolders++;
            }
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.NameEquals("arkivdel"))
            {
                _foldersByKlasse.Add(new Klasse { SystemId = _currentKlasseSystemId, Title =_currentKlasseTitle, NumberOfFolders = _currentNumberOfFolders });

                _currentKlasseSystemId = null;
                _currentKlasseTitle = null;
                _currentNumberOfFolders = 0;
            }
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }
    }

    class Klasse
    {
        public string SystemId { get; set; }
        public string Title { get; set; }
        public int NumberOfFolders { get; set; }
    }
}
