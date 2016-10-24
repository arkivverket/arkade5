using System;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core.Addml;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml
{
    public class FixedFormatReaderTest
    {

        [Fact]
        public void ShouldReadEmptyString()
        {
            StreamReader streamReader = CreateStreamReader("");
            int recordLength = 0;
            List<int> fieldLengths = new List<int>();

            FixedFormatReader reader = new FixedFormatReader(streamReader, recordLength, fieldLengths);

            reader.HasMoreRecords().Should().BeFalse();
            reader.GetNextValue().Should().BeEmpty();
        }

        [Fact]
        public void ShouldReadStringWithRecordLength5AndFieldLength1()
        {
            StreamReader streamReader = CreateStreamReader("123451234512345");
            int recordLength = 5;
            List<int> fieldLengths = new List<int> {1,1,1,1,1};

            FixedFormatReader reader = new FixedFormatReader(streamReader, recordLength, fieldLengths);
            for (int i = 0; i < 3; i++)
            {
                reader.HasMoreRecords().Should().BeTrue();
                reader.GetNextValue().Should().Equal(new List<string> {"1", "2", "3", "4", "5"});
            }
            reader.HasMoreRecords().Should().BeFalse();
        }

        [Fact]
        public void ShouldReadStringWithDifferentFieldLengths()
        {
            StreamReader streamReader = CreateStreamReader("122333444455555122333444455555122333444455555");
            int recordLength = 15;
            List<int> fieldLengths = new List<int> { 1, 2, 3, 4, 5 };

            FixedFormatReader reader = new FixedFormatReader(streamReader, recordLength, fieldLengths);
            for (int i = 0; i < 3; i++)
            {
                reader.HasMoreRecords().Should().BeTrue();
                reader.GetNextValue().Should().Equal(new List<string> { "1", "22", "333", "4444", "55555" });
            }
            reader.HasMoreRecords().Should().BeFalse();
        }

        [Fact]
        public void ShouldThrowExceptionIfSumOfFieldLengthsDoesNotMatchRecordLength()
        {
            StreamReader streamReader = CreateStreamReader("");
            int recordLength = 10;
            List<int> fieldLengths = new List<int> {1, 2, 4, 5};

            Assert.Throws<ArgumentException>(() => new FixedFormatReader(streamReader, recordLength, fieldLengths));
        }

        [Fact]
        public void ShouldThrowExceptionIfDataDoesNotCorrespondToRecordLength()
        {
            StreamReader streamReader = CreateStreamReader("123451234");
            int recordLength = 5;
            List<int> fieldLengths = new List<int> { 1, 1, 1, 1, 1};

            FixedFormatReader reader = new FixedFormatReader(streamReader, recordLength, fieldLengths);
            reader.GetNextValue();
            Assert.Throws<IOException>(() => reader.GetNextValue());
        }

        private StreamReader CreateStreamReader(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return new StreamReader(stream);
        }

    }
}
