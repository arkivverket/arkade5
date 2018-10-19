using System;
using Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Definitions.DataTypes
{
    public class DateDataTypeTest
    {
        [Fact]
        public void IsValidShouldReturnTrueIfDateIsValid()
        {
            new DateDataType("dd.MM.yyyyTHH:mm:sszzz", null).IsValid("18.11.2016T08:43:00+00:00").Should().BeTrue();
        }

        [Fact]
        public void IsValidShouldReturnFalsefDateIsInvalid()
        {
            new DateDataType("dd.MM.yyyyTHH:mm:sszzz", null).IsValid("99.11.2016T08:43:00+00:00").Should().BeFalse();
        }

        [Fact]
        public void ParseShouldReturnValidDateTimeObjects()
        {
            new DateDataType("dd.MM.yyyyTHH:mm:sszzz", null).Parse("18.11.2016T08:43:00+00:00")
                .Should().Be(new DateTimeOffset(2016, 11, 18, 08, 43, 0, TimeSpan.FromHours(0)));
            new DateDataType("dd.MM.yyyyTHH:mm:sszzz", null).Parse("18.11.2016T08:43:00+01:00")
                .Should().Be(new DateTimeOffset(2016, 11, 18, 08, 43, 0, TimeSpan.FromHours(1)));
            new DateDataType("dd.MM.yyyyTHH:mm:sszzz", null).Parse("18.11.2016T08:43:00-05:00")
                .Should().Be(new DateTimeOffset(2016, 11, 18, 08, 43, 0, TimeSpan.FromHours(-5)));
        }


    }
}