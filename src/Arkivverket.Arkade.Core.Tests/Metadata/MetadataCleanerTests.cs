using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Metadata;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Metadata
{
    public class MetadataCleanerTests
    {
        [Fact]
        public void CleanTest()
        {
            var metadata = new ArchiveMetadata // NB! Metadata-origin (unit testing)
            {
                Owners = new List<MetadataEntityInformationUnit>
                {
                    new MetadataEntityInformationUnit
                    {
                        ContactPerson = "\nNeil Armstrong\r\n",
                        Address = " The Moon "
                    }
                },
                Transferer = new MetadataEntityInformationUnit
                {
                    ContactPerson = "\nBuzz Aldrin\r\n",
                    Address = " The Eagle "
                },
                Producer = new MetadataEntityInformationUnit
                {
                    ContactPerson = "\nMichael Collins\r\n",
                    Address = " The Command Module "
                },
                System = new MetadataSystemInformationUnit
                {
                    Name = "\tApollo",
                    Version = "\t11"
                }
            };

            MetadataCleaner.Clean(metadata);

            metadata.Owners[0].ContactPerson.Should().Be("Neil Armstrong");
            metadata.Owners[0].Address.Should().Be("The Moon");

            metadata.Transferer.ContactPerson.Should().Be("Buzz Aldrin");
            metadata.Transferer.Address.Should().Be("The Eagle");

            metadata.Producer.ContactPerson.Should().Be("Michael Collins");
            metadata.Producer.Address.Should().Be("The Command Module");

            metadata.System.Name.Should().Be("Apollo");
            metadata.System.Version.Should().Be("11");
        }
    }
}
