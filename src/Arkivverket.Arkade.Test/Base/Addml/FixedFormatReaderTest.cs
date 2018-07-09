using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Addml;
using Arkivverket.Arkade.Core.Base.Addml.Definitions;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Base.Addml
{
    public class FixedFormatReaderTest
    {

        [Fact]
        public void ShouldReadEmptyString()
        {
            StreamReader streamReader = CreateStreamReader("");
            int recordLength = 0;
            List<int> fieldLengths = new List<int>();
            var fixedFormatDefinition = CreateFixedFormatDefinition(recordLength, fieldLengths);

            FixedFormatReader reader = new FixedFormatReader(streamReader, fixedFormatDefinition);

            reader.HasMoreRecords().Should().BeFalse();
            var value = reader.GetNextValue();
            reader.HasMoreRecords().Should().BeFalse();

            value.Item1.Should().Be("");
            value.Item2.Should().BeEmpty();
        }

        [Fact]
        public void ShouldReadStringWithDifferentFieldLengths()
        {
            StreamReader streamReader = CreateStreamReader("122333444455555122333444455555122333444455555");
            int recordLength = 15;
            List<int> fieldLengths = new List<int> {1, 2, 3, 4, 5};
            var fixedFormatDefinition = CreateFixedFormatDefinition(recordLength, fieldLengths);

            FixedFormatReader reader = new FixedFormatReader(streamReader, fixedFormatDefinition);
            for (int i = 0; i < 3; i++)
            {
                reader.HasMoreRecords().Should().BeTrue();
                var value = reader.GetNextValue();
                value.Item1.Should().Be("");
                value.Item2.Should().Equal(new List<string> {"1", "22", "333", "4444", "55555"});
            }
            reader.HasMoreRecords().Should().BeFalse();
        }

        [Fact]
        public void ShouldReadStringWithRecordLength5AndFieldLength1()
        {
            StreamReader streamReader = CreateStreamReader("123451234512345");
            int recordLength = 5;
            List<int> fieldLengths = new List<int> {1, 1, 1, 1, 1};
            var fixedFormatDefinition = CreateFixedFormatDefinition(recordLength, fieldLengths);

            FixedFormatReader reader = new FixedFormatReader(streamReader, fixedFormatDefinition);
            for (int i = 0; i < 3; i++)
            {
                reader.HasMoreRecords().Should().BeTrue();
                var value = reader.GetNextValue();
                value.Item1.Should().Be("");
                value.Item2.Should().Equal(new List<string> {"1", "2", "3", "4", "5"});
            }
            reader.HasMoreRecords().Should().BeFalse();
        }

        [Fact]
        public void ShouldThrowExceptionIfDataDoesNotCorrespondToRecordLength()
        {
            StreamReader streamReader = CreateStreamReader("123451234");
            int recordLength = 5;
            List<int> fieldLengths = new List<int> {1, 1, 1, 1, 1};
            var fixedFormatDefinition = CreateFixedFormatDefinition(recordLength, fieldLengths);

            FixedFormatReader reader = new FixedFormatReader(streamReader, fixedFormatDefinition);
            reader.GetNextValue();
            Assert.Throws<IOException>(() => reader.GetNextValue());
        }

        [Fact]
        public void ShouldThrowExceptionIfSumOfFieldLengthsDoesNotMatchRecordLength()
        {
            StreamReader streamReader = CreateStreamReader("");
            int recordLength = 10;
            List<int> fieldLengths = new List<int> {1, 2, 4, 5};
            var fixedFormatDefinition = CreateFixedFormatDefinition(recordLength, fieldLengths);

            Assert.Throws<ArkadeException>(() => new FixedFormatReader(streamReader, fixedFormatDefinition));
        }

        private StreamReader CreateStreamReader(string s)
        {
            return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(s)));
        }

        private FixedFormatReader.FixedFormatConfig CreateFixedFormatDefinition(int recordLength,
            List<int> fieldLengths)
        {
            FixedFormatReader.FixedFormatConfig fixedFormatConfig =
                new FixedFormatReader.FixedFormatConfig();
            fixedFormatConfig.RecordLength = recordLength;
            List<FixedFormatReader.FixedFormatRecordConfig> recordDefinitions =
                new List<FixedFormatReader.FixedFormatRecordConfig>();
            FixedFormatReader.FixedFormatRecordConfig fixedFormatRecordConfig =
                new FixedFormatReader.FixedFormatRecordConfig();
            fixedFormatRecordConfig.FieldLengths = fieldLengths;
            fixedFormatRecordConfig.RecordIndex = new RecordIndex("flatfileDefName", "recordDefName");
            recordDefinitions.Add(fixedFormatRecordConfig);
            fixedFormatConfig.RecordDefinitions = recordDefinitions;
            return fixedFormatConfig;
        }

    }
}