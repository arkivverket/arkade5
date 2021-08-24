using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Testing.Siard;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Siard
{
    public class SiardValidatorRunnerTest
    {
        [Fact]
        public void ShouldGenerateValidationReportFileAtDesignatedDestination()
        {
            string inputFilePath = Path.Combine("TestData", "Siard", "dbptk_produced.siard");
            string reportFilePath = Path.Combine("TestData", "Siard", "testReport.txt");

            SiardValidator.Validate(inputFilePath, reportFilePath);

            File.Exists(reportFilePath).Should().BeTrue();
            // clean up generated files
            Directory.GetFiles(Path.Combine("TestData", "Siard")).Where(f => f.EndsWith(".txt")).ToList().ForEach(File.Delete);

            File.Exists(reportFilePath).Should().BeFalse();
        }
    }
}
