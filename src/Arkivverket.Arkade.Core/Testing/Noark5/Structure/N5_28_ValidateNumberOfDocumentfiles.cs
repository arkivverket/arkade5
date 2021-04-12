using System;
using System.Collections.Generic;
using System.Linq;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Base.Noark5;
using Arkivverket.Arkade.Core.ExternalModels.Addml;
using Arkivverket.Arkade.Core.Resources;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Testing.Noark5.Structure
{
    public class N5_28_ValidateNumberOfDocumentfiles : Noark5StructureBaseTest
    {
        private readonly TestId _id = new TestId(TestId.TestKind.Noark5, 28);

        private readonly List<TestResult> _testResults = new List<TestResult>();

        public override TestId GetId()
        {
            return _id;
        }

        public override TestType GetTestType()
        {
            return TestType.StructureAnalysis;
        }

        protected override List<TestResult> GetTestResults()
        {
            return _testResults;
        }

        public override void Test(Archive archive)
        {
            var actualFileCount = 0;

            var documentsDirectory = archive.GetDocumentsDirectory();

            if (!documentsDirectory.Exists)
            {
                string documentDirectoryParent = archive.WorkingDirectory.Content().DirectoryInfo().Name + "\\";

                _testResults.Add(new TestResult(ResultType.Error, new Location(documentDirectoryParent),
                    Noark5Messages.ValidateNumberOfDocumentfilesMessage_FilesDirectoryNotFound));
            }
            else
            {
                actualFileCount = archive.DocumentFiles.Count;

                if (actualFileCount > 0)
                    _testResults.Add(new TestResult(ResultType.Success,
                        new Location(documentsDirectory.Name + "\\"),
                        string.Format(Noark5Messages.ValidateNumberOfDocumentfilesMessage_NumberOfFilesFound,
                            actualFileCount)));

                else
                    _testResults.Add(new TestResult(ResultType.Error,
                        new Location(documentsDirectory.Name + "\\"),
                        Noark5Messages.ValidateNumberOfDocumentfilesMessage_NoFilesFound));
            }

            try
            {
                int documentedFileCount = GetDocumentedFileCount(archive);

                _testResults.Add(new TestResult(ResultType.Success,
                    new Location(ArkadeConstants.ArkivuttrekkXmlFileName),
                    string.Format(Noark5Messages.ValidateNumberOfDocumentfilesMessage_NumberOfFilesDocumented,
                        documentedFileCount)));


                if (documentedFileCount != actualFileCount)
                {
                    _testResults.Add(new TestResult(ResultType.Error, new Location(""),
                        Noark5Messages.ValidateNumberOfDocumentfilesMessage_FileAndDocumentationMismatch));
                }
            }
            catch (Exception)
            {
                _testResults.Add(new TestResult(ResultType.Error, new Location(ArkadeConstants.ArkivuttrekkXmlFileName),
                    Noark5Messages.ValidateNumberOfDocumentfilesMessage_DocumentationNotFound));
            }
        }

        private static int GetDocumentedFileCount(Archive archive)
        {
            var archiveExtractionXml = SerializeUtil.DeserializeFromFile<addml>(archive.AddmlXmlUnit.File);

            dataObject archiveExtractionElement = archiveExtractionXml.dataset[0].dataObjects.dataObject[0];
            property infoElement = archiveExtractionElement.properties[0];
            property additionalInfoElement = infoElement.properties[1];
            property documentCountProperty =
                additionalInfoElement.properties.FirstOrDefault(p => p.name == "antallDokumentfiler");

            return int.Parse(documentCountProperty.value);
            // Throws exception if documentCountProperty is null or value could not be parsed

            // TODO: Add "antallDokumentfiler" to ArcadeConstants?
        }
    }
}
