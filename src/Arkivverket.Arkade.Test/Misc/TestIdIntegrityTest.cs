using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using Arkivverket.Arkade.Test.Tests.Noark5;
using Arkivverket.Arkade.Tests.Noark5;
using Arkivverket.Arkade.Util;
using FluentAssertions;
using Moq;
using Xunit;

namespace Arkivverket.Arkade.Test.Misc
{
    public class ArkadeTestIntegrityControl
    {
        [Fact]
        [Trait("Category", "Integration")]
        public void AllTestsAndProcessesHasAUniqueTestId()
        {
            IEnumerable<IArkadeTest> allArkadeTests = GetTests().Concat(ProcessProvider.GetAllProcesses());

            IEnumerable<TestId> testIds = allArkadeTests.Select(t => t.GetId());

            testIds.Should().OnlyHaveUniqueItems();
        }

        // TODO: Check that tests/processes are sorted

        private static IEnumerable<IArkadeTest> GetTests()
        {
            var tests = new List<IArkadeTest>();

            var noark5TestProvider = new Noark5TestProvider();

            Archive archive = TestUtil.CreateArchiveExtraction(Path.Combine("TestData", "Noark5", "Small"));

            tests.AddRange(noark5TestProvider.GetStructureTests());
            tests.AddRange(noark5TestProvider.GetContentTests(archive));

            return tests;
        }
    }
}
