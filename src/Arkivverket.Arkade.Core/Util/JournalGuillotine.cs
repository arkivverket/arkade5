using System;
using System.Xml;
using System.Xml.Serialization;

namespace Arkivverket.Arkade.Core.Util
{
    public static class JournalGuillotine
    {
        public static JournalHead Behead(ArkadeFile journalXmlFile)
        {
            var xmlTextReader = new XmlTextReader(journalXmlFile.FullName) {Namespaces = false};

            while (!xmlTextReader.Name.Equals("journalhode"))
                xmlTextReader.Read();

            string journalHeadXml = xmlTextReader.ReadOuterXml();

            xmlTextReader.Close();

            JournalHead journalHead = SerializeUtil.DeserializeFromString<JournalHead>(journalHeadXml);

            return journalHead;
        }
    }

    [XmlType("journalhode")]
    public class JournalHead
    {
        [XmlElement("journalStartDato")]
        public DateTime JournalStartDate { get; set; }

        [XmlElement("journalSluttDato")]
        public DateTime JournalEndDate { get; set; }

        [XmlElement("antallJournalposter")]
        public int NumberOfJournalposts { get; set; }
    }
}
