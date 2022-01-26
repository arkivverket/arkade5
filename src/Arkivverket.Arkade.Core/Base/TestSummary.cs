namespace Arkivverket.Arkade.Core.Base
{
    public class TestSummary
    {
        public string NumberOfProcessedFiles { get; }
        public string NumberOfProcessedRecords { get; }
        public string NumberOfTestsRun { get; }
        public string NumberOfErrors { get; }
        public string NumberOfWarnings { get; }

        public TestSummary(int numberOfProcessedFiles, long numberOfProcessedRecords, int numberOfTestsRun,
            int numberOfErrors, int numberOfWarnings)
        {
            NumberOfProcessedFiles = numberOfProcessedFiles.ToString();
            NumberOfProcessedRecords = numberOfProcessedRecords.ToString();
            NumberOfTestsRun = numberOfTestsRun.ToString();
            NumberOfErrors = numberOfErrors.ToString();
            NumberOfWarnings = numberOfWarnings.ToString();
        }

    }
}