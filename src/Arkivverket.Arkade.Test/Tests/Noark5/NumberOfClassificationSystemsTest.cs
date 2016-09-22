using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfClassificationSystemsTest
    {
       
        private TestResults RunTest(Stream content)
        {
            return new NumberOfClassificationSystems(new ArchiveContentMemoryStreamReader(content)).RunTest(new ArchiveExtraction("123", ""));
        }

        [Fact]
        public void NumberOfClassificationSystemsIsOne()
        {
            var archiveBuilder = new ArchiveBuilder().WithArchivePart().WithClassificationSystem();

            using (Stream archiveContent = archiveBuilder.Build())
            {
                TestResults testResults = RunTest(archiveContent);
                testResults.AnalysisResults[NumberOfClassificationSystems.AnalysisKeyClassificationSystems].Should().Be("1");
            }
        }

        [Fact]
        public void NumberOfClassificationSystemsIsTwo()
        {
            var archiveBuilder = new ArchiveBuilder()
                .WithArchivePart("part 1").WithClassificationSystem("classification system 1")
                .WithArchivePart("part 2").WithClassificationSystem("classification system 2");

            using (Stream archiveContent = archiveBuilder.Build())
            {
                TestResults testResults = RunTest(archiveContent);
                testResults.AnalysisResults[NumberOfClassificationSystems.AnalysisKeyClassificationSystems].Should().Be("2");
            }
        }
    }
}