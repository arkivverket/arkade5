using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfFoldersTest
    {
        [Fact(Skip = null)]
        public void NumberOfFoldersIsOne()
        {
            var archiveExtraction = TestUtil.CreateArchiveExtraction("TestData\\Noark5\\Small");

            var testResults = new NumberOfFolders(null).RunTest(archiveExtraction);

            testResults.AnalysisResults[NumberOfFolders.AnalysisKeyFolders].Should().Be("1");
        }

        [Fact(Skip = "testdata not completed")]
        public void ForTwoArchivePartsWithOneSingleFolderThenNumberOfFoldersIsTwo()
        {
            var archiveExtraction = TestUtil.CreateArchiveExtraction("TestData\\Noark5\\TwoArchiveParts");

            var testResults = new NumberOfFolders(null).RunTest(archiveExtraction);

            testResults.AnalysisResults[NumberOfFolders.AnalysisKeyFolders].Should().Be("2");
        }
    }
}
