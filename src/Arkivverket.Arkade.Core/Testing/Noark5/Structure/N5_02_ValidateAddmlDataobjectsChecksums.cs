using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5.Structure
{
    public class N5_02_ValidateAddmlDataobjectsChecksums : Noark5StructureBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 2);

        private readonly List<TestResult> _testResults = new List<TestResult>();

        public override void Test(Archive archive)
        {
            var structure = SerializeUtil.DeserializeFromFile<addml>(archive.AddmlXmlUnit.File);

            foreach (var entry in structure.dataset[0].dataObjects.dataObject)
            {
                foreach (var currentObject in entry.dataObjects.dataObject)
                {
                    foreach (var fileProperty in currentObject.properties.Where(s => s.name == "file"))
                    {
                        string fileName = GetFileNameFromProperty(fileProperty);
                        var fullPathToFile = Path.Combine(archive.WorkingDirectory.Content().DirectoryInfo().FullName, fileName);

                        var checksumAlgorithm = GetChecksumAlgorithmFromProperty(fileProperty);

                        var checksumValue = GetChecksumValueFromProperty(fileProperty);

                        var generatedChecksum = GenerateChecksumForFile(fullPathToFile, checksumAlgorithm);

                        var checksumsAreEqual = string.Equals(generatedChecksum, checksumValue, StringComparison.InvariantCultureIgnoreCase);

                        var testResult = CreateTestResult(checksumsAreEqual, generatedChecksum, checksumValue, fileName, checksumAlgorithm);
                        _testResults.Add(testResult);
                    }


                    foreach (var schema in currentObject.properties.Where(s => s.name == "schema"))
                    {
                        foreach (var fileProperty in schema.properties.Where(s => s.name == "file")) {

                        string fileName = GetFileNameFromProperty(fileProperty);
                        var fullPathToFile = Path.Combine(archive.WorkingDirectory.Content().DirectoryInfo().FullName, fileName);

                        var checksumAlgorithm = GetChecksumAlgorithmFromProperty(fileProperty);

                            if (!string.IsNullOrEmpty(checksumAlgorithm))
                            { 

                            var checksumValue = GetChecksumValueFromProperty(fileProperty);

                            var generatedChecksum = GenerateChecksumForFile(fullPathToFile, checksumAlgorithm);

                            var checksumsAreEqual = string.Equals(generatedChecksum, checksumValue, StringComparison.InvariantCultureIgnoreCase);

                            var testResult = CreateTestResult(checksumsAreEqual, generatedChecksum, checksumValue, fileName, checksumAlgorithm);
                            _testResults.Add(testResult);
                            }
                        }
                    }
                }
            }
        }

        private TestResult CreateTestResult(bool checksumsAreEqual, string generatedChecksum, string expectedChecksum, string fileName,
            string checksumAlgorithm)
        {
            if (!checksumsAreEqual)
            {
                var message = string.Format(Noark5Messages.ExceptionInvalidChecksum, fileName,
                    expectedChecksum.ToLower(), generatedChecksum.ToLower());
                return new TestResult(ResultType.Error, new Location(fileName),
                    $"{message}.\n " + string.Format(Noark5Messages.ChecksumAlgorithmMessage, checksumAlgorithm));
            }

            return new TestResult(ResultType.Success, new Location(fileName),
                string.Format(Noark5Messages.ChecksumAlgorithmMessage, checksumAlgorithm)
            );
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