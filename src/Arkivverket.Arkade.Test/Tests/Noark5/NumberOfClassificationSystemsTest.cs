using System;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfClassificationSystemsTest
    {
        [Fact]
        public void NumberOfClassificationSystemsIsOne()
        {
            var archiveExtraction = TestUtil.CreateArchiveExtraction("TestData\\Noark5\\Small");

            var testResults = new NumberOfClassificationSystems().RunTest(archiveExtraction);

            testResults.AnalysisResults[NumberOfClassificationSystems.AnalysisKeyClassificationSystems].Should().Be("1");
        }

        [Fact(Skip = "test case not completed")]
        public void NumberOfClassificationSystemsIsTwo()
        {
            var archiveExtraction = TestUtil.CreateArchiveExtraction("TestData\\Noark5\\ContentClassificationSystem");

            var testResults = new NumberOfClassificationSystems().RunTest(archiveExtraction);

            testResults.AnalysisResults[NumberOfClassificationSystems.AnalysisKeyClassificationSystems].Should().Be("2");
        }
    }
}