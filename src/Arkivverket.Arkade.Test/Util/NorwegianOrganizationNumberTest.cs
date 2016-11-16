using Arkivverket.Arkade.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Util
{
    public class NorwegianOrganizationNumberTest
    {
        [Fact]
        public void ShouldGenerateRandomValidOrganizationNumbers()
        {
            for (int i = 0; i < 100; i++)
            {
                NorwegianOrganizationNumber.CreateRandom().Verify().Should().BeTrue();
            }
        }

        [Fact]
        public void ShouldGenerateSameOrganizationNumberWithSameSeed()
        {
            NorwegianOrganizationNumber organizationNumber1 = NorwegianOrganizationNumber.CreateRandom("A");
            NorwegianOrganizationNumber organizationNumber2 = NorwegianOrganizationNumber.CreateRandom("A");

            organizationNumber1.Should().Be(organizationNumber2);
        }

        [Fact]
        public void ShouldNotGenerateSameOrganizationNumberWithDifferentSeed()
        {
            NorwegianOrganizationNumber organizationNumber1 = NorwegianOrganizationNumber.CreateRandom("1");
            NorwegianOrganizationNumber organizationNumber2 = NorwegianOrganizationNumber.CreateRandom("2");

            organizationNumber1.Should().NotBe(organizationNumber2);
        }

        [Fact]
        public void ShouldVerifyValidOrganizationNumbers()
        {
            NorwegianOrganizationNumber.Create("914 994 780").Verify().Should().BeTrue();
        }

        [Fact]
        public void ShouldRemoveSpaceInOrganizationNumber()
        {
            NorwegianOrganizationNumber.Create(" 914 994 780 ").ToString().Should().Be("914994780");
        }

        [Fact]
        public void ShouldRemoveDotInOrganizationNumber()
        {
            NorwegianOrganizationNumber.Create("1234.56.78903").ToString().Should().Be("12345678903");
        }

        [Fact]
        public void ToStringShouldReturnOrganizationNumberWithDotWhenDotSeparatorIsSpecfied()
        {
            NorwegianOrganizationNumber.Create("914994780").ToString(".").Should().Be("914.994.780");
        }

        [Fact]
        public void ToStringShouldReturnOrganizationNumberWithSpaceWhenSpaceSeparatorIsSpecfied()
        {
            NorwegianOrganizationNumber.Create("914994780").ToString(" ").Should().Be("914 994 780");
        }

        [Fact]
        public void ShouldNotVerifyInvalidOrganizationNumbers()
        {
            NorwegianOrganizationNumber.Create("914994781").Verify().Should().BeFalse();
        }


    }
}