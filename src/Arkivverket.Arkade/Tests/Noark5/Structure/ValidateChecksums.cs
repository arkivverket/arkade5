using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5.Structure
{
    public class ValidateChecksums : BaseTest
    {
        protected override TestResults Test(ArchiveExtraction archive)
        {
            TestResults results = new TestResults();

            addml structure = SerializeUtil.DeserializeFromFile<addml>(archive.GetStructureDescriptionFileName());

            foreach (dataObject entry in structure.dataset[0].dataObjects.dataObject)
            {
                foreach (dataObject currentObject in entry.dataObjects.dataObject)
                {
                    foreach (property fileProperty in currentObject.properties.Where(s => s.name == "file"))
                    {
                        string filename = archive.WorkingDirectory + Path.DirectorySeparatorChar + GetFilenameFromProperty(fileProperty);

                        string checksumAlgorithm = GetChecksumAlgorithmFromProperty(fileProperty);

                        string checksumValue = GetChecksumValueFromProperty(fileProperty);

                        string generatedChecksum = GenerateChecksumForFile(filename, checksumAlgorithm, checksumValue);

                        bool checksumsAreEqual = string.Equals(generatedChecksum,checksumValue, StringComparison.InvariantCultureIgnoreCase);

                        TestResult testResult = CreateTestResult(checksumsAreEqual, generatedChecksum, checksumValue, filename, checksumAlgorithm);
                        results.Add(testResult);
                    }
                }
            }
            return results;
        }

        private TestResult CreateTestResult(bool checksumsAreEqual, string generatedChecksum, string expectedChecksum, string filename, string checksumAlgorithm)
        {
            ResultType result = checksumsAreEqual ? ResultType.Success : ResultType.Error;

            string message = $"Checksum validated for file: {filename} with algorithm: {checksumAlgorithm}.";
            if (result == ResultType.Error)
            {
                message = message + $" Expected checksum: [{expectedChecksum}] Generated checksum: [{generatedChecksum}]. ";
            }
            return new TestResult(result, this.GetType().FullName, message);
        }

        private string GenerateChecksumForFile(string filename, string checksumAlgorithm, string checksumToValidateWith)
        {
            IChecksumGenerator generator = new ChecksumGeneratorFactory().GetGenerator(checksumAlgorithm);
            return generator.GenerateChecksum(filename);
        }

        private static string GetChecksumValueFromProperty(property fileProperty)
        {
            property checksumProperty = fileProperty.properties.FirstOrDefault(p => p.name == "checksum");
            property checksumValueProperty = checksumProperty?.properties.FirstOrDefault(p => p.name == "value");
            return checksumValueProperty?.value;
        }

        private static string GetChecksumAlgorithmFromProperty(property fileProperty)
        {
            property checksumProperty = fileProperty.properties.FirstOrDefault(p => p.name == "checksum");
            property checksumAlgorithmProperty = checksumProperty?.properties.FirstOrDefault(p => p.name == "algorithm");
            return checksumAlgorithmProperty?.value;
        }

        private static string GetFilenameFromProperty(property fileProperty)
        {
            property fileNameProperty = fileProperty.properties.FirstOrDefault(p => p.name == "name");
            return fileNameProperty?.value;
        }
    }
}
