using System;
using Arkivverket.Arkade.Core.Util;
using FluentAssertions;
using Xunit;
using Assert = Xunit.Assert;

namespace Arkivverket.Arkade.Core.Tests.Util
{
    public class NorwegianBirthNumberTest
    {
        [Fact]
        public void ShouldGenerateRandomValidNumbers()
        {
            for (int i = 0; i < 100; i++)
            {
                NorwegianBirthNumber birthNumber =  NorwegianBirthNumber.CreateRandom();
                bool verified = NorwegianBirthNumber.Verify(birthNumber._id);
                verified.Should().Be(true);
            }
        }

        [Fact]
        public void ShouldGenerateSameBirthNumberWithSameSeed()
        {
            NorwegianBirthNumber birthNumber1 = NorwegianBirthNumber.CreateRandom("12345678901");
            NorwegianBirthNumber birthNumber2 = NorwegianBirthNumber.CreateRandom("12345678901");

            birthNumber1.Should().Be(birthNumber2);
        }

        [Fact]
        public void ShouldNotGenerateSameBirthNumberWithDiffrerentSeed()
        {
            NorwegianBirthNumber birthNumber1 = NorwegianBirthNumber.CreateRandom("1");
            NorwegianBirthNumber birthNumber2 = NorwegianBirthNumber.CreateRandom("2");

            birthNumber1.Should().NotBe(birthNumber2);
        }

        [Fact]
        public void ShouldVerifyValidBirthNumbers()
        {
            // Birth numnbers randomly generated with http://www.fnrinfo.no/Verktoy/FinnLovlige_Tilfeldig.aspx
            NorwegianBirthNumber.Verify("19089328341").Should().BeTrue();
            NorwegianBirthNumber.Verify("08011129480").Should().BeTrue();
            NorwegianBirthNumber.Verify("08063048608").Should().BeTrue();
            NorwegianBirthNumber.Verify("01027244536").Should().BeTrue();
            NorwegianBirthNumber.Verify("20041622092").Should().BeTrue();
            NorwegianBirthNumber.Verify("24015706240").Should().BeTrue();
            NorwegianBirthNumber.Verify("08055207438").Should().BeTrue();
            NorwegianBirthNumber.Verify("22067937264").Should().BeTrue();
            NorwegianBirthNumber.Verify("28090607806").Should().BeTrue();
        }

        [Fact]
        public void ShouldNotVerifyInvalidBirthNumbers()
        {
            NorwegianBirthNumber.Verify("19089328311").Should().BeFalse();
            NorwegianBirthNumber.Verify("08011129410").Should().BeFalse();
            NorwegianBirthNumber.Verify("08063048618").Should().BeFalse();
            NorwegianBirthNumber.Verify("01027244516").Should().BeFalse();
            NorwegianBirthNumber.Verify("20041622012").Should().BeFalse();
            NorwegianBirthNumber.Verify("24015706210").Should().BeFalse();
            NorwegianBirthNumber.Verify("08055207418").Should().BeFalse();
            NorwegianBirthNumber.Verify("22067937214").Should().BeFalse();
            NorwegianBirthNumber.Verify("28090607816").Should().BeFalse();
        }

        [Fact]
        public void VerifyShouldIgnoreSpace()
        {
            NorwegianBirthNumber.Verify("   19089328341").Should().BeTrue();
            NorwegianBirthNumber.Verify("19089328341   ").Should().BeTrue();
            NorwegianBirthNumber.Verify("190893 28341").Should().BeTrue();
            NorwegianBirthNumber.Verify("190893 28341 ").Should().BeTrue();
        }


        [Fact]
        public void VerifyShouldReturnFalseIfBirthNumberIsNotOfLength11()
        {
            NorwegianBirthNumber.Verify("").Should().BeFalse();
            NorwegianBirthNumber.Verify("1").Should().BeFalse();
            NorwegianBirthNumber.Verify("1234567890").Should().BeFalse();
            NorwegianBirthNumber.Verify("123456789012").Should().BeFalse();
        }

        [Fact]
        public void VerifyShouldReturnFalseIfBirthNumberIsNotDigits()
        {
            NorwegianBirthNumber.Verify("1234567890A").Should().BeFalse();
            NorwegianBirthNumber.Verify("ABCDEFGHIJK").Should().BeFalse();
        }


        [Fact]
        public void CreateShouldNotThrowExceptionIfValidBirthNumberIsUsed()
        {
            NorwegianBirthNumber.Create("19089328341");
        }

        [Fact]
        public void CreateShouldThrowExceptionIfInvalidBirthNumberIsUsed()
        {
            Assert.Throws<ArgumentException>(() => NorwegianBirthNumber.Create("19089328342"));
        }

    }
}