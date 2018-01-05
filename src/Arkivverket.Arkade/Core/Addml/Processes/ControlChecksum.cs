using System.Collections.Generic;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Tests;
using System.IO;
using System.Security.Cryptography;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core.Addml.Processes
{
    public class ControlChecksum : AddmlProcess
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Addml, 0); // TODO: Assign correct test number

        public const string Name = "Control_Checksum";

        private readonly List<TestResult> _testResults = new List<TestResult>();

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Name;
        }

        public override string GetDescription()
        {
            return Messages.ControlChecksumDescription;
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
            if (flatFile.Definition.Checksum == null)
            {
                _testResults.Add(new TestResult(ResultType.Error, AddmlLocation.FromFlatFileIndex(flatFile.Definition.GetIndex()),
                           string.Format(Messages.ControlChecksumMessage3)));
                return;
            }

            string checksumAlgorithm = flatFile.Definition.Checksum.Algorithm;
            string expectedChecksum = flatFile.Definition.Checksum.Value;
            FileInfo file = flatFile.Definition.FileInfo;

            HashAlgorithm h = HashAlgorithm.Create(checksumAlgorithm);
            if (h == null)
            {
                _testResults.Add(new TestResult(ResultType.Error, AddmlLocation.FromFlatFileIndex(flatFile.Definition.GetIndex()),
                           string.Format(Messages.ControlChecksumMessage1, checksumAlgorithm)));
                return;
            }

            byte[] bytes;
            using (FileStream fs = file.OpenRead())
            {
                bytes = h.ComputeHash(fs);
            }
            string actualChecksum = Hex.ToHexString(bytes);

            if (expectedChecksum != actualChecksum)
            {
                _testResults.Add(new TestResult(ResultType.Error, AddmlLocation.FromFlatFileIndex(flatFile.Definition.GetIndex()),
                           string.Format(Messages.ControlChecksumMessage2, expectedChecksum, actualChecksum)));
            }
        }

        protected override void DoRun(Record record)
        {
        }

        protected override void DoEndOfFile()
        {
        }

        protected override void DoRun(Field field)
        {
        }
    }
}