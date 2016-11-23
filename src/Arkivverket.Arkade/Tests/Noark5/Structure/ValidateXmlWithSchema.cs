using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;

namespace Arkivverket.Arkade.Tests.Noark5.Structure
{
    /// <summary>
    ///     Validates that the XML is valid with regards to the XML schema. In this case the ADDML schema.
    /// </summary>
    public class ValidateXmlWithSchema : BaseTest
    {
        public ValidateXmlWithSchema(IArchiveContentReader archiveReader) : base(TestType.Structure, archiveReader)
        {
        }

        protected override void Test(Archive archive)
        {
            try
            {
                using (var validationReader = XmlReader.Create(ArchiveReader.GetStructureContentAsStream(archive), SetupXmlValidation()))
                {
                    while (validationReader.Read())
                    {
                    }
                }
                TestSuccess(new Location(archive.GetStructureDescriptionFileName()), $"Filen {archive.GetStructureDescriptionFileName()} er validert i henhold ADDML XML-skjema.");
            }
            catch (Exception e)
            {
                TestError(new Location(archive.GetStructureDescriptionFileName()), $"Filen {archive.GetStructureDescriptionFileName()} er ikke gyldig i henhold til ADDML XML-skjema:\n{e.Message}");
            }
        }

        public override void OnReadStartElementEvent(object sender, ReadElementEventArgs e)
        {
        }

        private static XmlReaderSettings SetupXmlValidation()
        {
            var settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += delegate(object sender, ValidationEventArgs vargs) { throw vargs.Exception; };
            settings.Schemas.Add("http://www.arkivverket.no/standarder/addml", GetPathToAddmlSchema());
            return settings;
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

    }
}