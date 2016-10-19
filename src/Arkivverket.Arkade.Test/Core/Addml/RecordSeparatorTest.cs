using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core.Addml.Definitions;
using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Test.Core.Addml
{
    public class RecordSeparatorTest
    {

        [Fact]
        public void ShouldConvertCrlfRecordSeparator()
        {
            RecordSeparator recordSeparator = new RecordSeparator("CRLF");
            recordSeparator.ToString().Should().Be("CRLF");
            recordSeparator.Get().Should().Be("\r\n");
        }

        [Fact]
        public void ShouldNotConvertOtherRecordSeparator()
        {
            RecordSeparator recordSeparator = new RecordSeparator("X");
            recordSeparator.ToString().Should().Be("X");
            recordSeparator.Get().Should().Be("X");
        }

    }
}
