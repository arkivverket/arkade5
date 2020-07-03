using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Util
{
    public class TestIdTests
    {
        [Fact]
        public void CreateTest()
        {
            TestId testId = TestId.Create("N5.15");

            testId.Kind.Should().Be(TestId.TestKind.Noark5);
            testId.Number.Should().Be(15);
        }

        [Fact]
        public void ToStringTest()
        {
            var testId = new TestId(TestId.TestKind.Noark5, 15);

            testId.ToString().Should().Be("N5.15");
        }

        [Fact]
        public void CompareToTest()
        {
            var testId16 = new TestId(TestId.TestKind.Noark5, 16);
            var testId15 = new TestId(TestId.TestKind.Noark5, 15);

            var testIds = new List<TestId>
            {
                testId16,
                testId15
            };

            testIds.Sort();

            testIds.First().Should().Be(testId15);
        }

        [Fact]
        public void EqualsTest()
        {
            var testIdA = new TestId(TestId.TestKind.Noark5, 15);
            var testIdB = new TestId(TestId.TestKind.Noark5, 15);

            testIdA.Should().BeEquivalentTo(testIdB);
        }
    }
}
