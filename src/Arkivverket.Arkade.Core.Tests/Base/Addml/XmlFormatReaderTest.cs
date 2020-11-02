using System.Collections.Generic;
using System.Xml;
using Arkivverket.Arkade.Core.Base.Addml;
using FluentAssertions;
using Xunit;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Tests.Base.Addml
{
    public class XmlFormatReaderTest
    {
        [Fact]
        public void ShouldReadSimpleXml()
        {
            XmlReader reader = XmlReaderUtil.Read(
                "<r>" +
                "  <o>" +
                "    <i1>1</i1>" +
                "    <i2>2</i2>" +
                "  </o>" +
                "  <o>" +
                "    <i1>3</i1>" +
                "    <i2>4</i2>" +
                "  </o>" +
                "</r>"
            );
            string recordName = "o";

            XmlFormatReader xmlFormatReader = new XmlFormatReader(reader, recordName);
            xmlFormatReader.HasNext().Should().BeTrue();
            xmlFormatReader.Next().Should().Equal(new Dictionary<string, string> {{"i1", "1"}, {"i2", "2"}});
            xmlFormatReader.HasNext().Should().BeTrue();
            xmlFormatReader.Next().Should().Equal(new Dictionary<string, string> {{"i1", "3"}, {"i2", "4"}});
            xmlFormatReader.HasNext().Should().BeFalse();
        }
    }
}