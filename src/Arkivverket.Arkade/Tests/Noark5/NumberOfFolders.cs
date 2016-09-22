using System.Xml;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfFolders : BaseTest
    {
        public const string AnalysisKeyFolders = "Folders";

        public NumberOfFolders(IArchiveContentReader archiveReader) : base(TestType.Content, archiveReader)
        {
        }

        protected override void Test(ArchiveExtraction archive)
        {
            using (var reader = CreateXmlReaderForContentDescriptionFile(archive))
            {
                int counter = 0;
                var elementName = "mappe";
                if (reader.ReadToDescendant(elementName))
                {
                    counter++;
                    while (reader.ReadToNextSibling(elementName))
                    {
                        counter++;
                    }
                }

                AddAnalysisResult(AnalysisKeyFolders, counter.ToString());

                TestSuccess($"Found {counter} classification systems.");
            }

        }
        
    }
}