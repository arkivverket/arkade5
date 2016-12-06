using System;
using System.Collections.Generic;
using System.IO;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5.Structure
{
    /// <summary>
    ///     Validates that the XML is valid with regards to the XML schema. In this case the ADDML schema.
    /// </summary>
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
            Stream addmlXsd = ResourceUtil.GetResourceAsStream(ArkadeConstants.AddmlXsdResource);

            try
            {
                XmlUtil.Validate(_archiveReader.GetStructureContentAsStream(archive), addmlXsd);

                _testResults.Add(new TestResult(ResultType.Success, new Location(archive.GetStructureDescriptionFileName()),
                    $"Filen {archive.GetStructureDescriptionFileName()} er validert i henhold ADDML XML-skjema."));
            }
            catch (Exception e)
            {
                var message = string.Format(Noark5Messages.ExceptionXmlDoesNotValidateWithSchema, 
                    Path.GetFileName(archive.GetStructureDescriptionFileName()), ArkadeConstants.AddmlXsdFileName, e.Message);
                throw new ArkadeException(message, e);
            }
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