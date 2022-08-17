using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5.Structure
{
    public class N5_03_ValidateXmlWithSchema : Noark5StructureBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 3);

        private readonly List<TestResult> _testResults = new List<TestResult>();

        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.StructureControl;
        }

        protected override TestResultSet GetTestResults()
        {
            return new()
            {
                TestsResults = _testResults
            };
        }

        public override void Test(Archive archive)
        {
            foreach (ArchiveXmlUnit xmlUnit in archive.XmlUnits)
            {
                if (xmlUnit.AllFilesExists())
                    Validate(xmlUnit);
                else
                    ReportMissingFiles(xmlUnit);
            }
        }

        private void Validate(ArchiveXmlUnit archiveXmlUnit)
        {
            ReportFallbackOnBuiltInSchemas(archiveXmlUnit);

            string fileName = GetFileNameForReport(archiveXmlUnit);

            Dictionary<string, List<long>> validationErrorMessages;

            try
            {
                validationErrorMessages = new XmlValidator().Validate(archiveXmlUnit);
            }
            catch (Exception exception)
            {
                throw new ArkadeException(
                    string.Format(Noark5Messages.ExceptionDuringXmlValidation, fileName, exception.Message),
                    exception
                );
            }

            foreach ((string errorMessage, List<long> errorLocations) in validationErrorMessages)
                _testResults.Add(new TestResult(ResultType.Error, new Location(fileName, errorLocations), errorMessage));
        }

        private void ReportFallbackOnBuiltInSchemas(ArchiveXmlUnit archiveXmlUnit)
        {
            foreach (ArchiveXmlSchema schema in archiveXmlUnit.Schemas)
                if (schema.IsArkadeBuiltIn())
                    _testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                        // TODO: Consider implementing and using ResultType.Warning
                        string.Format(Noark5Messages.InternalSchemaFileIsUsed, schema.FileName,
                            (schema as ArkadeBuiltInXmlSchema).GetArchiveTypeVersion())));
        }

        private static string GetFileNameForReport(ArchiveXmlUnit archiveXmlUnit)
        {
            return archiveXmlUnit.File.Name.Equals(ArkadeConstants.AddmlXmlFileName)
                ? ArkadeConstants.ArkivuttrekkXmlFileName
                : archiveXmlUnit.File.Name;
        }

        private void ReportMissingFiles(ArchiveXmlUnit xmlUnit)
        {
            foreach (string missingFile in xmlUnit.GetMissingFiles())
                _testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                    string.Format(ExceptionMessages.FileNotFound, missingFile)));
        }
    }
}
