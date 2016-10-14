using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Addml;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml
{
    public class AddmlDatasetTestEngineTest
    {
        [Fact]
        public void ShouldReturnTestSuiteFromTests()
        {
            var addmlDefinition = new AddmlDefinition();
            var testSession = new TestSession(new Archive(ArchiveType.Noark3, Uuid.Random(), new DirectoryInfo(@"c:\temp")));
            var addmlDatasetTestEngine = new AddmlDatasetTestEngine(new FlatFileReaderFactory(), new AddmlProcessRunner());
            TestSuite testSuite = addmlDatasetTestEngine.RunTests(addmlDefinition, testSession);
            testSuite.Should().NotBeNull();
        }
    }
}