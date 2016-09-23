using System;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

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
            return new NumberOfArchives(new ArchiveContentMemoryStreamReader(_archiveContent)).RunTest(new Archive("123", ""));
        }

        [Fact]
        public void NumberOfArchivesIsOne()
        {
            _archiveContent = ArchiveBuilder.Arkiv().Build();

            var testResults = RunTest();
            testResults.AnalysisResults[NumberOfArchives.AnalysisKeyArchives].Should().Be("1");
        }

        [Fact]
        public void NumberOfArchivesIsThree()
        {
            _archiveContent = ArchiveBuilder.Arkiv()
                .Arkivdel()
                .Underarkiv().Arkivdel()
                .Underarkiv().Arkivdel().Build();

            var testResults = RunTest();
            testResults.AnalysisResults[NumberOfArchives.AnalysisKeyArchives].Should().Be("3");
        }
    }
}