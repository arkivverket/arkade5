using System;
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
        private readonly Archive _archive;

        public NumberOfChangesLogged(Archive archive)
        {
            _archive = archive;
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
            var testResults = new List<TestResult>();

            try
            {
                var changeLog = SerializeUtil.DeserializeFromFile<endringslogg>(
                    _archive.WorkingDirectory.Content().WithFile(ArkadeConstants.ChangeLogXmlFileName).FullName
                );

                int numberOfChangesLogged = changeLog.endring.Length;

                testResults.Add(new TestResult(ResultType.Success, new Location(ArkadeConstants.ChangeLogXmlFileName),
                    string.Format(Noark5Messages.NumberOfChangesLoggedMessage, numberOfChangesLogged)));
            }
            catch (Exception)
            {
                testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                    string.Format(Noark5Messages.FileNotFound, ArkadeConstants.ChangeLogXmlFileName)));
            }

            return testResults;
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
