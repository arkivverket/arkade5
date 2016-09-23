using System;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfFoldersTest : IDisposable
    {
        private Stream _archiveContent;

        private TestResults RunTest()
        {
            return new NumberOfFolders(new ArchiveContentMemoryStreamReader(_archiveContent)).RunTest(new ArchiveExtraction("123", null));
        }

        [Fact]
        public void NumberOfFoldersIsOne()
        {
            _archiveContent = ArchiveBuilder.Arkiv().Arkivdel().Klassifikasjonssystem().Klasse().Mappe().Build();

            var testResults = RunTest();

            testResults.AnalysisResults[NumberOfFolders.AnalysisKeyFolders].Should().Be("1");
        }

        [Fact]
        public void ForTwoArchivePartsWithOneSingleFolderThenNumberOfFoldersIsTwo()
        {
            _archiveContent = ArchiveBuilder.Arkiv()
                .Arkivdel().Klassifikasjonssystem().Klasse().Mappe()
                .Arkivdel().Klassifikasjonssystem().Klasse().Mappe().Build();

            var testResults = RunTest();

            testResults.AnalysisResults[NumberOfFolders.AnalysisKeyFolders].Should().Be("2");
        }

        [Fact]
        public void ShouldFindMultipleFoldersWithinSameArchiveParts()
        {
            _archiveContent = ArchiveBuilder.Arkiv().Arkivdel().Klassifikasjonssystem().Klasse()
                    .Mappe()
                    .Mappe()
                    .Mappe()
                .Build();

            var testResults = RunTest();

            testResults.AnalysisResults[NumberOfFolders.AnalysisKeyFolders].Should().Be("3");
        }


        public void Dispose()
        {
            _archiveContent?.Dispose();
        }
    }
}
