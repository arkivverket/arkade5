using System;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfClassificationSystemsTest : IDisposable
    {
        private Stream _archiveContent;

        private TestResults RunTest()
        {
            return new NumberOfClassificationSystems(new ArchiveContentMemoryStreamReader(_archiveContent)).RunTest(new ArchiveExtraction("123", ""));
        }

        [Fact]
        public void NumberOfClassificationSystemsIsOne()
        {
            _archiveContent = ArchiveBuilder.Arkiv().Arkivdel().Klassifikasjonssystem().Build();

            TestResults testResults = RunTest();
            testResults.AnalysisResults[NumberOfClassificationSystems.AnalysisKeyClassificationSystems].Should().Be("1");
        }

        [Fact]
        public void NumberOfClassificationSystemsIsTwo()
        {
            _archiveContent = ArchiveBuilder.Arkiv()
                .Arkivdel().Klassifikasjonssystem()
                .Arkivdel().Klassifikasjonssystem()
                .Build();

            TestResults testResults = RunTest();
            testResults.AnalysisResults[NumberOfClassificationSystems.AnalysisKeyClassificationSystems].Should().Be("2");
        }

        public void Dispose()
        {
            _archiveContent.Dispose();
        }
    }
}