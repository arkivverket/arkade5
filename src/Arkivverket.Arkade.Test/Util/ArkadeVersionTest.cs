using Xunit;
using Arkivverket.Arkade.Util;
using FluentAssertions;
using System.Reflection;
using System.Diagnostics;

namespace Arkivverket.Arkade.Test.Util
{
    public class ArkadeVersionTest
    {
        [Fact]
        public void VersionShouldReturnVersionNumber()
        {
            string version = ArkadeVersion.Version;

            version.Should().MatchRegex("[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+");
        }
    }
}
