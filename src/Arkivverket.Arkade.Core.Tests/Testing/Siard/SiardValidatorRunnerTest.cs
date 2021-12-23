using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Arkivverket.Arkade.Core.Testing.Siard;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Testing.Siard
{
    public class SiardValidatorRunnerTest
    {
        [Fact]
        [Trait("Category", "DBPTK integration")]
        public void ShouldGenerateValidationReportFileAtDesignatedDestination()
        {
            string inputFilePath = Path.Combine("TestData", "Siard", "dbptk_produced.siard");
            string reportFilePath = Path.Combine("TestData", "Siard", "testReport.txt");

            _ = SiardValidator.Validate(inputFilePath, reportFilePath);

            File.Exists(reportFilePath).Should().BeTrue();
            // clean up generated files
            Directory.GetFiles(Path.Combine("TestData", "Siard")).Where(f => f.EndsWith(".txt")).ToList().ForEach(File.Delete);

            File.Exists(reportFilePath).Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "DBPTK integration")]
        public void ShouldReportUnsupportedSiardVersion()
        {
            string inputFilePath = Path.Combine("TestData", "Siard", "siard1_med_blobs.siard");
            string reportFilePath = Path.Combine("TestData", "Siard", "testReport.txt");

            (List<string> results, _) = SiardValidator.Validate(inputFilePath, reportFilePath);

            results.Count.Should().Be(2);
            results[0].Should().Be(Resources.SiardMessages.ErrorMessage);
            results[1].Should().Be(Resources.SiardMessages.ValidatorDoesNotSupportVersionMessage);

            File.Exists(reportFilePath).Should().BeTrue();
            // clean up generated files
            Directory.GetFiles(Path.Combine("TestData", "Siard")).Where(f => f.EndsWith(".txt")).ToList().ForEach(File.Delete);

            File.Exists(reportFilePath).Should().BeFalse();
        }
        
        [Fact]
        [Trait("Category", "DBPTK integration")]
        public void ShouldValidateExtractProducedBySiardGui()
        {
            string inputFilePath = Path.Combine("TestData", "Siard", "siard2", "siardGui", "external", "siardGui.siard");
            string reportFilePath = Path.Combine("TestData", "Siard", "testReport.txt");

            (_, List<string> errorsAndWarnings) = SiardValidator.Validate(inputFilePath, reportFilePath);

            var errors = new List<string>(errorsAndWarnings.Where(e => e == null || !e.StartsWith("WARN")));

            errors.Count.Should().Be(1);
            errors[0].Should().BeNull();

            File.Exists(reportFilePath).Should().BeTrue();
            // clean up generated files
            Directory.GetFiles(Path.Combine("TestData", "Siard")).Where(f => f.EndsWith(".txt")).ToList().ForEach(File.Delete);

            File.Exists(reportFilePath).Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "DBPTK integration")]
        public void ShouldValidateExtractProducedByDbptkDeveloper()
        {
            string inputFilePath = Path.Combine("TestData", "Siard", "siard2", "dbPtk", "external", "dbptk.siard");
            string reportFilePath = Path.Combine("TestData", "Siard", "testReport.txt");

            (_, List<string> errorsAndWarnings) = SiardValidator.Validate(inputFilePath, reportFilePath);

            var errors = new List<string>(errorsAndWarnings.Where(e => e == null || !e.StartsWith("WARN")));

            errors.Count.Should().Be(1);
            errors[0].Should().BeNull();

            File.Exists(reportFilePath).Should().BeTrue();
            // clean up generated files
            Directory.GetFiles(Path.Combine("TestData", "Siard")).Where(f => f.EndsWith(".txt")).ToList().ForEach(File.Delete);

            File.Exists(reportFilePath).Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "DBPTK integration")]
        public void ShouldFailToValidateExtractProducedBySpectralCoreFullConvert()
        {
            Directory.GetFiles(Path.Combine("TestData", "Siard")).Where(f => f.EndsWith(".txt")).ToList().ForEach(File.Delete);

            string inputFilePath = Path.Combine("TestData", "Siard", "siard2", "fullConvert", "external", "scfc.siard");
            string reportFilePath = Path.Combine("TestData", "Siard", "testReport.txt");

            (_, List<string> errorsAndWarnings) = SiardValidator.Validate(inputFilePath, reportFilePath);

            var errors = new List<string>(errorsAndWarnings.Where(e => e == null || !e.StartsWith("WARN")));

            errors.Count.Should().Be(3);
            errors[0].Should().Be("ERROR Missing mandatory strings in the metadata.xml file (schemaName: , schemaFolder: schema0");
            errors[1].Should().Be("ERROR Schema name or schema folder attributes have a blank value. Please check the metadata.xml file for more information");
            errors[2].Should().BeNull();

            File.Exists(reportFilePath).Should().BeTrue();
            // clean up generated files
            Directory.GetFiles(Path.Combine("TestData", "Siard")).Where(f => f.EndsWith(".txt")).ToList().ForEach(File.Delete);

            File.Exists(reportFilePath).Should().BeFalse();
        }

        [Fact]
        [Trait("Category", "DBPTK integration")]
        public void ShouldReportWarningsWhenExternalLobsAreMissing()
        {
            Directory.GetFiles(Path.Combine("TestData", "Siard")).Where(f => f.EndsWith(".txt")).ToList().ForEach(File.Delete);

            string inputFilePath = Path.Combine("TestData", "Siard", "siard2", "externalLobsMissing", "dbptk.siard");
            string reportFilePath = Path.Combine("TestData", "Siard", "testReport.txt");

            (List<string> results, List<string> errorsAndWarnings) = SiardValidator.Validate(inputFilePath, reportFilePath);

            List<string> summary = results.Where(r => r != null && r.Trim().StartsWith("Number of")).ToList();

            string numberOfWarnings = new Regex(@"(?!\[)\d+(?=\])").Match(summary.First(s => s.Contains("warnings"))).Value;
            numberOfWarnings.Should().Be("31");

            var errors = new List<string>(errorsAndWarnings.Where(e => e == null || !e.StartsWith("WARN")));

            errors.Count.Should().Be(1);
            errors[0].Should().BeNull();

            File.Exists(reportFilePath).Should().BeTrue();
            // clean up generated files
            Directory.GetFiles(Path.Combine("TestData", "Siard")).Where(f => f.EndsWith(".txt")).ToList().ForEach(File.Delete);

            File.Exists(reportFilePath).Should().BeFalse();
        }
    }
}
