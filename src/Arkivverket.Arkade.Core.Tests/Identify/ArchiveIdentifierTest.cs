using System;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Identify;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Identify
{
    public class ArchiveIdentifierTest
    {
        [Fact]
        public void ShouldIdentifyNoark3()
        {
            FindArchiveType("noark3-info.xml").Should().Be(ArchiveType.Noark3);
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
