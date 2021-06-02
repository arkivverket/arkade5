using System;
using System.Collections.Generic;
using System.Xml;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5
{
    public class N5_62_ChangeLogArchiveReferenceControl : Noark5XmlReaderBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 62);

        private readonly Archive _archive;
        private readonly HashSet<string> _systemIDs;

        public N5_62_ChangeLogArchiveReferenceControl(Archive archive)
        {
            _archive = archive;
            _systemIDs = new HashSet<string>();
        }
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
            var testResultSet = new TestResultSet();

            string changelogFullFilename = _archive.WorkingDirectory.Content()
                .WithFile(ArkadeConstants.ChangeLogXmlFileName).FullName;

            try
            {
                var logReader = new XmlTextReader(changelogFullFilename);

                while (logReader.Read())
                {
                    if (logReader.Name.Equals("referanseArkivenhet") && logReader.IsStartElement())
                    {
                        logReader.Read(); // Move to textnode containing the actual reference (systemID)

                        string loggedSystemId = logReader.Value;

                        if (!_systemIDs.Contains(loggedSystemId))
                        {
                            testResultSet.TestsResults.Add(new TestResult(ResultType.Error,
                                new Location(ArkadeConstants.ChangeLogXmlFileName),
                                string.Format(Noark5Messages.ChangeLogArchiveReferenceControlMessage, loggedSystemId))
                            );
                        }
                    }
                }

                logReader.Close();
            }
            catch (Exception)
            {
                testResultSet.TestsResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                    string.Format(Noark5Messages.FileNotFound, ArkadeConstants.ChangeLogXmlFileName)));
            }

            return testResultSet;
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
