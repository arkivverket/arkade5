using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;
using System.Collections.Generic;
using System.Linq;

namespace Arkivverket.Arkade.Core.Testing.Noark5.Structure
{
    public class N5_01_ValidateStructureFileExists : Noark5StructureBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 1);
        private readonly List<TestResult> _testResults = new List<TestResult>();

        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.StructureControl;
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
            foreach (KeyValuePair<string, IEnumerable<string>> documentedXmlUnit in archive.Details.DocumentedXmlUnits)
            {
                foreach (string documentedXmlFile in documentedXmlUnit.Value.Concat([documentedXmlUnit.Key]))
                {
                    if (!archive.WorkingDirectory.Content().WithFile(documentedXmlFile).Exists)
                    {
                        _testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                            string.Format(Noark5Messages.ValidateStructureFileExists_FileMissing, documentedXmlFile)));
                    }
                }
            }
        }
    }
}
