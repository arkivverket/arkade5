using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Archives;
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

        public override void Test(Noark5Archive archive)
        {
            foreach (ArchiveXmlUnit xmlUnit in archive.XmlUnits)
            {
                Validate(xmlUnit);
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
                if (schema is ArkadeBuiltInXmlSchema builtInSchema)
                    _testResults.Add(new TestResult(ResultType.Error, new Location(string.Empty),
                        // TODO: Consider implementing and using ResultType.Warning
                        string.Format(Noark5Messages.InternalSchemaFileIsUsed, builtInSchema.FileName, builtInSchema.SchemaVersion.Name)));
        }

        private static string GetFileNameForReport(ArchiveXmlUnit archiveXmlUnit)
        {
            return archiveXmlUnit.File.Name.Equals(ArkadeConstants.AddmlXmlFileName)
                ? ArkadeConstants.ArkivuttrekkXmlFileName
                : archiveXmlUnit.File.Name;
        }
    }
}
