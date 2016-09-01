using System.Collections.Generic;
using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core
{
    public class TestEngine
    {
        public List<TestResults> RunTestsOnArchive(ArchiveExtraction archiveExtraction)
        {
            var testsToRun = new TestProvider().GetTestsForArchiveExtraction(archiveExtraction);

            List<TestResults> testResultsFromAllTests = new List<TestResults>();
            foreach (var test in testsToRun)
            {
                TestResults testResults = test.RunTest(archiveExtraction);
                testResultsFromAllTests.Add(testResults);
            }
            return testResultsFromAllTests;
        }
    }
}