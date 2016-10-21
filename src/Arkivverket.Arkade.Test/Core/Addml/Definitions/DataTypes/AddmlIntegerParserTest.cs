using System;
using Arkivverket.Arkade.Core.Addml.Definitions;
using Arkivverket.Arkade.Core.Addml.Definitions.DataTypes;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml.Definitions.DataTypes
{
    public class AddmlIntegerParserTest
    {
        [Fact]
        public void ShouldParseSimpleInteger()
        {
            AddmlIntegerParser parser = new AddmlIntegerParser(new IntegerDataType());
            parser.Parse("1").Should().Be(1);
            parser.Parse("9999").Should().Be(9999);
            parser.Parse("0").Should().Be(0);
            parser.Parse("-1").Should().Be(-1);
            parser.Parse("-123456").Should().Be(-123456);
        }

        [Fact]
        public void ShouldParseIntegerWithLeadingZeros()
        {
            AddmlIntegerParser parser = new AddmlIntegerParser(new IntegerDataType());
            parser.Parse("01").Should().Be(1);
            parser.Parse("0000099").Should().Be(99);
            parser.Parse("-010").Should().Be(-10);
        }

        [Fact]
        public void ShouldParseIntegerWithThousandSeparator()
        {
            AddmlIntegerParser parser = new AddmlIntegerParser(new IntegerDataType());
            parser.Parse("1.000").Should().Be(1000);
            parser.Parse("1.000.000").Should().Be(1000000);
            parser.Parse("1.876.543.210").Should().Be(1876543210);
            parser.Parse("000.001.000").Should().Be(1000);
        }

        [Fact]
        public void ShouldParseIntegerWithSpace()
        {
            AddmlIntegerParser parser = new AddmlIntegerParser(new IntegerDataType());
            parser.Parse(" 1").Should().Be(1);
            parser.Parse("    99    ").Should().Be(99);
            parser.Parse("- 10").Should().Be(-10);
            parser.Parse("1 000").Should().Be(1000);
            parser.Parse(" 2 000 000 ").Should().Be(2000000);
        }

        [Fact]
        public void ShouldThrowExceptionIfInputIsNotANumber()
        {
            AddmlIntegerParser parser = new AddmlIntegerParser(new IntegerDataType());
            Assert.Throws<ArgumentException>(() => parser.Parse(""));
            Assert.Throws<ArgumentException>(() => parser.Parse("a"));
            Assert.Throws<ArgumentException>(() => parser.Parse("a"));
            Assert.Throws<ArgumentException>(() => parser.Parse("  abc "));
        }

        [Fact]
        public void ShouldParseIntegerInExponentialForm()
        {
            AddmlIntegerParser parser = new AddmlIntegerParser(new IntegerDataType("nnE+exp"));
            parser.Parse("1E+2").Should().Be(100);
            parser.Parse("2E+3").Should().Be(2000);
            parser.Parse("10E+0").Should().Be(10);
            parser.Parse("10E+1").Should().Be(100);
            parser.Parse("10e+1").Should().Be(100);
            parser.Parse("10e+01").Should().Be(100);

            parser.Parse("1E2").Should().Be(100);
            parser.Parse("2E3").Should().Be(2000);
        }

    }


}

