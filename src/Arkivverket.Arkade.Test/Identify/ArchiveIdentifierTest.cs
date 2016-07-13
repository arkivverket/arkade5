using System;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Identify;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Identify
{
    public class ArchiveIdentifierTest
    {
        [Fact]
        public void ShouldIdentifyNoark3()
        {
            FindArchiveType("noark3-info.xml").Should().Be(ArchiveType.Noark3);
        }

        [Fact]
        public void ShouldIdentifyNoark4()
        {
            FindArchiveType("noark4-info.xml").Should().Be(ArchiveType.Noark4);
        }

        [Fact]
        public void ShouldIdentifyNoark5()
        {
            FindArchiveType("noark5-info.xml").Should().Be(ArchiveType.Noark5);
        }

        private ArchiveType FindArchiveType(string testFileName)
        {
            var metadata = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\{testFileName}";
            return new ArchiveIdentifier().Identify(metadata);
        }
    }
}
