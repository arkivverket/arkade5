using System;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Tests;
using Arkivverket.Arkade.Tests.Noark5;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Arkivverket.Arkade.Test.Tests.Noark5
{
    public class NumberOfClassificationSystemsTest
    {
        private readonly ITestOutputHelper _output;

        public NumberOfClassificationSystemsTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void NumberOfClassificationSystemsIsOne()
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\Small";
            var archiveExtraction = new ArchiveExtraction("uuid", workingDirectory);
            archiveExtraction.ArchiveType = ArchiveType.Noark5;

            TestResults testResults = new NumberOfClassificationSystems().RunTest(archiveExtraction);

            // todo - add somwhere to put this data in TestResult
            testResults.Results[0].Message.Should().Contain("1");
            _output.WriteLine(testResults.Results[0].Message);
        }

        [Fact]
        public void NumberOfClassificationSystemsIsTwo()
        {
            string workingDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\Noark5\\ContentClassificationSystem";
            var archiveExtraction = new ArchiveExtraction("uuid", workingDirectory);
            archiveExtraction.ArchiveType = ArchiveType.Noark5;

            TestResults testResults = new NumberOfClassificationSystems().RunTest(archiveExtraction);

            // todo - add somwhere to put this data in TestResult
            testResults.Results[0].Message.Should().Contain("2");
            _output.WriteLine(testResults.Results[0].Message);
        }
    }
}
