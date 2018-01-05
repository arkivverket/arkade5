using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Resources;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5.Structure
{
    public class ValidateAddmlDataobjectsChecksums : Noark5StructureBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 2);

        private readonly List<TestResult> _testResults = new List<TestResult>();

        public override void Test(Archive archive)
        {
            var structure = SerializeUtil.DeserializeFromFile<addml>(archive.GetStructureDescriptionFileName());

            foreach (var entry in structure.dataset[0].dataObjects.dataObject)
            {
                foreach (var currentObject in entry.dataObjects.dataObject)
                {
                    foreach (var fileProperty in currentObject.properties.Where(s => s.name == "file"))
                    {
                        string fileName = GetFileNameFromProperty(fileProperty);
                        var fullPathToFile = archive.WorkingDirectory.Content().DirectoryInfo().FullName + Path.DirectorySeparatorChar + fileName;

                        var checksumAlgorithm = GetChecksumAlgorithmFromProperty(fileProperty);

                        var checksumValue = GetChecksumValueFromProperty(fileProperty);

                        var generatedChecksum = GenerateChecksumForFile(fullPathToFile, checksumAlgorithm);

                        var checksumsAreEqual = string.Equals(generatedChecksum, checksumValue, StringComparison.InvariantCultureIgnoreCase);

                        var testResult = CreateTestResult(checksumsAreEqual, generatedChecksum, checksumValue, fileName, checksumAlgorithm);
                        _testResults.Add(testResult);
                    }
                }
            }
        }

        private TestResult CreateTestResult(bool checksumsAreEqual, string generatedChecksum, string expectedChecksum, string fileName,
            string checksumAlgorithm)
        {
            if (!checksumsAreEqual)
            {
                var message = string.Format(Noark5Messages.ExceptionInvalidChecksum, fileName, expectedChecksum, generatedChecksum);
                throw new ArkadeException(message);
            }
            return new TestResult(ResultType.Success, new Location(fileName), $"Sjekksum er kontrollert med algoritmen {checksumAlgorithm}.");
        }

        private string GenerateChecksumForFile(string filename, string checksumAlgorithm)
        {
            var generator = new ChecksumGeneratorFactory().GetGenerator(checksumAlgorithm);
            return generator.GenerateChecksum(filename);
        }

        private static string GetChecksumValueFromProperty(property fileProperty)
        {
            var checksumProperty = fileProperty.properties.FirstOrDefault(p => p.name == "checksum");
            var checksumValueProperty = checksumProperty?.properties.FirstOrDefault(p => p.name == "value");
            return checksumValueProperty?.value;
        }

        private static string GetChecksumAlgorithmFromProperty(property fileProperty)
        {
            var checksumProperty = fileProperty.properties.FirstOrDefault(p => p.name == "checksum");
            var checksumAlgorithmProperty = checksumProperty?.properties.FirstOrDefault(p => p.name == "algorithm");
            return checksumAlgorithmProperty?.value;
        }

        private static string GetFileNameFromProperty(property fileProperty)
        {
            var fileNameProperty = fileProperty.properties.FirstOrDefault(p => p.name == "name");
            return fileNameProperty?.value;
        }

        public override TestId GetId()
        {
            return _id;
        }

        public override string GetName()
        {
            return Noark5Messages.ValidateAddmlDataobjectsChecksums;
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