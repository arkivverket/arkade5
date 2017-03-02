using System.Collections.Generic;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.ExternalModels.ChangeLog;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    ///     Noark5 - test #42
    /// </summary>
    public class NumberOfChangesLogged : Noark5XmlReaderBaseTest
    {
        private readonly endringslogg _changeLog;

        public NumberOfChangesLogged(Archive archive)
        {
            _changeLog = SerializeUtil.DeserializeFromFile<endringslogg>(
                archive.WorkingDirectory.Content().WithFile(ArkadeConstants.ChangeLogXmlFileName).FullName
            );
        }

        public override string GetName()
        {
            return Noark5Messages.NumberOfChangesLogged;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            int numberOfChangesLogged = _changeLog.endring.Length;

            return new List<TestResult>
            {
                new TestResult(ResultType.Success, new Location(ArkadeConstants.ChangeLogXmlFileName),
                    string.Format(Noark5Messages.NumberOfChangesLoggedMessage, numberOfChangesLogged))
            };
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

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }
    }
}
