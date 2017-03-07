using System.Collections.Generic;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #50
    /// </summary>
    public class ClassReferenceControl : Noark5XmlReaderBaseTest
    {
        private readonly List<string> _classSystemIds = new List<string>();
        private readonly Stack<Folder> _folders = new Stack<Folder>();
        private readonly List<Folder> _classReferringDossiers = new List<Folder>();

        public override string GetName()
        {
            return Noark5Messages.ClassReferenceControl;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            foreach (Folder classRefferingDossier in _classReferringDossiers)
            {
                if (HasInvalidReference(classRefferingDossier))
                    testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                        string.Format(Noark5Messages.ClassReferenceControlMessage,
                            classRefferingDossier.SystemId ?? "?", classRefferingDossier.ClassReference)));
            }

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Name.Equals("mappe"))
                _folders.Push(new Folder());
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Name.Equals("xsi:type") && eventArgs.Value.Equals("saksmappe"))
                _folders.Peek().IsDossier = true;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID"))
            {
                if (eventArgs.Path.GetParent().Equals("klasse"))
                    _classSystemIds.Add(eventArgs.Value);

                if (eventArgs.Path.GetParent().Equals("mappe"))
                    _folders.Peek().SystemId = eventArgs.Value;
            }

            if (eventArgs.Path.Matches("referanseSekundaerKlassifikasjon") && eventArgs.Path.GetParent().Equals("mappe"))
                _folders.Peek().ClassReference = eventArgs.Value;
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Name.Equals("mappe"))
            {
                Folder examinedFolder = _folders.Pop();

                if (examinedFolder.IsDossier && examinedFolder.ClassReference != null)
                    _classReferringDossiers.Add(examinedFolder);
            }
        }

        private bool HasInvalidReference(Folder folder)
        {
            return !_classSystemIds.Contains(folder.ClassReference);
        }

        private class Folder
        {
            public string SystemId { get; set; }
            public bool IsDossier { get; set; }
            public string ClassReference { get; set; }
        }
    }
}
