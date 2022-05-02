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
        public void IsValidShouldReturnFalseIfDateIsInvalid()
        {
            new DateDataType("dd.MM.yyyyTHH:mm:sszzz", null).IsValid("99.11.2016T08:43:00+00:00").Should().BeFalse();
            new DateDataType("not-a-date").IsValid("also not a date").Should().BeFalse();
            new DateDataType("YYYY-MM-DD\"T\"hh\":\"mm\":\"ss\"Z\"").IsValid("2001-01-01-T10:10:10Z").Should().BeFalse();
            new DateDataType("YYYY-MM-DD\"T\"hh\":\"mm\":\"ss\"Z\"").IsValid("åpenbar feil").Should().BeFalse();
            new DateDataType("YYYY-DDD").IsValid("2022--200-").Should().BeFalse();
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

        [Fact]
        public void ParseShouldSupportNS_ISO_8601Formats()
        {
            //Calendar dates
            //Basic representations
            new DateDataType("YYYYMMDD").Parse("20200101")
                .Should().Be(new DateTimeOffset(2020, 01, 01, 00, 00, 00, TimeSpan.FromHours(0)));
            new DateDataType("YYYYMMDD\"T\"hhmmss\"Z\"").Parse("20200101T010203Z")
                .Should().Be(new DateTimeOffset(2020, 01, 01, 01, 02, 03, TimeSpan.FromHours(0)));
            new DateDataType("YYYYMMDD\"T\"hhmmss±hh").Parse("20200101T010203+01")
                .Should().Be(new DateTimeOffset(2020, 01, 01, 01, 02, 03, TimeSpan.FromHours(1)));
            new DateDataType("YYYYMMDD\"T\"hhmmss±hhmm").Parse("20200101T010203+0100")
                .Should().Be(new DateTimeOffset(2020, 01, 01, 01, 02, 03, TimeSpan.FromHours(1)));
            //Extended representations
            new DateDataType("YYYY-MM-DD").Parse("2020-01-01")
                .Should().Be(new DateTimeOffset(2020, 01, 01, 00, 00, 00, TimeSpan.FromHours(0)));
            new DateDataType("YYYY-MM-DD\"T\"hh\":\"mm\":\"ss\"Z\"").Parse("2020-01-01T01:02:03Z")
                .Should().Be(new DateTimeOffset(2020, 01, 01, 01, 02, 03, TimeSpan.FromHours(0)));
            new DateDataType("YYYY-MM-DD\"T\"hh\":\"mm\":\"ss±hh").Parse("2020-01-01T01:02:03+01")
                .Should().Be(new DateTimeOffset(2020, 01, 01, 01, 02, 03, TimeSpan.FromHours(1)));
            new DateDataType("YYYY-MM-DD\"T\"hh\":\"mm\":\"ss±hh\":\"mm").Parse("2020-01-01T01:02:03+01:00")
                .Should().Be(new DateTimeOffset(2020, 01, 01, 01, 02, 03, TimeSpan.FromHours(1)));

            //Ordinal dates
            //Basic representations
            new DateDataType("YYYYDDD").Parse("2022076")
                .Should().Be(new DateTimeOffset(2022, 03, 17, 00, 00, 00, TimeSpan.FromHours(0)));
            new DateDataType("YYYYDDD\"T\"hhmmss\"Z\"").Parse("2022076T092800Z")
                .Should().Be(new DateTimeOffset(2022, 03, 17, 09, 28, 00, TimeSpan.FromHours(0)));
            new DateDataType("YYYYDDD\"T\"hhmmss±hh").Parse("2022076T092800+01")
                .Should().Be(new DateTimeOffset(2022, 03, 17, 09, 28, 00, TimeSpan.FromHours(1)));
            new DateDataType("YYYYDDD\"T\"hhmmss±hhmm").Parse("2022076T092800+0100")
                .Should().Be(new DateTimeOffset(2022, 03, 17, 09, 28, 00, TimeSpan.FromHours(1)));
            //Extended representations
            new DateDataType("YYYY-DDD").Parse("2024-366")
                .Should().Be(new DateTimeOffset(2024, 12, 31, 00, 00, 00, TimeSpan.FromHours(0)));
            new DateDataType("YYYY-DDD\"T\"hh\":\"mm\":\"ss\"Z\"").Parse("2024-366T09:28:00Z")
                .Should().Be(new DateTimeOffset(2024, 12, 31, 09, 28, 00, TimeSpan.FromHours(0)));
            new DateDataType("YYYY-DDD\"T\"hh\":\"mm\":\"ss±hh").Parse("2024-366T09:28:00+01")
                .Should().Be(new DateTimeOffset(2024, 12, 31, 09, 28, 00, TimeSpan.FromHours(1)));
            new DateDataType("YYYY-DDD\"T\"hh\":\"mm\":\"ss±hh\":\"mm").Parse("2024-366T09:28:00+01:00")
                .Should().Be(new DateTimeOffset(2024, 12, 31, 09, 28, 00, TimeSpan.FromHours(1)));

            //Week dates
            //Basic representations
            new DateDataType("YYYY\"W\"WWD").Parse("2022W011")
                .Should().Be(new DateTimeOffset(2022, 01, 03, 00, 00, 00, TimeSpan.FromHours(0)));
            new DateDataType("YYYY\"W\"WWD\"T\"hhmmss\"Z\"").Parse("2022W011T010203Z")
                .Should().Be(new DateTimeOffset(2022, 01, 03, 01, 02, 03, TimeSpan.FromHours(0)));
            new DateDataType("YYYY\"W\"WWD\"T\"hhmmss±hh").Parse("2022-W01-1T01:02:03+01")
                .Should().Be(new DateTimeOffset(2022, 01, 03, 01, 02, 03, TimeSpan.FromHours(1)));
            new DateDataType("YYYY\"W\"WWD\"T\"hhmmss±hhmm").Parse("2022-W01-1T01:02:03+01:00")
                .Should().Be(new DateTimeOffset(2022, 01, 03, 01, 02, 03, TimeSpan.FromHours(1)));
            //Extended representations
            new DateDataType("YYYY-\"W\"WW-D").Parse("2022-W52-7")
                .Should().Be(new DateTimeOffset(2023, 01, 01, 00, 00, 00, TimeSpan.FromHours(0)));
            new DateDataType("YYYY-\"W\"WW-D\"T\"hh\":\"mm\":\"ss\"Z\"").Parse("2022-W52-7T01:02:03Z")
                .Should().Be(new DateTimeOffset(2023, 01, 01, 01, 02, 03, TimeSpan.FromHours(0)));
            new DateDataType("YYYY-\"W\"WW-D\"T\"hh\":\"mm\":\"ss±hh").Parse("2022-W52-7T01:02:03+01")
                .Should().Be(new DateTimeOffset(2023, 01, 01, 01, 02, 03, TimeSpan.FromHours(1)));
            new DateDataType("YYYY-\"W\"WW-D\"T\"hh\":\"mm\":\"ss±hh\":\"mm").Parse("2022-W52-7T01:02:03+01:00")
                .Should().Be(new DateTimeOffset(2023, 01, 01, 01, 02, 03, TimeSpan.FromHours(1)));
        }
        
        [Fact]
        public void NS_ISO_8601FormatsShouldBeValid()
        {
            //Calendar dates
            //Basic representations
            new DateDataType("YYYYMMDD").IsValid("20200101").Should().BeTrue();
            new DateDataType("YYYYMMDD\"T\"hhmmss\"Z\"").IsValid("20200101T010203Z").Should().BeTrue();
            new DateDataType("YYYYMMDD\"T\"hhmmss±hh").IsValid("20200101T010203+01").Should().BeTrue();
            new DateDataType("YYYYMMDD\"T\"hhmmss±hhmm").IsValid("20200101T010203+0100").Should().BeTrue();
            //Extended representations
            new DateDataType("YYYY-MM-DD").IsValid("2020-01-01").Should().BeTrue();
            new DateDataType("YYYY-MM-DD\"T\"hh\":\"mm\":\"ss\"Z\"").IsValid("2020-01-01T01:02:03Z").Should().BeTrue();
            new DateDataType("YYYY-MM-DD\"T\"hh\":\"mm\":\"ss±hh").IsValid("2020-01-01T01:02:03+01").Should().BeTrue();
            new DateDataType("YYYY-MM-DD\"T\"hh\":\"mm\":\"ss±hh\":\"mm").IsValid("2020-01-01T01:02:03+01:00").Should().BeTrue();

            //Ordinal dates
            //Basic representations
            new DateDataType("YYYYDDD").IsValid("2022076").Should().BeTrue();
            new DateDataType("YYYYDDD\"T\"hhmmss\"Z\"").IsValid("2022076T092800Z").Should().BeTrue();
            new DateDataType("YYYYDDD\"T\"hhmmss±hh").IsValid("2022076T092800+01").Should().BeTrue();
            new DateDataType("YYYYDDD\"T\"hhmmss±hhmm").IsValid("2022076T092800+0100").Should().BeTrue();
            //Extended representations
            new DateDataType("YYYY-DDD").IsValid("2024-366").Should().BeTrue();
            new DateDataType("YYYY-DDD\"T\"hh\":\"mm\":\"ss\"Z\"").IsValid("2024-366T09:28:00Z").Should().BeTrue();
            new DateDataType("YYYY-DDD\"T\"hh\":\"mm\":\"ss±hh").IsValid("2024-366T09:28:00+01").Should().BeTrue();
            new DateDataType("YYYY-DDD\"T\"hh\":\"mm\":\"ss±hh\":\"mm").IsValid("2024-366T09:28:00+01:00").Should().BeTrue();

            //Week dates
            //Basic representations
            new DateDataType("YYYY\"W\"WWD").IsValid("2022W011").Should().BeTrue();
            new DateDataType("YYYY\"W\"WWD\"T\"hhmmss\"Z\"").IsValid("2022W011T010203Z").Should().BeTrue();
            new DateDataType("YYYY\"W\"WWD\"T\"hhmmss±hh").IsValid("2022-W01-1T01:02:03+01").Should().BeTrue();
            new DateDataType("YYYY\"W\"WWD\"T\"hhmmss±hhmm").IsValid("2022-W01-1T01:02:03+01:00").Should().BeTrue();
            //Extended representations
            new DateDataType("YYYY-\"W\"WW-D").IsValid("2022-W52-7").Should().BeTrue();
            new DateDataType("YYYY-\"W\"WW-D\"T\"hh\":\"mm\":\"ss\"Z\"").IsValid("2022-W52-7T01:02:03Z").Should().BeTrue();
            new DateDataType("YYYY-\"W\"WW-D\"T\"hh\":\"mm\":\"ss±hh").IsValid("2022-W52-7T01:02:03+01").Should().BeTrue();
            new DateDataType("YYYY-\"W\"WW-D\"T\"hh\":\"mm\":\"ss±hh\":\"mm").IsValid("2022-W52-7T01:02:03+01:00").Should().BeTrue();
        }
    }
}