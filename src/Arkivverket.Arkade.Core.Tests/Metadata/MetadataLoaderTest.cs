using System;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Metadata;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Metadata
{
    public class MetadataLoaderTest
    {
        private readonly ArchiveMetadata _metadata = new ArchiveMetadata
        {
            System = new MetadataSystemInformationUnit {Name = "Some system name"},
            StartDate = new DateTime(1970, 01, 01),
            EndDate = new DateTime(2000, 01, 01)
        };

        [Fact]
        public void ComposeStandardLabelTest()
        {
            // All parts provided
            MetadataLoader.ComposeStandardLabel(_metadata.System.Name, _metadata.StartDate, _metadata.EndDate)
                .Should().Be("Some system name (1970 - 2000)");

            // System name missing
            MetadataLoader.ComposeStandardLabel(null, _metadata.StartDate, _metadata.EndDate)
                .Should().Be("(1970 - 2000)");

            // Start-date missing
            MetadataLoader.ComposeStandardLabel(_metadata.System.Name, null, _metadata.EndDate)
                .Should().Be("Some system name");

            // End-date missing
            MetadataLoader.ComposeStandardLabel(_metadata.System.Name, _metadata.StartDate, null)
                .Should().Be("Some system name");
            
            // No parts provided
            MetadataLoader.ComposeStandardLabel(null, null, null)
                .Should().Be("");
        }

        [Fact]
        public void HandleLabelPlaceholderTest()
        {
            TheResultOfUsingLabel("Some label").Should().Be("Some label");

            TheResultOfUsingLabel("[standard_label]").Should().Be("Some system name (1970 - 2000)");

            TheResultOfUsingLabel("").Should().Be("");

            TheResultOfUsingLabel(null).Should().Be(null);
        }

        private string TheResultOfUsingLabel(string label)
        {
            _metadata.Label = label;

            MetadataLoader.HandleLabelPlaceholder(_metadata);

            return _metadata.Label;
        }
    }
}
