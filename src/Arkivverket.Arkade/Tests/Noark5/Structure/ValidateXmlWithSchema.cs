using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5.Structure
{
    /// <summary>
    ///     Validates that the XML files (arkivuttrekk.xml and arkivstruktur.xml) are valid with regards to the XML schema.
    ///  </summary>
    public class ValidateXmlWithSchema : Noark5StructureBaseTest
    {
        private readonly IArchiveContentReader _archiveReader;
        private readonly List<TestResult> _testResults = new List<TestResult>();

        public ValidateXmlWithSchema(IArchiveContentReader archiveReader)
        {
            _archiveReader = archiveReader;
        }

        public override void Test(Archive archive)
        {
            ValidateXml(archive.GetStructureDescriptionFileName(), _archiveReader.GetStructureContentAsStream(archive),
                GetStructureDescriptionXmlSchemaFileName(archive));

            ValidateXml(archive.GetContentDescriptionFileName(), _archiveReader.GetContentAsStream(archive),
                GetContentDescriptionXmlSchemaFileName(archive), GetMetadataCatalogXmlSchemaFileName(archive));
        }

        private void ValidateXml(string fullPathToFile, Stream fileStream, params string[] xsdResources)
        {
            string fileName = Path.GetFileName(fullPathToFile);
            try
            {
                XmlUtil.Validate(fileStream, xsdResources);
                _testResults.Add(new TestResult(ResultType.Success, new Location(fileName), Noark5Messages.ValidateXmlWithSchemaMessageValid));
            }
            catch (Exception e)
            {
                string message = string.Format(Noark5Messages.ExceptionXmlDoesNotValidateWithSchema,
                    fileName, e.Message);
                throw new ArkadeException(message, e);
            }
        }

        private string GetStructureDescriptionXmlSchemaFileName(Archive archive)
        {
            if (archive.HasStructureDescriptionXmlSchema())
                return archive.GetStructureDescriptionXmlSchemaFileName();

            // Fallback on internal addml.xsd:

            _testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                // TODO: Consider ResultType.Error (or ResultType.Warning if it becomes supported)
                string.Format(Noark5Messages.InternalSchemaFileIsUsed, ArkadeConstants.AddmlXsdFileName)));

            return ArkadeConstants.AddmlXsdResource;
        }

        private string GetContentDescriptionXmlSchemaFileName(Archive archive)
        {
            if (archive.HasContentDescriptionXmlSchema())
                return archive.GetContentDescriptionXmlSchemaFileName();

            // Fallback on internal arkivstruktur.xsd:

            _testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                // TODO: Consider ResultType.Error (or ResultType.Warning if it becomes supported)
                string.Format(Noark5Messages.InternalSchemaFileIsUsed, ArkadeConstants.ArkivstrukturXsdFileName)));

            return ArkadeConstants.ArkivstrukturXsdResource;
        }

        private string GetMetadataCatalogXmlSchemaFileName(Archive archive)
        {
            if (archive.HasMetadataCatalogXmlSchema())
                return archive.GetMetadataCatalogXmlSchemaFileName();

            // Fallback on internal metadatakatalog.xsd:

            _testResults.Add(new TestResult(ResultType.Success, new Location(string.Empty),
                // TODO: Consider ResultType.Error (or ResultType.Warning if it becomes supported)
                string.Format(Noark5Messages.InternalSchemaFileIsUsed, ArkadeConstants.MetadatakatalogXsdFileName)));

            return ArkadeConstants.MetadatakatalogXsdResource;
        }

        public override string GetName()
        {
            return Noark5Messages.ValidateXmlWithSchema;
        }

        public override TestType GetTestType()
        {
            return TestType.Structure;
        }

        protected override List<TestResult> GetTestResults()
        {
            return _testResults;
        }
    }
}