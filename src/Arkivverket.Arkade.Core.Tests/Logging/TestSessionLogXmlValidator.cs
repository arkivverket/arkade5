using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Arkivverket.Arkade.Core.Tests.Logging
{
    class TestSessionLogXmlValidator
    {
        private static string schemaFileName = $"{AppDomain.CurrentDomain.BaseDirectory}\\..\\..\\..\\Arkivverket.Arkade\\ExternalModels\\xsd\\testSessionLog.xsd";
        private static string schemaNamespace = "http://www.arkivverket.no/dataextracttools/arkade5/testsessionlog";

        public static void Validate(string xml)
        {
            List<string> validationErrors = GetValidationErrors(xml);
            if (validationErrors.Count == 0)
            {
                return;
            } else { 
                string errorMessage = "";
                foreach (string error in validationErrors)
                {
                    errorMessage += error + Environment.NewLine;
                }

                throw new XmlSchemaValidationException(errorMessage);
            }
        }

        private static List<string> GetValidationErrors(string xml)
        {

            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(schemaNamespace, schemaFileName);

            XDocument doc = XDocument.Parse(xml);

            List<string> errors = new List<string>();
            doc.Validate(schemas, (sender, args) =>
            {
                errors.Add(args.Message);
            });

            return errors;


            /*
                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.ValidationType = ValidationType.Schema;
                        settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                        settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                        settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                        settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

                        XmlReader reader = XmlReader.Create("inlineSchema.xml", settings);

                        while (reader.Read());

                    }

                    private static void ValidationCallBack(object sender, ValidationEventArgs args)
                    {
                        if (args.Severity == XmlSeverityType.Warning)
                            Console.WriteLine("\tWarning: Matching schema not found.  No validation occurred." + args.Message);
                        else
                            Console.WriteLine("\tValidation error: " + args.Message);

                    }
                    */
        }
    }
}
