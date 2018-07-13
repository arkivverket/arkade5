using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Tests.Base
{
    public class ArkadeTestMock : IArkadeTest
    {
        private readonly TestId _testId;
        private readonly string _name;
        private readonly TestType _testType;
        private readonly string _description;

        public ArkadeTestMock(string name, TestType testType, string description = null) :
            this(new TestId(TestId.TestKind.Unidentified, 0), name, testType, description)
        {
        }

        public ArkadeTestMock(TestId testId, string name, TestType testType, string description = null)
        {
            _testId = testId;
            _name = name;
            _testType = testType;
            _description = description;
        }

        public TestId GetId()
        {
            return _testId;
        }

        public string GetName()
        {
            return _name;
        }

        public TestType GetTestType()
        {
            return _testType;
        }

        public string GetDescription()
        {
            return _description;
        }

        public TestRun GetTestRun()
        {
            return new TestRun(this);
        }

        public int CompareTo(object obj)
        {
            var arkadeTest = (IArkadeTest) obj;

            return GetId().CompareTo(arkadeTest.GetId());
        }
    }
}
