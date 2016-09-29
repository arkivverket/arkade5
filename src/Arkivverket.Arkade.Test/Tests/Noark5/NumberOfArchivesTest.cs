using System;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;
using Arkivverket.Arkade.Test.Core;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfArchivesTest : IDisposable
    {
        public void Dispose()
        {
            _archiveContent?.Dispose();
        }

        private Stream _archiveContent;

        private TestRun RunTest()
        {
            return new NumberOfArchives(new ArchiveContentMockReader(_archiveContent)).RunTest(new ArchiveBuilder().Build());
        }

        [Fact]
        public void NumberOfArchivesIsOne()
        {
            _archiveContent = Noark5XmlBuilder.Arkiv().Build();

            var testResults = RunTest();
            testResults.AnalysisResults[NumberOfArchives.AnalysisKeyArchives].Should().Be("1");
        }

        [Fact]
        public void NumberOfArchivesIsThree()
        {
            _archiveContent = Noark5XmlBuilder.Arkiv()
                .Arkivdel()
                .Underarkiv().Arkivdel()
                .Underarkiv().Arkivdel().Build();

            var testResults = RunTest();
            testResults.AnalysisResults[NumberOfArchives.AnalysisKeyArchives].Should().Be("3");
        }
    }
}