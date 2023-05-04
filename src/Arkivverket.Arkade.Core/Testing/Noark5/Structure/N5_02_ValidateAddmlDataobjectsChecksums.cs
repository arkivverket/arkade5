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

        private readonly HashSet<string> _validatedSchemas = new();

        public override void Test(Archive archive)
        {
            addml structure = archive.AddmlInfo.Addml;

            string basePath = archive.WorkingDirectory.Content().DirectoryInfo().FullName;

            foreach (var entry in structure.dataset[0].dataObjects.dataObject)
            {
                foreach (var currentObject in entry.dataObjects.dataObject)
                {
                    foreach (var fileProperty in currentObject.properties.Where(s => s.name == "file"))
                    {
                        string fileName = GetFileNameFromProperty(fileProperty);
                        PerformTestOnFile(fileProperty, basePath, fileName);
                    }

                    foreach (var schema in currentObject.properties.Where(s => s.name == "schema"))
                    {
                        foreach (var fileProperty in schema.properties.Where(s => s.name == "file"))
                        {
                            string fileName = GetFileNameFromProperty(fileProperty);
                            if (!string.IsNullOrEmpty(fileName) && _validatedSchemas.Contains(fileName))
                                continue;

                            PerformTestOnFile(fileProperty, basePath, fileName);
                            _validatedSchemas.Add(GetFileNameFromProperty(fileProperty));
                        }
                    }
                }
            }
        }

        private void PerformTestOnFile(property fileProperty, string basePath, string fileName)
        {
            string fullPathToFile = Path.Combine(basePath, fileName);

            property checksumProperty = GetChecksumPropertyFromFileProperty(fileProperty);

            string resultMessage;
            if (checksumProperty == null)
            {
                resultMessage = string.Format(Noark5Messages.ChecksumPropertyMissing, fileName);
                _testResults.Add(CreateTestResult(resultMessage, ResultType.Error));
                return;
            }

            string checksumAlgorithm = GetChecksumAlgorithmFromProperty(fileProperty);

            if (string.IsNullOrEmpty(checksumAlgorithm))
            {
                resultMessage = string.Format(Noark5Messages.ChecksumAlgorithmMissing, fileName);
                _testResults.Add(CreateTestResult(resultMessage, ResultType.Error));
                return;
            }

            if (checksumAlgorithm is not ("SHA-256" or "SHA256"))
            {
                resultMessage = string.Format(Noark5Messages.UnsupportedChecksumAlgorithm, checksumAlgorithm, fileName);
                _testResults.Add(CreateTestResult(resultMessage, ResultType.Error));
                return;
            }

            string expectedChecksum = GetChecksumValueFromProperty(fileProperty);

            if (!File.Exists(fullPathToFile))
            {
                resultMessage = string.Format(Noark5Messages.FileNotFound, fileName);
                _testResults.Add(CreateTestResult(resultMessage, ResultType.Error));
                return;
            }

            string generatedChecksum = GenerateChecksumForFile(fullPathToFile, checksumAlgorithm);

            bool checksumsAreEqual = string.Equals(generatedChecksum, expectedChecksum, StringComparison.InvariantCultureIgnoreCase);

            string checksumAlgorithmMessage = string.Format(Noark5Messages.ChecksumAlgorithmMessage, checksumAlgorithm);

            if (!checksumsAreEqual)
            {
                resultMessage = string.Format(Noark5Messages.ExceptionInvalidChecksum, fileName,
                    expectedChecksum?.ToLower(), generatedChecksum?.ToLower());
                resultMessage = $"{resultMessage}.\n {checksumAlgorithmMessage}";
                _testResults.Add(CreateTestResult(resultMessage, ResultType.Error));
            }
            else
            {
                resultMessage = checksumAlgorithmMessage;
                _testResults.Add(CreateTestResult(resultMessage, ResultType.Success));
            }
        }

        private TestResult CreateTestResult(string resultMessage, ResultType resultType)
        {
            return new TestResult(resultType, new Location(ArkadeConstants.ArkivuttrekkXmlFileName), resultMessage);
        }

        private string GenerateChecksumForFile(string filename, string checksumAlgorithm)
        {
            var generator = new ChecksumGeneratorFactory().GetGenerator(checksumAlgorithm);
            return generator.GenerateChecksum(filename);
        }

        private static property? GetChecksumPropertyFromFileProperty(property fileProperty)
        {
            return fileProperty.properties.FirstOrDefault(p => p.name == "checksum");
        }

        private static string? GetChecksumValueFromProperty(property fileProperty)
        {
            var checksumProperty = fileProperty.properties?.FirstOrDefault(p => p.name == "checksum");
            var checksumValueProperty = checksumProperty?.properties.FirstOrDefault(p => p.name == "value");
            return checksumValueProperty?.value;
        }

        private static string? GetChecksumAlgorithmFromProperty(property fileProperty)
        {
            var checksumProperty = fileProperty.properties.FirstOrDefault(p => p.name == "checksum");
            var checksumAlgorithmProperty = checksumProperty?.properties?.FirstOrDefault(p => p.name == "algorithm");
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

        protected override TestResultSet GetTestResults()
        {
            return new()
            {
                TestsResults = _testResults
            };
        }
    }
}