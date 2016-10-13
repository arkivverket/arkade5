using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core
{
    public class AddmlDatasetTestEngineTest
    {
        [Fact]
        public void ShouldReturnTestSuiteFromTests()
        {
            var addmlDefinition = new AddmlDefinition();
            var testSuite = new AddmlDatasetTestEngine(addmlDefinition).RunTests();
            testSuite.Should().NotBeNull();
        }
    }
}