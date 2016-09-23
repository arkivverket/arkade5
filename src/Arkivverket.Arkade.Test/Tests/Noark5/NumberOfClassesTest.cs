using System;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfClassesTest : IDisposable
    {
        private Stream _archiveContent;

        [Fact]
        public void NumberOfClassesIsOne()
        {
            _archiveContent = ArchiveBuilder.Arkiv().Arkivdel().Klassifikasjonssystem().Klasse().Build();

            TestRun testResults = RunTest();
            testResults.AnalysisResults[NumberOfClasses.AnalysisKeyClasses].Should().Be("1");
        }

        [Fact]
        public void NumberOfClassesIsFour()
        {
            _archiveContent = ArchiveBuilder.Arkiv()
                .Arkivdel().Klassifikasjonssystem()
                    .Klasse()
                    .Klasse()
                    .Klasse()
                .Arkivdel().Klassifikasjonssystem().Klasse().Build();

            TestRun testResults = RunTest();
            testResults.AnalysisResults[NumberOfClasses.AnalysisKeyClasses].Should().Be("4");
        }

        private TestRun RunTest()
        {
            return new NumberOfClasses(new ArchiveContentMemoryStreamReader(_archiveContent)).RunTest(new Archive("123", ""));
        }

        public void Dispose()
        {
            _archiveContent?.Dispose();
        }
    }
}
