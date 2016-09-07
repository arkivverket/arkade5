using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests.Noark5.Structure
{
    /// <summary>
    ///     Validates that the XML is valid with regards to the XML schema. In this case the ADDML schema.
    /// </summary>
    public class ValidateXmlWithSchema : BaseTest
    {
        public ValidateXmlWithSchema() : base(TestType.Structure)
        {
        }

        protected override void Test(ArchiveExtraction archive)
        {
            try
            {
                ValidateXmlDocument(archive.GetStructureDescriptionFileName(), GetPathToAddmlSchema());
                TestSuccess($"Validated XML file {archive.GetStructureDescriptionFileName()} with ADDML schema.");
            }
            catch (Exception e)
            {
                TestError($"Error while validating xml [{archive.GetStructureDescriptionFileName()}] with ADDML schema: {e.Message}");
            }

        }

        private static string GetPathToAddmlSchema()
        {
            return AppDomain.CurrentDomain.BaseDirectory
                + Path.DirectorySeparatorChar
                + "ExternalModels"
                + Path.DirectorySeparatorChar
                + "xsd" 
                + Path.DirectorySeparatorChar
                + "addml.xsd";
        }


        private void ValidateXmlDocument(string documentToValidateFileName, string schemaFileName)
        {
            XmlSchema schema;
            using (var schemaReader = XmlReader.Create(schemaFileName))
            {
                schema = XmlSchema.Read(schemaReader, ValidationEventHandler);
            }

            var schemas = new XmlSchemaSet();
            schemas.Add(schema);

            var settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = schemas;
            settings.ValidationFlags =
                XmlSchemaValidationFlags.ProcessIdentityConstraints |
                XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += ValidationEventHandler;

            using (var validationReader = XmlReader.Create(documentToValidateFileName, settings))
            {
                while (validationReader.Read())
                {
                }
            }
        }

        private static void ValidationEventHandler(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Error)
            {
                throw args.Exception;
            }

            Debug.WriteLine(args.Message);
        }
    }
}