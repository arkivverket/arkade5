using System.Xml;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfClasses : BaseTest
    {
        public const string AnalysisKeyClasses = "Classes";

        public NumberOfClasses(IArchiveContentReader archiveReader) : base(TestType.Content, archiveReader)
        {
        }

        protected override void Test(ArchiveExtraction archive)
        {
            using (var reader = XmlReader.Create(ArchiveReader.GetContentAsStream(archive)))
            {
                var counter = 0;

                while (reader.Read())
                {
                    if (reader.IsNodeTypeAndName(XmlNodeType.Element, "klasse"))
                    {
                        counter++;
                    }
                }

                AddAnalysisResult(AnalysisKeyClasses, counter.ToString());

                TestSuccess($"Found {counter} classes.");
            }
        }
    }
}