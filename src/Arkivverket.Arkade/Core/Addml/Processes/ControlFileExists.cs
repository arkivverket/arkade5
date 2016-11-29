using System.Collections.Generic;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;
using System.IO;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlFileExists : AddmlProcess
    {
        public const string Name = "Control_FileExists";

        private readonly List<TestResult> _testResults = new List<TestResult>();

        public override string GetName()
        {
            return Name;
        }

        public override string GetDescription()
        {
            return Messages.ControlFileExistsDescription;
        }

        public override TestType GetTestType()
        {
            return TestType.ContentControl;
        }

        protected override List<TestResult> GetTestResults()
        {
            return _testResults;
        }

        protected override void DoRun(FlatFile flatFile)
        {
            FileInfo file = flatFile.Definition.FileInfo;
            if (file != null)
            {
                if (!file.Exists)
                {
                    _testResults.Add(new TestResult(ResultType.Error, AddmlLocation.FromFlatFileIndex(flatFile.Definition.GetIndex()),
                               string.Format(Messages.ControlFileExistsMessage, file.FullName)));
                }
            }
        }

        protected override void DoRun(Record record)
        {
        }

        protected override void DoRun(Field field)
        {
        }

        protected override void DoEndOfFile()
        {
        }

    }
}