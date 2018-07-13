using System;
using Xunit;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using System.Text.RegularExpressions;
using Moq;

namespace Arkivverket.Arkade.Core.Tests.Util
{
    public class ArkadeVersionTest
    {
        private readonly IReleaseInfoReader _releaseInfoReader;

        public ArkadeVersionTest()
        {
            _releaseInfoReader = Mock.Of<IReleaseInfoReader>();

            var versionHigherThanCurrent = new Version(ArkadeVersion.GetCurrent().Major + 1 + ".0.0.0");

            Mock.Get(_releaseInfoReader).Setup(r => r.GetLatestVersion()).Returns(versionHigherThanCurrent);
        }

        [Fact]
        public void ShouldReturnCurrentVersionNumberAsExpected()
        {
            HasExpectedValue(ArkadeVersion.Current).Should().BeTrue();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void ShouldReturnLatestVersionNumberAsExpected()
        {
            string latestVersionNumber = new ArkadeVersion(_releaseInfoReader).GetLatest().ToString();

            HasExpectedValue(latestVersionNumber).Should().BeTrue();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void ShouldFindThatNewerVersionIsAvailable()
        {
            var arkadeVersion = new ArkadeVersion(_releaseInfoReader);

            arkadeVersion.UpdateIsAvailable().Should().BeTrue();
        }

        [Fact (Skip="Not used feature")]
        [Trait("Category", "Integration")]
        public void ShouldUpdateLastCheckForUpdateTime()
        {
            new ArkadeVersion(_releaseInfoReader).GetLatest();

            DateTime? lastCheckForUpdate = new ArkadeVersion(_releaseInfoReader).GetTimeLastCheckForUpdate();

            DateTime now = DateTime.Now;

            lastCheckForUpdate.Should().BeCloseTo(now, 60000);
        }

        private bool HasExpectedValue(string versionNumber)
        {
            return !string.IsNullOrEmpty(versionNumber) &&
                   Regex.IsMatch(versionNumber, "[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+");
        }
    }
}
