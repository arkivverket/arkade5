using Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Base.Addml.Definitions.DataTypes
{
    public class TimeDataTypeTest
    {
        [Fact]
        public void IsValidShouldReturnTrueIfTimeStringAndFormatStringAreValid()
        {
            new TimeDataType("hhmmss").IsValid("101010").Should().BeTrue();

            new TimeDataType("HHMMSS").IsValid("101010").Should().BeTrue();

            new TimeDataType("hh:mm:ss").IsValid("10:10:10").Should().BeTrue();

            new TimeDataType("hh-mm-ss").IsValid("10-10-10").Should().BeTrue();
        }

        [Fact]
        public void IsValidShouldReturnFalseIfTimeStringIsInvalid()
        {
            new TimeDataType("hhmmss").IsValid("240000").Should().BeFalse();

            new TimeDataType("hhmmss").IsValid("").Should().BeFalse();
        }

        [Fact]
        public void IsValidShouldReturnFalseIfFormatStringIsInvalid()
        {
            new TimeDataType("hh mm ss").IsValid("10 10 10").Should().BeFalse();

            new TimeDataType("xxyyzz").IsValid("101010").Should().BeFalse();

            new TimeDataType("").IsValid("101010").Should().BeFalse();
        }
    }
}
