using System;
using FluentAssertions;
using Xunit;
using Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Definitions.DataTypes
{
    public class IntegerDataTypeTest
    {

        [Fact]
        public void ShouldAcceptNullAsFieldFormat()
        {
            IntegerDataType dataType = new IntegerDataType(null, null);
            dataType.GetThousandSeparator().Should().BeNull();
        }

        [Fact]
        public void ShouldAcceptExponentAsFieldFormat()
        {
            new IntegerDataType("nnE+exp", null);
        }

        [Fact]
        public void ShouldAcceptDotAsThousandSeparator()
        {
            IntegerDataType dataType = new IntegerDataType("n.nnn", null);
            dataType.GetThousandSeparator().Should().Be(".");
        }

        [Fact]
        public void ShouldAcceptCommaAsThousandSeparator()
        {
            IntegerDataType dataType = new IntegerDataType("n,nnn", null);
            dataType.GetThousandSeparator().Should().Be(",");
        }

        [Fact]
        public void ShouldAcceptWhiteSpaceAsThousandSeparator()
        {
            IntegerDataType dataType = new IntegerDataType("n nnn", null);
            dataType.GetThousandSeparator().Should().Be(" ");
        }

        [Fact]
        public void ShouldNotAcceptOtherCharactersAsThousandSeparator()
        {
            Assert.Throws<ArgumentException>(() => new IntegerDataType("n'nnn", null));
            Assert.Throws<ArgumentException>(() => new IntegerDataType("n#nnn", null));
            Assert.Throws<ArgumentException>(() => new IntegerDataType("n..nnn", null));
            Assert.Throws<ArgumentException>(() => new IntegerDataType("n,,nnn", null));
        }

        [Fact]
        public void ShouldOnlyAcceptNDotNn()
        {
            // Only accept "n.nnn"
            Assert.Throws<ArgumentException>(() => new IntegerDataType("nn.nnn", null));
            Assert.Throws<ArgumentException>(() => new IntegerDataType("nnn.nnn", null));
            Assert.Throws<ArgumentException>(() => new IntegerDataType("nnn.n", null));
            Assert.Throws<ArgumentException>(() => new IntegerDataType("n.nnn.nnn", null));
            Assert.Throws<ArgumentException>(() => new IntegerDataType("d.ddd", null));
            Assert.Throws<ArgumentException>(() => new IntegerDataType("n,nnn.nnn", null));
            Assert.Throws<ArgumentException>(() => new IntegerDataType("n nnn.nnn", null));
            Assert.Throws<ArgumentException>(() => new IntegerDataType("n.nnn nnn", null));
            Assert.Throws<ArgumentException>(() => new IntegerDataType("n,nnn nnn", null));
        }

        [Fact]
        public void IsValidShouldIgnoreSpace()
        {
            new IntegerDataType(null, null).IsValid("1 000", true).Should().BeTrue();
            new IntegerDataType("n.nnn", null).IsValid("1.0 00", true).Should().BeTrue();
            new IntegerDataType("nnE+exp", null).IsValid("4 E+5", true).Should().BeTrue();
        }

        [Fact]
        public void IsValidShouldReturnTrueWithThousandSeparatorPlacedCorrect()
        {
            new IntegerDataType("n.nnn", null).IsValid("1", true).Should().BeTrue();
            new IntegerDataType("n.nnn", null).IsValid("12", true).Should().BeTrue();
            new IntegerDataType("n.nnn", null).IsValid("123", true).Should().BeTrue();
            new IntegerDataType("n.nnn", null).IsValid("4.000", true).Should().BeTrue();
            new IntegerDataType("n.nnn", null).IsValid("111.000", true).Should().BeTrue();
            new IntegerDataType("n.nnn", null).IsValid("1.111.000", true).Should().BeTrue();
            new IntegerDataType("n.nnn", null).IsValid("1.111.000", true).Should().BeTrue();
            new IntegerDataType("n.nnn", null).IsValid("22.111.000", true).Should().BeTrue();
            new IntegerDataType("n.nnn", null).IsValid("222.111.000", true).Should().BeTrue();
            new IntegerDataType("n.nnn", null).IsValid("3.222.111.000", true).Should().BeTrue();
            new IntegerDataType("n.nnn", null).IsValid("-3.222.111.000", true).Should().BeTrue();

            new IntegerDataType("n,nnn", null).IsValid("1", true).Should().BeTrue();
            new IntegerDataType("n,nnn", null).IsValid("12", true).Should().BeTrue();
            new IntegerDataType("n,nnn", null).IsValid("123", true).Should().BeTrue();
            new IntegerDataType("n,nnn", null).IsValid("4,000", true).Should().BeTrue();
            new IntegerDataType("n,nnn", null).IsValid("111,000", true).Should().BeTrue();
            new IntegerDataType("n,nnn", null).IsValid("1,111,000", true).Should().BeTrue();
            new IntegerDataType("n,nnn", null).IsValid("1,111,000", true).Should().BeTrue();
            new IntegerDataType("n,nnn", null).IsValid("22,111,000", true).Should().BeTrue();
            new IntegerDataType("n,nnn", null).IsValid("222,111,000", true).Should().BeTrue();
            new IntegerDataType("n,nnn", null).IsValid("3,222,111,000", true).Should().BeTrue();
            new IntegerDataType("n,nnn", null).IsValid("-3,222,111,000", true).Should().BeTrue();

            new IntegerDataType("n nnn", null).IsValid("1", true).Should().BeTrue();
            new IntegerDataType("n nnn", null).IsValid("12", true).Should().BeTrue();
            new IntegerDataType("n nnn", null).IsValid("123", true).Should().BeTrue();
            new IntegerDataType("n nnn", null).IsValid("4 000", true).Should().BeTrue();
            new IntegerDataType("n nnn", null).IsValid("111 000", true).Should().BeTrue();
            new IntegerDataType("n nnn", null).IsValid("1 111 000", true).Should().BeTrue();
            new IntegerDataType("n nnn", null).IsValid("1 111 000", true).Should().BeTrue();
            new IntegerDataType("n nnn", null).IsValid("22 111 000", true).Should().BeTrue();
            new IntegerDataType("n nnn", null).IsValid("222 111 000", true).Should().BeTrue();
            new IntegerDataType("n nnn", null).IsValid("3 222 111 000", true).Should().BeTrue();
            new IntegerDataType("n nnn", null).IsValid("-3 222 111 000", true).Should().BeTrue();
        }

        [Fact]
        public void IsValidShouldReturnFalseWithThousandSeparatorPlacedIncorrect()
        {
            new IntegerDataType("n.nnn", null).IsValid(".1", true).Should().BeFalse();
            new IntegerDataType("n.nnn", null).IsValid("1.2", true).Should().BeFalse();
            new IntegerDataType("n.nnn", null).IsValid("1.23", true).Should().BeFalse();
            new IntegerDataType("n.nnn", null).IsValid("1.23.", true).Should().BeFalse();
            new IntegerDataType("n.nnn", null).IsValid("1.23.4", true).Should().BeFalse();
            new IntegerDataType("n.nnn", null).IsValid("1.234.", true).Should().BeFalse();
            new IntegerDataType("n.nnn", null).IsValid("1.234.5", true).Should().BeFalse();
            new IntegerDataType("n.nnn", null).IsValid(".123.456", true).Should().BeFalse();

            new IntegerDataType("n,nnn", null).IsValid(",1", true).Should().BeFalse();
            new IntegerDataType("n,nnn", null).IsValid("1,2", true).Should().BeFalse();
            new IntegerDataType("n,nnn", null).IsValid("1,23", true).Should().BeFalse();
            new IntegerDataType("n,nnn", null).IsValid("1,23,", true).Should().BeFalse();
            new IntegerDataType("n,nnn", null).IsValid("1,23,4", true).Should().BeFalse();
            new IntegerDataType("n,nnn", null).IsValid("1,234,", true).Should().BeFalse();
            new IntegerDataType("n,nnn", null).IsValid("1,234,5", true).Should().BeFalse();
            new IntegerDataType("n,nnn", null).IsValid(",123,456", true).Should().BeFalse();
        }

        [Fact]
        public void IsValidShouldReturnFalseIfDecimalSeparatorIsInUse()
        {
            new IntegerDataType("n.nnn", null).IsValid("1,1", true).Should().BeFalse();
            new IntegerDataType("n.nnn", null).IsValid("12,1", true).Should().BeFalse();
            new IntegerDataType("n.nnn", null).IsValid("123,1", true).Should().BeFalse();
            new IntegerDataType("n.nnn", null).IsValid("4.000,1", true).Should().BeFalse();
            new IntegerDataType("n.nnn", null).IsValid("111.000,1", true).Should().BeFalse();
            new IntegerDataType("n.nnn", null).IsValid("1.111.000,1", true).Should().BeFalse();
            new IntegerDataType("n.nnn", null).IsValid("1.111.000,1", true).Should().BeFalse();
            new IntegerDataType("n.nnn", null).IsValid("22.111.000,1", true).Should().BeFalse();
            new IntegerDataType("n.nnn", null).IsValid("222.111.000,1", true).Should().BeFalse();
            new IntegerDataType("n.nnn", null).IsValid("3.222.111.000,1", true).Should().BeFalse();
            new IntegerDataType("n.nnn", null).IsValid("-3.222.111.000,1", true).Should().BeFalse();
        }

        [Fact]
        public void IsValidShouldReturnTrueWithCorrectExponentFormat()
        {
            new IntegerDataType("nnE+exp", null).IsValid("4E+5", true).Should().BeTrue();
            new IntegerDataType("nnE+exp", null).IsValid("123E+4", true).Should().BeTrue();
            new IntegerDataType("nnE+exp", null).IsValid("123E+10", true).Should().BeTrue();
            new IntegerDataType("nnE+exp", null).IsValid("-123E+10", true).Should().BeTrue();
        }

        [Fact]
        public void IsValidShouldReturnFalseWithIncorrectExponentFormat()
        {
            new IntegerDataType("nnE+exp", null).IsValid("E+5", true).Should().BeFalse();
            new IntegerDataType("nnE+exp", null).IsValid("1E+", true).Should().BeFalse();
            new IntegerDataType("nnE+exp", null).IsValid("4E-2", true).Should().BeFalse();

            // Supported by BigInteger.Parse, but not according to ADDML spec
            new IntegerDataType("nnE+exp", null).IsValid("4e+5", true).Should().BeFalse();
            new IntegerDataType("nnE+exp", null).IsValid("4E5", true).Should().BeFalse();
            new IntegerDataType("nnE+exp", null).IsValid("400E-2", true).Should().BeFalse();
            new IntegerDataType("nnE+exp", null).IsValid("1", true).Should().BeFalse();
        }

    }
}
