using Arkivverket.Arkade.Test.Core;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Arkivverket.Arkade.Util
{
    public class XmlUtil
    {

        public static void Validate(String xmlString, String xmlSchemaString)
        {
            MemoryStream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString));
            MemoryStream xmlSchemaStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlSchemaString));
            Validate(xmlStream, xmlSchemaStream);
        }

        public static void Validate(Stream xmlStream, Stream xmlSchemaStream)
        {
            XmlSchema xmlSchema = XmlSchema.Read(xmlSchemaStream, new ValidationEventHandler(ValidationCallBack));
            using (var validationReader = XmlReader.Create(xmlStream, SetupXmlValidation(xmlSchema)))
            {
                while (validationReader.Read())
                {
                }
            }
        }

        private static XmlReaderSettings SetupXmlValidation(XmlSchema xmlSchema)
        {
            var settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
            settings.Schemas.Add(xmlSchema);
            return settings;
        }

        private static void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            // TODO: Gather all problems
            throw args.Exception;
        }

    }
}
