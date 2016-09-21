using System;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfClassificationSystemsTest
    {
        private static ArchiveExtraction CreateArchiveExtraction(string testdataDirectory)
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\{testdataDirectory}";
            var archiveExtraction = new ArchiveExtraction("uuid", workingDirectory);
            archiveExtraction.ArchiveType = ArchiveType.Noark5;
            return archiveExtraction;
        }

        [Fact]
        public void NumberOfClassificationSystemsIsOne()
        {
            var archiveExtraction = CreateArchiveExtraction("TestData\\Noark5\\Small");

            var testResults = new NumberOfClassificationSystems().RunTest(archiveExtraction);

            testResults.AnalysisResults[NumberOfClassificationSystems.AnalysisKeyClassificationSystems].Should().Be("1");
        }

        [Fact]
        public void NumberOfClassificationSystemsIsTwo()
        {
            var archiveExtraction = CreateArchiveExtraction("TestData\\Noark5\\ContentClassificationSystem");

            var testResults = new NumberOfClassificationSystems().RunTest(archiveExtraction);

            testResults.AnalysisResults[NumberOfClassificationSystems.AnalysisKeyClassificationSystems].Should().Be("2");
        }
    }
}