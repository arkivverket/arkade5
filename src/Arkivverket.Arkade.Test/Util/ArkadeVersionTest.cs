using Xunit;
using Arkivverket.Arkade.Util;
using FluentAssertions;

namespace Arkivverket.Arkade.Test.Util
{
    public class ArkadeVersionTest
    {
        [Fact]
        public void GetVersionShouldReturnVersionNumber()
        {
            string version = ArkadeVersion.GetVersion();

            version.Should().Be("unknown");
        }
    }
}
