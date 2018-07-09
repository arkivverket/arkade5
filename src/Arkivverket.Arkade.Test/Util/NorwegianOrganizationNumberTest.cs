using System;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;
using Assert = Xunit.Assert;

namespace Arkivverket.Arkade.Test.Util
{
    public class NorwegianOrganizationNumberTest
    {
        [Fact]
        public void ShouldGenerateRandomValidOrganizationNumbers()
        {
            for (int i = 0; i < 100; i++)
            {
                NorwegianOrganizationNumber.CreateRandom();
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
            NorwegianOrganizationNumber.Verify("914 994 780").Should().BeTrue();
        }

        [Fact]
        public void ShouldRemoveSpaceInOrganizationNumber()
        {
            NorwegianOrganizationNumber.Create(" 914 994 780 ").ToString().Should().Be("914994780");
        }

        [Fact]
        public void ShouldRemoveDotInOrganizationNumber()
        {
            NorwegianOrganizationNumber.Create("914.994.780").ToString().Should().Be("914994780");
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
            NorwegianOrganizationNumber.Verify("914994781").Should().BeFalse();
        }




        [Fact]
        public void VerifyShouldReturnFalseIfOrganizationNumberIsNotOfLength9()
        {
            NorwegianOrganizationNumber.Verify("").Should().BeFalse();
            NorwegianOrganizationNumber.Verify("1").Should().BeFalse();
            NorwegianOrganizationNumber.Verify("12345678").Should().BeFalse();
            NorwegianOrganizationNumber.Verify("1234567890").Should().BeFalse();
        }

        [Fact]
        public void VerifyShouldReturnFalseIfOrganizationNumberIsNotDigits()
        {
            NorwegianOrganizationNumber.Verify("12345678A").Should().BeFalse();
            NorwegianOrganizationNumber.Verify("ABCDEFGHI").Should().BeFalse();
        }


        [Fact]
        public void CreateShouldNotThrowExceptionIfValidOrganizationNumberIsUsed()
        {
            NorwegianOrganizationNumber.Create("914994780");
        }

        [Fact]
        public void CreateShouldThrowExceptionIfInvalidBirthNumberIsUsed()
        {
            Assert.Throws<ArgumentException>(() => NorwegianOrganizationNumber.Create("914994781"));
        }

    }
}