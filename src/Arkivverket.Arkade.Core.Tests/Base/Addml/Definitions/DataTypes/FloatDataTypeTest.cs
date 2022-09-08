using System;
using FluentAssertions;
using Xunit;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml.Definitions.DataTypes
{
    public class FloatDataTypeTest
    {

        [Fact]
        public void ShouldAcceptNullAsFieldFormat()
        {
            new FloatDataType(null, null);
        }

        [Fact]
        public void ShouldAcceptDecimalAsFieldFormat()
        {
            new FloatDataType("nn,nn", null);
            new FloatDataType("nn.nn", null);
        }

        [Fact]
        public void ShouldAcceptDecimalAndThousandAsFieldFormat()
        {
            new FloatDataType("n.nnn,nn", null);
            new FloatDataType("n nnn,nn", null);
            new FloatDataType("n,nnn.nn", null);
            new FloatDataType("n nnn.nn", null);
        }

        [Fact]
        public void ShouldNotAcceptOtherFieldFormats()
        {
            Assert.Throws<ArgumentException>(() => new FloatDataType("", null));
            Assert.Throws<ArgumentException>(() => new FloatDataType("n", null));
            Assert.Throws<ArgumentException>(() => new FloatDataType("n'nnn", null));
            Assert.Throws<ArgumentException>(() => new FloatDataType("n#nnn", null));
            Assert.Throws<ArgumentException>(() => new FloatDataType("n..nnn", null));
            Assert.Throws<ArgumentException>(() => new FloatDataType("n,,nnn", null));
            Assert.Throws<ArgumentException>(() => new FloatDataType("n.nnn,", null));
            Assert.Throws<ArgumentException>(() => new FloatDataType(".nnn,nn", null));
            Assert.Throws<ArgumentException>(() => new FloatDataType("n.nnn,nnn", null));
        }


        [Fact]
        public void ShouldAcceptBothDecimalAndThousandSeparatorWhenFieldFormatIsNull()
        {
            new FloatDataType(null, null).IsValid("1", true).Should().BeTrue();
            new FloatDataType(null, null).IsValid("1,1", true).Should().BeTrue();
            new FloatDataType(null, null).IsValid("1.1", true).Should().BeTrue();
            new FloatDataType(null, null).IsValid("1.111.000,1234567890", true).Should().BeTrue();
            new FloatDataType(null, null).IsValid("1,111,000.1234567890", true).Should().BeTrue();
        }

        [Fact]
        public void IsValidShouldReturnTrueWithDecimalAndThousandSeparatorPlacedCorrect()
        {
            new FloatDataType("n.nnn,nn", null).IsValid("1", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("12", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("123", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("4.000", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("111.000", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("1.111.000", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("1.111.000", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("22.111.000", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("222.111.000", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("3.222.111.000", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("-3.222.111.000", true).Should().BeTrue();

            new FloatDataType("n nnn,nn", null).IsValid("1", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("12", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("123", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("4 000", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("111 000", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("1 111 000", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("1 111 000", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("22 111 000", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("222 111 000", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("3 222 111 000", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("-3 222 111 000", true).Should().BeTrue();

            new FloatDataType("n,nnn.nn", null).IsValid("1", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("12", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("123", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("4,000", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("111,000", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("1,111,000", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("1,111,000", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("22,111,000", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("222,111,000", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("3,222,111,000", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("-3,222,111,000", true).Should().BeTrue();

            new FloatDataType("n nnn.nn", null).IsValid("1", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("12", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("123", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("4 000", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("111 000", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("1 111 000", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("1 111 000", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("22 111 000", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("222 111 000", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("3 222 111 000", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("-3 222 111 000", true).Should().BeTrue();

            new FloatDataType("n.nnn,nn", null).IsValid("1,1", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("12,1", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("123,1", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("4.000,1", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("111.000,1", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("1.111.000,1", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("1.111.000,1", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("22.111.000,1", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("222.111.000,1", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("3.222.111.000,1", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("-3.222.111.000,1", true).Should().BeTrue();

            new FloatDataType("n nnn,nn", null).IsValid("1,1", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("12,1", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("123,1", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("4 000,1", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("111 000,1", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("1 111 000,1", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("1 111 000,1", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("22 111 000,1", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("222 111 000,1", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("3 222 111 000,1", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("-3 222 111 000,1", true).Should().BeTrue();

            new FloatDataType("n,nnn.nn", null).IsValid("1.1", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("12.1", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("123.1", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("4,000.1", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("111,000.1", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("1,111,000.1", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("1,111,000.1", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("22,111,000.1", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("222,111,000.1", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("3,222,111,000.1", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("-3,222,111,000.1", true).Should().BeTrue();

            new FloatDataType("n nnn.nn", null).IsValid("1.1", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("12.1", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("123.1", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("4 000.1", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("111 000.1", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("1 111 000.1", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("1 111 000.1", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("22 111 000.1", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("222 111 000.1", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("3 222 111 000.1", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("-3 222 111 000.1", true).Should().BeTrue();

            new FloatDataType("n.nnn,nn", null).IsValid("1,1234567890", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("1.111.000,1234567890", true).Should().BeTrue();

            new FloatDataType("n nnn,nn", null).IsValid("1,1234567890", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("1 111 000,1234567890", true).Should().BeTrue();

            new FloatDataType("n,nnn.nn", null).IsValid("1.1234567890", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("1,111,000.1234567890", true).Should().BeTrue();

            new FloatDataType("n nnn.nn", null).IsValid("1.1234567890", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("1 111 000.1234567890", true).Should().BeTrue();
        }

        [Fact]
        public void IsValidShouldReturnFalseWithThousandSeparatorPlacedIncorrect()
        {
            new FloatDataType("n.nnn,nn", null).IsValid(".1,1", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid(".123.456,1", true).Should().BeFalse();

            new FloatDataType("n nnn,nn", null).IsValid("1,1 1", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid("123 456,1 1", true).Should().BeFalse();

            new FloatDataType("n,nnn.nn", null).IsValid(",1.1", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid(",123,456.1", true).Should().BeFalse();

            new FloatDataType("n nnn.nn", null).IsValid("1.1 1", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid("123 456.1 1", true).Should().BeFalse();
        }

        [Fact]
        public void DecimalParseThinksTheseArOkSoWeGoWithThat()
        {
            new FloatDataType("n.nnn,nn", null).IsValid("1.2,1", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("1.23,1", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("1.23.,1", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("1.23.4,1", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("1.234.,1", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("1.234.5,1", true).Should().BeTrue();
            new FloatDataType("n.nnn,nn", null).IsValid("1.234.51", true).Should().BeTrue();

            new FloatDataType("n nnn,nn", null).IsValid("1 2,1", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("1 23,1", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("1 23 ,1", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("1 23 4,1", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("1 234 ,1", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("1 234 5,1", true).Should().BeTrue();
            new FloatDataType("n nnn,nn", null).IsValid("1 234 51", true).Should().BeTrue();

            new FloatDataType("n,nnn.nn", null).IsValid("1,2.1", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("1,23.1", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("1,23,.1", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("1,23,4.1", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("1,234,.1", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("1,234,5.1", true).Should().BeTrue();
            new FloatDataType("n,nnn.nn", null).IsValid("1,234,51", true).Should().BeTrue();
            
            new FloatDataType("n nnn.nn", null).IsValid("1 2.1", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("1 23.1", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("1 23 .1", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("1 23 4.1", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("1 234 .1", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("1 234 5.1", true).Should().BeTrue();
            new FloatDataType("n nnn.nn", null).IsValid("1 234 51", true).Should().BeTrue();
        }

        [Fact]
        public void IsValidShouldReturnFalseWithDecimalSeparatorPlacedIncorrect()
        {
            new FloatDataType("n.nnn,nn", null).IsValid("1.000,1,", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid("1.000,1,0", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid("1.0,00,1", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid(",1.000", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid("1.000,,1", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid("1.000,,", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid(",,1.000", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid(",", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid(",,", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid(",1,", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid(",,1,", true).Should().BeFalse();

            new FloatDataType("n nnn,nn", null).IsValid("1 000,1,", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid("1 000,1,0", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid("1 0,00,1", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid(",1 000", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid("1 000,,1", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid("1 000,,", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid(",,1 000", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid(",", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid(",,", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid(",1,", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid(",,1,", true).Should().BeFalse();

            new FloatDataType("n,nnn.nn", null).IsValid("1,000.1.", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid("1,000.1.0", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid("1,0.00.1", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid(".1,000", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid("1,000..1", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid("1,000..", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid("..1,000", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid(".", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid("..", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid(".1.", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid("..1.", true).Should().BeFalse();

            new FloatDataType("n nnn.nn", null).IsValid("1 000.1.", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid("1 000.1.0", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid("1 0.00.1", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid(".1 000", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid("1 000..1", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid("1 000..", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid("..1 000", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid(".", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid("..", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid(".1.", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid("..1.", true).Should().BeFalse();
        }

        [Fact]
        public void IsValidShouldReturnFalseWhenNotANumber()
        {
            new FloatDataType("nn,nn", null).IsValid("A", true).Should().BeFalse();
            new FloatDataType("nn,nn", null).IsValid("1a", true).Should().BeFalse();
            new FloatDataType("nn,nn", null).IsValid("deadbeef", true).Should().BeFalse();
            new FloatDataType("nn,nn", null).IsValid("DEADBEEF,1", true).Should().BeFalse();

            new FloatDataType("nn.nn", null).IsValid("A", true).Should().BeFalse();
            new FloatDataType("nn.nn", null).IsValid("1a", true).Should().BeFalse();
            new FloatDataType("nn.nn", null).IsValid("deadbeef", true).Should().BeFalse();
            new FloatDataType("nn.nn", null).IsValid("DEADBEEF,1", true).Should().BeFalse();

            new FloatDataType("n.nnn,nn", null).IsValid("A", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid("1a", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid("deadbeef", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid("DEADBEEF,1", true).Should().BeFalse();

            new FloatDataType("n nnn,nn", null).IsValid("A", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid("1a", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid("deadbeef", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid("DEADBEEF,1", true).Should().BeFalse();

            new FloatDataType("n,nnn.nn", null).IsValid("A", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid("1a", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid("deadbeef", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid("DEADBEEF,1", true).Should().BeFalse();

            new FloatDataType("n nnn.nn", null).IsValid("A", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid("1a", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid("deadbeef", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid("DEADBEEF,1", true).Should().BeFalse();
        }

        [Fact]
        public void IsValidShouldReturnFalseWhenFormatMismatch()
        {
            new FloatDataType("nn,nn", null).IsValid("22.22", true).Should().BeFalse();
            new FloatDataType("nn,nn", null).IsValid("22.222,22", true).Should().BeFalse();

            new FloatDataType("nn.nn", null).IsValid("22,22", true).Should().BeFalse();
            new FloatDataType("nn.nn", null).IsValid("22,222.22", true).Should().BeFalse();

            new FloatDataType("n.nnn,nn", null).IsValid("1.111 11", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid("1,111.11", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid("1,111,11", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid("1,111 11", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid("1 111.11", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid("1 111 11", true).Should().BeFalse();
            new FloatDataType("n.nnn,nn", null).IsValid("1 111,11", true).Should().BeFalse();

            new FloatDataType("n nnn,nn", null).IsValid("1.111.11", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid("1.111,11", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid("1.111 11", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid("1,111.11", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid("1,111,11", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid("1,111 11", true).Should().BeFalse();
            new FloatDataType("n nnn,nn", null).IsValid("1 111.11", true).Should().BeFalse();

            new FloatDataType("n,nnn.nn", null).IsValid("1.111.11", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid("1.111,11", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid("1.111 11", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid("1,111 11", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid("1 111.11", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid("1 111,11", true).Should().BeFalse();
            new FloatDataType("n,nnn.nn", null).IsValid("1 111 11", true).Should().BeFalse();

            new FloatDataType("n nnn.nn", null).IsValid("1.111.11", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid("1.111,11", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid("1.111 11", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid("1,111,11", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid("1,111.11", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid("1,111 11", true).Should().BeFalse();
            new FloatDataType("n nnn.nn", null).IsValid("1 111,11", true).Should().BeFalse();
        }
    }
}
