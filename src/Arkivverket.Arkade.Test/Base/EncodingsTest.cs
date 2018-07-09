using Arkivverket.Arkade.Core.Base;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Base
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