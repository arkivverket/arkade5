using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Arkivverket.Arkade.Core.Base;
using Serilog;

namespace Arkivverket.Arkade.Core.Util
{
    public class XmlValidator
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

        private const int ValidationErrorCountLimit = 100;
        private readonly List<string> _validationErrorMessages = new List<string>();

        public List<string> Validate(string xmlString, string xmlSchemaString)
        {
            var xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlString));
            var xmlSchemaStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlSchemaString));
            return Validate(xmlStream, xmlSchemaStream);
        }

        public List<string> Validate(Stream xmlStream, Stream xmlSchemaStream)
        {
            XmlSchema xmlSchema = XmlSchema.Read(xmlSchemaStream, ValidationCallBack);
            XmlReaderSettings xmlReaderSettings = SetupXmlValidation(new List<XmlSchema> {xmlSchema});
            Validate(xmlStream, xmlReaderSettings);

            return _validationErrorMessages;
        }

        public List<string> Validate(Stream xmlStream, Stream[] xmlSchemaStreams, string xmlFileName)
        {
            var xmlSchemas = new List<XmlSchema>();

            foreach (Stream xmlSchemaStream in xmlSchemaStreams)
                xmlSchemas.Add(XmlSchema.Read(xmlSchemaStream, ValidationCallBack));

            XmlReaderSettings xmlReaderSettings = SetupXmlValidation(xmlSchemas, xmlFileName);
            Validate(xmlStream, xmlReaderSettings);

            return _validationErrorMessages;
        }

        private void Validate(Stream xmlStream, XmlReaderSettings xmlReaderSettings)
        {
            using (XmlReader validationReader = XmlReader.Create(xmlStream, xmlReaderSettings))
            {
                while (validationReader.Read())
                    if (_validationErrorMessages.Count >= ValidationErrorCountLimit)
                        break;
            }
        }

        public IEnumerable<string> Validate(ArchiveXmlUnit xmlUnit)
        {
            Stream xmlStream = xmlUnit.File.AsStream();

            Stream[] xmlSchemaStreams = xmlUnit.Schemas.Select(s => s.AsStream()).ToArray();

            return Validate(xmlStream, xmlSchemaStreams, xmlUnit.File.Name);
        }

        private XmlReaderSettings SetupXmlValidation(IEnumerable<XmlSchema> xmlSchemas, string xmlFileName = null)
        {
            var settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += ValidationCallBack;

            xmlFileName = string.IsNullOrWhiteSpace(xmlFileName) ? " " : $" {xmlFileName} ";

            foreach (XmlSchema xmlSchema in xmlSchemas)
            {
                Log.Information(string.IsNullOrWhiteSpace(xmlSchema.Version)
                    ? $"Validating{xmlFileName}against {xmlSchema.TargetNamespace} with unspecified version."
                    : $"Validating{xmlFileName}against {xmlSchema.TargetNamespace} with version {xmlSchema.Version}.");
                settings.Schemas.Add(xmlSchema);
            }

            return settings;
        }

        private void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            _validationErrorMessages.Add(string.Format(
                Resources.ExceptionMessages.XmlValidationErrorMessage,
                args.Exception.LineNumber,
                args.Message
            ));
        }
    }
}
