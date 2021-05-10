using System;
using Arkivverket.Arkade.Core.Testing;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base
{
    public class TestRun : IComparable
    {
        private readonly IArkadeTest _test;
        public TestId TestId => _test.GetId();
        public string TestName => ArkadeTestNameProvider.GetDisplayName(_test);
        public TestType TestType => _test.GetTestType();
        public string TestDescription => _test.GetDescription();
        public List<TestResult> Results { get; set; }
        public long TestDuration { get; set; }

        public TestRun(IArkadeTest test)
        {
            _test = test;

            Results = new List<TestResult>();
        }

        public bool IsSuccess()
        {
            return Results.TrueForAll(r => !r.IsError());
        }

        public int FindNumberOfErrors()
        {
            return Results.Count(r => r.IsError()) + Results.Where(r => r.IsErrorGroup()).Sum(r => r.GroupErrors);
        }

        public int CompareTo(object obj)
        {
            var testRun = (TestRun) obj;

            return _test.CompareTo(testRun._test);
        }
    }
}
