using Arkivverket.Arkade.Util;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Util
{
    public class NorwegianBirthNumberTest
    {
        [Fact]
        public void ShouldGenerateRandomValidNumbers()
        {
            for (int i = 0; i < 100; i++)
            {
                NorwegianBirthNumber.CreateRandom().Verify().Should().BeTrue();
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
            NorwegianBirthNumber.Create("19089328341").Verify().Should().BeTrue();
            NorwegianBirthNumber.Create("08011129480").Verify().Should().BeTrue();
            NorwegianBirthNumber.Create("08063048608").Verify().Should().BeTrue();
            NorwegianBirthNumber.Create("01027244536").Verify().Should().BeTrue();
            NorwegianBirthNumber.Create("20041622092").Verify().Should().BeTrue();
            NorwegianBirthNumber.Create("24015706240").Verify().Should().BeTrue();
            NorwegianBirthNumber.Create("08055207438").Verify().Should().BeTrue();
            NorwegianBirthNumber.Create("22067937264").Verify().Should().BeTrue();
            NorwegianBirthNumber.Create("28090607806").Verify().Should().BeTrue();
        }

        [Fact]
        public void ShouldNotVerifyInvalidBirthNumbers()
        {
            NorwegianBirthNumber.Create("19089328311").Verify().Should().BeFalse();
            NorwegianBirthNumber.Create("08011129410").Verify().Should().BeFalse();
            NorwegianBirthNumber.Create("08063048618").Verify().Should().BeFalse();
            NorwegianBirthNumber.Create("01027244516").Verify().Should().BeFalse();
            NorwegianBirthNumber.Create("20041622012").Verify().Should().BeFalse();
            NorwegianBirthNumber.Create("24015706210").Verify().Should().BeFalse();
            NorwegianBirthNumber.Create("08055207418").Verify().Should().BeFalse();
            NorwegianBirthNumber.Create("22067937214").Verify().Should().BeFalse();
            NorwegianBirthNumber.Create("28090607816").Verify().Should().BeFalse();
        }


    }
}