using Arkivverket.Arkade.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Util
{
    public class NorwegianAccountNumberTest
    {
        [Fact]
        public void ShouldGenerateRandomValidAccountNumbers()
        {
            for (int i = 0; i < 100; i++)
            {
                NorwegianAccountNumber.CreateRandom().Verify().Should().BeTrue();
            }
        }

        [Fact]
        public void ShouldGenerateSameAccountNumberWithSameSeed()
        {
            NorwegianAccountNumber accountNumber1 = NorwegianAccountNumber.CreateRandom("A");
            NorwegianAccountNumber accountNumber2 = NorwegianAccountNumber.CreateRandom("A");

            accountNumber1.Should().Be(accountNumber2);
        }

        [Fact]
        public void ShouldNotGenerateSameAccountNumberWithDiffrerentSeed()
        {
            NorwegianAccountNumber accountNumber1 = NorwegianAccountNumber.CreateRandom("1");
            NorwegianAccountNumber accountNumber2 = NorwegianAccountNumber.CreateRandom("2");

            accountNumber1.Should().NotBe(accountNumber2);
        }

        [Fact]
        public void ShouldVerifyValidAccountNumbers()
        {
            NorwegianAccountNumber.Create("1234.56.78903").Verify().Should().BeTrue();
        }

        [Fact]
        public void ShouldRemoveSpaceInAccountNumber()
        {
            NorwegianAccountNumber.Create(" 1234 56 78903 ").ToString().Should().Be("12345678903");
        }

        [Fact]
        public void ShouldRemoveDotInAccountNumber()
        {
            NorwegianAccountNumber.Create("1234.56.78903").ToString().Should().Be("12345678903");
        }

        [Fact]
        public void ToStringShouldReturnAccountNumberWithDotWhenDotSeparatorIsSpecfied()
        {
            NorwegianAccountNumber.Create("12345678903").ToString(".").Should().Be("1234.56.78903");
        }

        [Fact]
        public void ToStringShouldReturnAccountNumberWithSpaceWhenSpaceSeparatorIsSpecfied()
        {
            NorwegianAccountNumber.Create("12345678903").ToString(" ").Should().Be("1234 56 78903");
        }

        [Fact]
        public void ShouldNotVerifyInvalidAccountNumbers()
        {
            NorwegianAccountNumber.Create("19089328311").Verify().Should().BeFalse();
        }


    }
}