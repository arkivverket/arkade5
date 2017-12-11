using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfConversions : Noark5XmlReaderBaseTest
    {
        private readonly Dictionary<string, int> _numberOfConvertionsPerArchivePart = new Dictionary<string, int>();

        public override string GetName()
        {
            return Noark5Messages.NumberOfConversions;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            var testResults = new List<TestResult>();

            foreach (var archivePartConvertionsCount in _numberOfConvertionsPerArchivePart)
            {
                if (archivePartConvertionsCount.Value == 0)
                    continue;

                var message = new StringBuilder(
                    string.Format(Noark5Messages.NumberOfConversionsMessage, archivePartConvertionsCount.Value)
                );

                if (_numberOfConvertionsPerArchivePart.Keys.Count > 1) // Multiple archiveparts
                    message.Insert(0,
                        string.Format(Noark5Messages.ArchivePartSystemId, archivePartConvertionsCount.Key) + " - ");

                testResults.Add(new TestResult(ResultType.Success, new Location(""), message.ToString()));
            }

            return testResults;
        }

        protected override void ReadStartElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("konvertering", "dokumentobjekt"))
                _numberOfConvertionsPerArchivePart[_numberOfConvertionsPerArchivePart.Keys.Last()]++;
        }

        protected override void ReadElementValueEvent(object sender, ReadElementEventArgs eventArgs)
        {
            if (eventArgs.Path.Matches("systemID", "arkivdel"))
                _numberOfConvertionsPerArchivePart[eventArgs.Value] = 0;
        }

        protected override void ReadAttributeEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }

        protected override void ReadEndElementEvent(object sender, ReadElementEventArgs eventArgs)
        {
        }
    }
}
