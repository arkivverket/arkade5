using Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Definitions.DataTypes
{
    public class TimeDataTypeTest
    {
        [Fact]
        public void IsValidShouldReturnTrueIfTimeStringAndFormatStringAreValid()
        {
            new TimeDataType("hhmmss").IsValid("101010", true).Should().BeTrue();

            new TimeDataType("HHMMSS").IsValid("101010", true).Should().BeTrue();

            new TimeDataType("hh:mm:ss").IsValid("10:10:10", true).Should().BeTrue();

            new TimeDataType("hh-mm-ss").IsValid("10-10-10", true).Should().BeTrue();
        }

        [Fact]
        public void IsValidShouldReturnFalseIfTimeStringIsInvalid()
        {
            new TimeDataType("hhmmss").IsValid("240000", true).Should().BeFalse();

            new TimeDataType("hhmmss").IsValid("", false).Should().BeFalse();
        }

        [Fact]
        public void IsValidShouldReturnFalseIfFormatStringIsInvalid()
        {
            new TimeDataType("hh mm ss").IsValid("10 10 10", true).Should().BeFalse();

            new TimeDataType("xxyyzz").IsValid("101010", true).Should().BeFalse();

            new TimeDataType("").IsValid("101010", true).Should().BeFalse();
        }
    }
}
