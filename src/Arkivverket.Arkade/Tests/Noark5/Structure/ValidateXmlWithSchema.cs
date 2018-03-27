using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
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
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 3);

        private readonly IArchiveContentReader _archiveReader;
        private readonly List<TestResult> _testResults = new List<TestResult>();

        public ValidateXmlWithSchema(IArchiveContentReader archiveReader)
        {
            _archiveReader = archiveReader;
        }

        public override void Test(Archive archive)
        {
            ValidateXml(archive.GetStructureDescriptionFileName(), _archiveReader.GetStructureContentAsStream(archive),
                GetStructureDescriptionXmlSchemaStream(archive));

            ValidateXml(archive.GetContentDescriptionFileName(), _archiveReader.GetContentAsStream(archive),
                GetContentDescriptionXmlSchemaStream(archive), GetMetadataCatalogXmlSchemaStream(archive));
        }

        private void ValidateXml(string fullPathToFile, Stream fileStream, params Stream[] xsdResources)
        {
            string fileName = Path.GetFileName(fullPathToFile);

            // Use the Noark 5 archive filename for testresults:
            if (fileName.Equals(ArkadeConstants.AddmlXmlFileName)) 
                fileName = ArkadeConstants.ArkivuttrekkXmlFileName;

            try
            {
                List<string> validationErrorMessages = new XmlValidator().Validate(fileStream, xsdResources);
                if (validationErrorMessages.Any())
                    foreach (string validationErrorMessage in validationErrorMessages)
                    {
                        _testResults.Add(new TestResult(ResultType.Error, new Location(fileName),
                            string.Format(Noark5Messages.XmlDoesNotValidateWithSchema,
                                fileName, validationErrorMessage)));
                    }
                else
                    _testResults.Add(new TestResult(ResultType.Success, new Location(fileName),
                        Noark5Messages.ValidateXmlWithSchemaMessageValid));
            }
            catch (Exception e)
            {
                string message = string.Format(Noark5Messages.ExceptionDuringXmlValidation, fileName, e.Message);
                throw new ArkadeException(message, e);
            }
        }

        private Stream GetStructureDescriptionXmlSchemaStream(Archive archive)
        {
            if (archive.HasStructureDescriptionXmlSchema())
                return _archiveReader.GetStructureDescriptionXmlSchemaAsStream(archive);

            // Fallback on internal addml.xsd:

            _testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                // TODO: Consider ResultType.Warning (if it becomes supported)
                string.Format(Noark5Messages.InternalSchemaFileIsUsed, ArkadeConstants.AddmlXsdFileName)));

            return ResourceUtil.GetResourceAsStream(ArkadeConstants.AddmlXsdResource);
        }

        private Stream GetContentDescriptionXmlSchemaStream(Archive archive)
        {
            if (archive.HasContentDescriptionXmlSchema())
                return _archiveReader.GetContentDescriptionXmlSchemaAsStream(archive);

            // Fallback on internal arkivstruktur.xsd:

            _testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                // TODO: Consider ResultType.Warning (if it becomes supported)
                string.Format(Noark5Messages.InternalSchemaFileIsUsed, ArkadeConstants.ArkivstrukturXsdFileName)));

            return ResourceUtil.GetResourceAsStream(ArkadeConstants.ArkivstrukturXsdResource);
        }

        private Stream GetMetadataCatalogXmlSchemaStream(Archive archive)
        {
            if (archive.HasMetadataCatalogXmlSchema())
                return _archiveReader.GetMetadataCatalogXmlSchemaAsStream(archive);

            // Fallback on internal metadatakatalog.xsd:

            _testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                // TODO: Consider ResultType.Warning (if it becomes supported)
                string.Format(Noark5Messages.InternalSchemaFileIsUsed, ArkadeConstants.MetadatakatalogXsdFileName)));

            return ResourceUtil.GetResourceAsStream(ArkadeConstants.MetadatakatalogXsdResource);
        }

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5Messages.ValidateXmlWithSchema;
        }

        public override TestType GetTestType()
        {
            return TestType.StructureControl;
        }

        protected override List<TestResult> GetTestResults()
        {
            return _testResults;
        }
    }
}