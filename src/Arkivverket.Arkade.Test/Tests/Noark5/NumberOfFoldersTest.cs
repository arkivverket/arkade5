using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfFoldersTest
    {
        [Fact]
        public void NumberOfFoldersIsOne()
        {
            var archiveExtraction = TestUtil.CreateArchiveExtraction("TestData\\Noark5\\Small");

            var testResults = new NumberOfFolders().RunTest(archiveExtraction);

            testResults.AnalysisResults[NumberOfFolders.AnalysisKeyFolders].Should().Be("1");
        }

        [Fact]
        public void ForTwoArchivePartsWithOneSingleFolderThenNumberOfFoldersIsTwo()
        {
            var archiveExtraction = TestUtil.CreateArchiveExtraction("TestData\\Noark5\\TwoArchiveParts");

            var testResults = new NumberOfFolders().RunTest(archiveExtraction);

            testResults.AnalysisResults[NumberOfFolders.AnalysisKeyFolders].Should().Be("2");
        }
    }
}
