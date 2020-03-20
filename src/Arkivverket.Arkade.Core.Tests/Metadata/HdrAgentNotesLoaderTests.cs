using Arkivverket.Arkade.Core.Metadata;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Metadata
{
    public class HdrAgentNotesLoaderTests
    {
        private const string Address = "Road 1, 1000 City";
        private const string Telephone = "99999999";
        private const string Email = "post@entity.com";

        private readonly string[] _entityNotes =
        {
            Address,
            Telephone,
            Email
        };

        private const string Version = "v1.0.0";
        private const string Type = "Noark5";
        private const string TypeVersion = "v3.1";

        private readonly string[] _systemNotes =
        {
            Version,
            Type,
            TypeVersion,
        };

        [Fact]
        public void GetAddressTest()
        {
            HdrAgentNotesLoader.GetAddress(_entityNotes).Should().Be(Address);
        }

        [Fact]
        public void GetTelephoneTest()
        {
            HdrAgentNotesLoader.GetTelephone(_entityNotes).Should().Be(Telephone);
        }

        [Fact]
        public void GetEmailTest()
        {
            HdrAgentNotesLoader.GetEmail(_entityNotes).Should().Be(Email);
        }

        [Fact]
        public void GetEmailWithÆøåTest()
        {
            HdrAgentNotesLoader.GetEmail(new[] {"æøå@æøå.æøå", "eoa@eoa.eoa"}).Should().Be("æøå@æøå.æøå");
        }

        [Fact]
        public void GetVersionTest()
        {
            HdrAgentNotesLoader.GetVersion(_systemNotes).Should().Be(Version);
        }

        [Fact]
        public void GetTypeTest()
        {
            HdrAgentNotesLoader.GetType(_systemNotes).Should().Be(Type);
        }

        [Fact]
        public void GetTypeVersionTest()
        {
            HdrAgentNotesLoader.GetTypeVersion(_systemNotes).Should().Be(TypeVersion);
        }
    }
}
