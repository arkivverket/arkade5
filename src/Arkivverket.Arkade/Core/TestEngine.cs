using Arkivverket.Arkade.Tests;

namespace Arkivverket.Arkade.Core
{
    public class TestEngine
    {
        public void LoadArchive(ArchiveExtraction archiveExtraction)
        {
            var testsToRun = new TestProvider().GetTestsForArchiveExtraction(archiveExtraction);

            foreach (var test in testsToRun)
            {
                test.RunTest(archiveExtraction);
            }
        }
    }
}