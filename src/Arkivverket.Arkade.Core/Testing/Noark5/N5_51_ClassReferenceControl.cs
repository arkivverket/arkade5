using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_51_ClassReferenceControl : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 51);

        private ArchivePart _currentArchivePart = new();
        private readonly Dictionary<ArchivePart, List<Folder>> _classReferringDossiersPerArchivePart = new();
        private readonly List<string> _classSystemIds = new();
        private readonly Stack<Folder> _folders = new();

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
            bool multipleArchiveParts = _classReferringDossiersPerArchivePart.Count > 1;

            var testResultSet = new TestResultSet();

            var totalNumberOfInvalidClassReferences = 0;

            foreach ((ArchivePart archivePart, List<Folder> classReferringDossiers) in _classReferringDossiersPerArchivePart)
            {
                int numberOfInvalidClassReferences = classReferringDossiers.Count(HasInvalidReference);
                var testResults = new List<TestResult>();

                testResults.AddRange
                (classReferringDossiers.Where(HasInvalidReference).Select
                    (dossierWithInvalidReference => new TestResult(ResultType.Error, new Location(
                            ArkadeConstants.ArkivuttrekkXmlFileName, dossierWithInvalidReference.XmlLineNumber),
                        string.Format(Noark5Messages.ClassReferenceControlMessage,
                            dossierWithInvalidReference.SystemId ?? "?", dossierWithInvalidReference.ClassReference))
                    )
                );

                totalNumberOfInvalidClassReferences += numberOfInvalidClassReferences;

                if (multipleArchiveParts)
                {
                    if (numberOfInvalidClassReferences > 0)
                        testResults.Insert(0, new TestResult(ResultType.Error, new Location(string.Empty),
                            string.Format(Noark5Messages.NumberOf, numberOfInvalidClassReferences)));

                    testResultSet.TestResultSets.Add(new TestResultSet
                    {
                        Name = archivePart.ToString(),
                        TestsResults = testResults,
                    });
                }
                else
                    testResultSet.TestsResults.AddRange(testResults);
            }

            if (totalNumberOfInvalidClassReferences > 0)
                testResultSet.TestsResults.Insert(0, new TestResult(ResultType.Error, new Location(string.Empty),
                    string.Format(Noark5Messages.TotalResultNumber, totalNumberOfInvalidClassReferences)));

            return testResultSet;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Name.Equals("mappe"))
                _folders.Push(new Folder {XmlLineNumber = eventArgs.LineNumber});
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Name.Equals("xsi:type") && eventArgs.Value.Equals("saksmappe"))
                _folders.Peek().IsDossier = true;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("tittel", "arkivdel"))
                _currentArchivePart.Name = eventArgs.Value;

            if (eventArgs.Path.Matches("systemID"))
            {
                if (eventArgs.Path.GetParent().Equals("klasse"))
                    _classSystemIds.Add(eventArgs.Value);

                if (eventArgs.Path.GetParent().Equals("mappe"))
                    _folders.Peek().SystemId = eventArgs.Value;

                if (eventArgs.Path.GetParent().Equals("arkivdel"))
                    _currentArchivePart.SystemId = eventArgs.Value;
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
                {
                    if (_classReferringDossiersPerArchivePart.ContainsKey(_currentArchivePart))
                        _classReferringDossiersPerArchivePart[_currentArchivePart].Add(examinedFolder);
                    else
                        _classReferringDossiersPerArchivePart.Add(_currentArchivePart, new List<Folder>
                        {
                            examinedFolder
                        });
                }
            }

            if (eventArgs.Name.Equals("arkivdel"))
                _currentArchivePart = new ArchivePart();
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
            public int XmlLineNumber { get; init; }
        }
    }
}
