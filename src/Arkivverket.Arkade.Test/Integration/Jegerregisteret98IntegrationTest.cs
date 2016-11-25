using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml.Processes;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Integration
{
    public class Jegerregisteret98IntegrationTest
    {
        [Fact]
        public void ShouldReadSmallVersionOfJegerregisteret98()
        {
            ArchiveFile archive = ArchiveFile.Read("..\\..\\TestData\\tar\\jegerregisteret98-small\\20b5f34c-4411-47c3-a0f9-0a8bca631603.tar");
            Arkade.Core.Arkade arkade = new Arkade.Core.Arkade();
            TestSession testSesson = arkade.RunTests(archive);

            testSesson.Should().NotBeNull();
            TestSuite testSuite = testSesson.TestSuite;
            testSuite.Should().NotBeNull();

            /* TODO Add some tests
            testSuite.TestRuns.Should().NotBeNullOrEmpty();

            List<TestRun> analyseFindMinMaxValues = testSuite.TestRuns
                .Where(run => run.TestName == AnalyseFindMinMaxValues.Name)
                .ToList();
            analyseFindMinMaxValues.Count.Should().Be(1);
            */
        }

    }
}