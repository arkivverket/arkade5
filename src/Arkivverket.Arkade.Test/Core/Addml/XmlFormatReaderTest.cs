using System.Collections.Generic;
using System.Text;
using System.Xml;
using Arkivverket.Arkade.Core.Addml;
using FluentAssertions;
using Xunit;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Test.Core.Addml
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

        [Fact]
        public void ShouldReadRealWorldNoark4Xml()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"<?xml version=""1.0"" encoding=""ISO-8859-1""?>");
            sb.AppendLine(@"<!DOCTYPE AVGRADKODE.TAB SYSTEM ""AVGRKODE.DTD"">");
            sb.AppendLine(@"<AVGRADKODE.TAB VERSJON=""1.0"">");
            sb.AppendLine(@"   <AVGRADKODE>");
            sb.AppendLine(@"      <AG.KODE>A</AG.KODE>");
            sb.AppendLine(@"      <AG.BETEGN>Avgraderes ved funksjon kjørt etter avgraderingstidspunkt</AG.BETEGN>");
            sb.AppendLine(@"   </AVGRADKODE>");
            sb.AppendLine(@"   <AVGRADKODE>");
            sb.AppendLine(@"      <AG.KODE>G</AG.KODE>");
            sb.AppendLine(@"      <AG.BETEGN>Gjennomgås for vurdering ved avgraderingstidspunkt</AG.BETEGN>");
            sb.AppendLine(@"   </AVGRADKODE>");
            sb.AppendLine(@"   <AVGRADKODE>");
            sb.AppendLine(@"      <AG.KODE>S</AG.KODE>");
            sb.AppendLine(@"      <AG.BETEGN>Sperrefrist, avgraderes automatisk på avgraderingsdato</AG.BETEGN>");
            sb.AppendLine(@"   </AVGRADKODE>");
            sb.AppendLine(@"   <AVGRADKODE>");
            sb.AppendLine(@"      <AG.KODE>AU</AG.KODE>");
            sb.AppendLine(@"      <AG.BETEGN>Avgradering utført</AG.BETEGN>");
            sb.AppendLine(@"   </AVGRADKODE>");
            sb.AppendLine(@"   <AVGRADKODE>");
            sb.AppendLine(@"      <AG.KODE>U</AG.KODE>");
            sb.AppendLine(@"      <AG.BETEGN>Unntatt fra automatisk avgradering</AG.BETEGN>");
            sb.AppendLine(@"   </AVGRADKODE>");
            sb.AppendLine(@"</AVGRADKODE.TAB>");

            XmlReader reader = XmlReaderUtil.Read(sb.ToString());
            string recordName = "AVGRADKODE";

            XmlFormatReader xmlFormatReader = new XmlFormatReader(reader, recordName);
            xmlFormatReader.HasNext().Should().BeTrue();
            xmlFormatReader.Next()
                .Should()
                .Equal(new Dictionary<string, string>
                {
                    {"AG.KODE", "A"},
                    {"AG.BETEGN", "Avgraderes ved funksjon kjørt etter avgraderingstidspunkt"}
                });
            xmlFormatReader.HasNext().Should().BeTrue();
            xmlFormatReader.Next()
                .Should()
                .Equal(new Dictionary<string, string>
                {
                    {"AG.KODE", "G"},
                    {"AG.BETEGN", "Gjennomgås for vurdering ved avgraderingstidspunkt"}
                });
            xmlFormatReader.HasNext().Should().BeTrue();
            xmlFormatReader.Next()
                .Should()
                .Equal(new Dictionary<string, string>
                {
                    {"AG.KODE", "S"},
                    {"AG.BETEGN", "Sperrefrist, avgraderes automatisk på avgraderingsdato"}
                });
            xmlFormatReader.HasNext().Should().BeTrue();
            xmlFormatReader.Next()
                .Should()
                .Equal(new Dictionary<string, string> {{"AG.KODE", "AU"}, {"AG.BETEGN", "Avgradering utført"}});
            xmlFormatReader.HasNext().Should().BeTrue();
            xmlFormatReader.Next()
                .Should()
                .Equal(new Dictionary<string, string>
                {
                    {"AG.KODE", "U"},
                    {"AG.BETEGN", "Unntatt fra automatisk avgradering"}
                });
            xmlFormatReader.HasNext().Should().BeFalse();
        }
  
    }
}