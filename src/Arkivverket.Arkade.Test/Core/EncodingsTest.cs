using Arkivverket.Arkade.Core;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core
{
    public class EncodingsTest
    {
        [Fact]
        public void ShouldGetEncodings()
        {
            Encodings.UTF8.Should().NotBeNull();
            Encodings.ISO_8859_1.Should().NotBeNull();
        }
    }
}