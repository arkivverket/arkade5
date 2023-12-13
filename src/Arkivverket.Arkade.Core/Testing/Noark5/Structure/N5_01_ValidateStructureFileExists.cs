using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Testing.Noark5.Structure
{
    public class N5_01_ValidateStructureFileExists : Noark5StructureBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 1);
        public const TestType Type = TestType.StructureControl;
        private readonly List<TestResult> _testResults = new List<TestResult>();

        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return Type;
        }

        protected override TestResultSet GetTestResults()
        {
            return new()
            {
                TestsResults = _testResults
            };
        }

        public override void Test(Archive archive)
        {
            foreach (ArchiveXmlUnit xmlUnit in archive.XmlUnits)
            {
                foreach (string missingFile in xmlUnit.GetMissingFiles())
                    _testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                        string.Format(Noark5Messages.ValidateStructureFileExists_FileMissing, missingFile)));
            }
        }
    }
}
