using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Arkivverket.Arkade.Core.Testing.Siard;

namespace Arkivverket.Arkade.Core.Base.Siard
{
    public class SiardTestEngine : ITestEngine
    {
        private readonly ISiardValidator _validator;

        public SiardTestEngine(ISiardValidator validator)
        {
            _validator = validator;
        }

        public TestSuite RunTestsOnArchive(TestSession testSession)
        {
            FileInfo siardFileInfo = testSession.Archive.Content.DirectoryInfo().GetFiles()
                .First(f => f.Extension.Equals(".siard"));
            string inputFilePath = siardFileInfo.FullName;
            string reportFilePath = Path.Combine(testSession.TemporaryTestResultFilesDirectory.FullName,
                Resources.OutputFileNames.DbptkValidationReportFile);

            SiardValidationReport report = _validator.Validate(inputFilePath, reportFilePath);

            List<string> summary = report.Results.Where(r => r != null && r.Contains("number of", StringComparison.InvariantCultureIgnoreCase)).ToList();

            int numberOfValidationErrors;
            int numberOfValidationWarnings;

            if (!summary.Any())
            {
                numberOfValidationErrors = 0;
                numberOfValidationWarnings = 0;
            }
            else
            {
                numberOfValidationErrors = GetNumberOfXFromSummary("errors", summary);
                numberOfValidationWarnings = GetNumberOfXFromSummary("warnings", summary);
            }

            testSession.TestSummary = new TestSummary(0, 0, 0, numberOfValidationErrors, numberOfValidationWarnings);

            return new TestSuite(report.TestingTool);
        }

        private int GetNumberOfXFromSummary(string x, List<string> summary)
        {
            var regex = new Regex(@"(?!\[)\d+(?=\])");
            string match = regex.Match(summary.First(s => s.Contains(x, StringComparison.InvariantCultureIgnoreCase))).Value;

            return int.Parse(match);
        }
    }
}
