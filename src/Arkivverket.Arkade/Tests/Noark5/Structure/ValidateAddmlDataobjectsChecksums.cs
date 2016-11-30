using System;
using System.IO;
using System.Linq;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Core.Noark5;
using Arkivverket.Arkade.ExternalModels.Addml;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5.Structure
{
    public class ValidateAddmlDataobjectsChecksums : BaseNoark5Test
    {
        public ValidateAddmlDataobjectsChecksums(IArchiveContentReader archiveReader) : base(TestType.Structure, archiveReader)
        {
        }

        protected override void Test(Archive archive)
        {
            var structure = SerializeUtil.DeserializeFromFile<addml>(archive.GetStructureDescriptionFileName());

            foreach (var entry in structure.dataset[0].dataObjects.dataObject)
            {
                foreach (var currentObject in entry.dataObjects.dataObject)
                {
                    foreach (var fileProperty in currentObject.properties.Where(s => s.name == "file"))
                    {
                        var filename = archive.WorkingDirectory.FullName + Path.DirectorySeparatorChar + GetFilenameFromProperty(fileProperty);

                        var checksumAlgorithm = GetChecksumAlgorithmFromProperty(fileProperty);

                        var checksumValue = GetChecksumValueFromProperty(fileProperty);

                        var generatedChecksum = GenerateChecksumForFile(filename, checksumAlgorithm);

                        var checksumsAreEqual = string.Equals(generatedChecksum, checksumValue, StringComparison.InvariantCultureIgnoreCase);

                        var testResult = CreateTestResult(checksumsAreEqual, generatedChecksum, checksumValue, filename, checksumAlgorithm);
                        TestResults.Add(testResult);
                    }
                }
            }
        }

        public override void OnReadStartElementEvent(object sender, ReadElementEventArgs e)
        {
        }

        private TestResult CreateTestResult(bool checksumsAreEqual, string generatedChecksum, string expectedChecksum, string filename,
            string checksumAlgorithm)
        {
            var result = checksumsAreEqual ? ResultType.Success : ResultType.Error;

            string message = $"Sjekksum er kontrollert for {filename} med algoritmen {checksumAlgorithm}.";
            if (result == ResultType.Error)
            {
                message = message + $"\nForventet sjekksum: [{expectedChecksum}]\nGenerert sjekksum: [{generatedChecksum}]. ";
            }
            return new TestResult(result, new Location(filename), message);
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

        private static string GetFilenameFromProperty(property fileProperty)
        {
            var fileNameProperty = fileProperty.properties.FirstOrDefault(p => p.name == "name");
            return fileNameProperty?.value;
        }
    }
}