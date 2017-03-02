using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.ExternalModels.ChangeLog;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #51
    /// </summary>
    public class ChangeLogArchiveReferenceControl : Noark5XmlReaderBaseTest
    {
        private readonly endringslogg _changeLog;
        private readonly List<string> _systemIDs;

        public ChangeLogArchiveReferenceControl(Archive archive)
        {
            _changeLog = SerializeUtil.DeserializeFromFile<endringslogg>(
                archive.WorkingDirectory.Content().WithFile(ArkadeConstants.ChangeLogXmlFileName).FullName
            );

            _systemIDs = new List<string>();
        }

        public override string GetName()
        {
            return Noark5Messages.ChangeLogArchiveReferenceControl;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            var changeLogChanges = _changeLog.endring;

            foreach (var change in changeLogChanges)
            {
                if (!_systemIDs.Contains(change.referanseArkivenhet))
                    testResults.Add(new TestResult(ResultType.Error, new Location(ArkadeConstants.ChangeLogXmlFileName),
                        string.Format(Noark5Messages.ChangeLogArchiveReferenceControlMessage, change.referanseArkivenhet)));
            }

            return testResults;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID"))
                _systemIDs.Add(eventArgs.Value);
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }
    }
}
