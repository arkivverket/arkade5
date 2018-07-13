using System;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;
using Assert = Xunit.Assert;

namespace Arkivverket.Arkade.Core.Tests.Util
{
    public class NorwegianAccountNumberTest
    {
        [Fact]
        public void CreateShouldNotThrowExceptionIfValidAccountNumberIsUsed()
        {
            NorwegianAccountNumber.Create("12345678903");
        }

        [Fact]
        public void CreateShouldThrowExceptionIfInvalidAccountNumberIsUsed()
        {
            Assert.Throws<ArgumentException>(() => NorwegianAccountNumber.Create("12345678900"));
        }

        [Fact]
        public void ShouldGenerateRandomValidAccountNumbers()
        {
            for (int i = 0; i < 100; i++)
            {
                NorwegianAccountNumber.CreateRandom();
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
        public void ShouldNotVerifyInvalidAccountNumbers()
        {
            NorwegianAccountNumber.Verify("19089328311").Should().BeFalse();
        }

        [Fact]
        public void ShouldRemoveDotInAccountNumber()
        {
            NorwegianAccountNumber.Create("1234.56.78903").ToString().Should().Be("12345678903");
        }

        [Fact]
        public void ShouldRemoveSpaceInAccountNumber()
        {
            NorwegianAccountNumber.Create(" 1234 56 78903 ").ToString().Should().Be("12345678903");
        }

        [Fact]
        public void ShouldVerifyValidAccountNumbers()
        {
            NorwegianAccountNumber.Verify("1234.56.78903").Should().BeTrue();
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
        public void VerifyShouldReturnFalseIfAccountNumberIsNotDigits()
        {
            NorwegianAccountNumber.Verify("12345678A").Should().BeFalse();
            NorwegianAccountNumber.Verify("ABCDEFGHI").Should().BeFalse();
        }


        [Fact]
        public void VerifyShouldReturnFalseIfAccountNumberIsNotOfLength9()
        {
            NorwegianAccountNumber.Verify("").Should().BeFalse();
            NorwegianAccountNumber.Verify("1").Should().BeFalse();
            NorwegianAccountNumber.Verify("12345678").Should().BeFalse();
            NorwegianAccountNumber.Verify("1234567890").Should().BeFalse();
        }
    }
}