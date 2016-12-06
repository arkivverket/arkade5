using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Arkivverket.Arkade.Util
{
    public class XmlUtil
    {
        public static void Validate(string xmlString, string xmlSchemaString)
        {
            var xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString));
            var xmlSchemaStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlSchemaString));
            Validate(xmlStream, xmlSchemaStream);
        }

        public static void Validate(Stream xmlStream, Stream xmlSchemaStream)
        {
            XmlSchema xmlSchema = XmlSchema.Read(xmlSchemaStream, ValidationCallBack);
            XmlReaderSettings xmlReaderSettings = SetupXmlValidation(new List<XmlSchema> {xmlSchema});
            Validate(xmlStream, xmlReaderSettings);
        }

        public static void Validate(Stream xmlStream, string[] xmlSchemaResources)
        {
            var xmlSchemas = new List<XmlSchema>();
            foreach (string xmlSchemaResource in xmlSchemaResources)
            {
                xmlSchemas.Add(XmlSchema.Read(ResourceUtil.GetResourceAsStream(xmlSchemaResource), ValidationCallBack));
            }

            XmlReaderSettings xmlReaderSettings = SetupXmlValidation(xmlSchemas);
            Validate(xmlStream, xmlReaderSettings);
        }

        private static void Validate(Stream xmlStream, XmlReaderSettings xmlReaderSettings)
        {
            using (XmlReader validationReader = XmlReader.Create(xmlStream, xmlReaderSettings))
            {
                while (validationReader.Read())
                {
                }
            }
        }

        private static XmlReaderSettings SetupXmlValidation(IEnumerable<XmlSchema> xmlSchemas)
        {
            var settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += ValidationCallBack;

            foreach (XmlSchema xmlSchema in xmlSchemas)
            {
                settings.Schemas.Add(xmlSchema);
            }

            return settings;
        }

        private static void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            // TODO: Gather all problems
            throw args.Exception;
        }
    }
}