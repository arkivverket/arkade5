using System.Collections.Generic;
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

            List<string> results = SiardValidator.Validate(inputFilePath, reportFilePath);

            File.Exists(reportFilePath).Should().BeTrue();
            // clean up generated files
            Directory.GetFiles(Path.Combine("TestData", "Siard")).Where(f => f.EndsWith(".txt")).ToList().ForEach(File.Delete);

            File.Exists(reportFilePath).Should().BeFalse();
        }

        [Fact]
        public void ShouldReportUnsupportedSiardVersion()
        {
            string inputFilePath = Path.Combine("TestData", "Siard", "siard1_med_blobs.siard");
            string reportFilePath = Path.Combine("TestData", "Siard", "testReport.txt");

            List<string> results = SiardValidator.Validate(inputFilePath, reportFilePath);

            results.Count.Should().Be(2);
            results[0].Should().Be(Resources.SiardMessages.ErrorMessage);
            results[1].Should().Be(Resources.SiardMessages.ValidatorDoesNotSupportVersionMessage);

            File.Exists(reportFilePath).Should().BeTrue();
            // clean up generated files
            Directory.GetFiles(Path.Combine("TestData", "Siard")).Where(f => f.EndsWith(".txt")).ToList().ForEach(File.Delete);

            File.Exists(reportFilePath).Should().BeFalse();
        }
        
        [Fact]
        public void ShouldValidateExtractProducedBySiardGui()
        {
            string inputFilePath = Path.Combine("TestData", "Siard", "siard2-1_med_eksterne_lobs", "siardGui", "siardGui.siard");
            string reportFilePath = Path.Combine("TestData", "Siard", "testReport.txt");

            SiardValidator.Validate(inputFilePath, reportFilePath);

            File.Exists(reportFilePath).Should().BeTrue();
            // clean up generated files
            Directory.GetFiles(Path.Combine("TestData", "Siard")).Where(f => f.EndsWith(".txt")).ToList().ForEach(File.Delete);

            File.Exists(reportFilePath).Should().BeFalse();
        }

        [Fact]
        public void ShouldValidateExtractProducedByDbptkDeveloper()
        {
            string inputFilePath = Path.Combine("TestData", "Siard", "siard2-1_med_eksterne_lobs", "dbPtk", "dbptk.siard");
            string reportFilePath = Path.Combine("TestData", "Siard", "testReport.txt");

            SiardValidator.Validate(inputFilePath, reportFilePath);

            File.Exists(reportFilePath).Should().BeTrue();
            // clean up generated files
            Directory.GetFiles(Path.Combine("TestData", "Siard")).Where(f => f.EndsWith(".txt")).ToList().ForEach(File.Delete);

            File.Exists(reportFilePath).Should().BeFalse();
        }

        [Fact]
        public void ShouldValidateExtractProducedBySpectralCoreFullConvert()
        {
            string inputFilePath = Path.Combine("TestData", "Siard", "siard2-1_med_eksterne_lobs", "fullConvert", "scfc.siard");
            string reportFilePath = Path.Combine("TestData", "Siard", "testReport.txt");

            SiardValidator.Validate(inputFilePath, reportFilePath);

            File.Exists(reportFilePath).Should().BeTrue();
            // clean up generated files
            Directory.GetFiles(Path.Combine("TestData", "Siard")).Where(f => f.EndsWith(".txt")).ToList().ForEach(File.Delete);

            File.Exists(reportFilePath).Should().BeFalse();
        }
    }
}
