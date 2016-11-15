using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core
{
    public class TestSuiteTest
    {

        [Fact]
        public void ShouldReturnOneError()
        {
            var testSuite = new TestSuite();
            var testRun = new TestRun("test with error", TestType.Content);
            testRun.Add(new TestResult(ResultType.Error, new Location(""), "feil"));
            testSuite.AddTestRun(testRun);

            testSuite.FindNumberOfErrors().Should().Be(1);

        }

    }
}
