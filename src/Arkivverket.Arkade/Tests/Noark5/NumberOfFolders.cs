using System.IO;
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

        protected override void Test(Archive archive)
        {
            using (Stream content = ArchiveReader.GetContentAsStream(archive))
            {
                using (var reader = XmlReader.Create(content))
                {
                    int counter = 0;
                    var elementName = "mappe";
                    if (reader.ReadToDescendant(elementName))
                    {
                        counter++;
                        while (reader.ReadToFollowing(elementName))
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
}