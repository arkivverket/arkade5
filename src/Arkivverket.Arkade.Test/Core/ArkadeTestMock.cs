using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Test.Core
{
    public class ArkadeTestMock : IArkadeTest
    {
        private readonly TestId _testId;
        private readonly string _name;
        private readonly TestType _testType;
        private readonly string _description;

        public ArkadeTestMock(string name, TestType testType, string description = null) :
            this(new TestId(TestId.TestKind.Other, 0), name, testType, description)
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
    }
}
