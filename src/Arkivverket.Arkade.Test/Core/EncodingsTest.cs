using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Encodings.ISO_8859_1.Should().NotBeNull();
            Encodings.ISO_8859_4.Should().NotBeNull();
        }
    }
}