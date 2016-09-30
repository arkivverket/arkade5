using System;
using System.Xml;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfArchives : BaseTest
    {
        public const string AnalysisKeyArchives = "Archives";

        public NumberOfArchives(IArchiveContentReader archiveReader) : base(TestType.Content, archiveReader)
        {
        }

        protected override void Test(Archive archive)
        {
            using (var reader = XmlReader.Create(ArchiveReader.GetContentAsStream(archive)))
            {
                int counter = 0;
                while (reader.ReadToFollowing("arkiv"))
                {
                    counter++;
                }

                AddAnalysisResult(AnalysisKeyArchives, counter.ToString());

                TestSuccess($"Antall arkiver: {counter}.");
            }
        }

    }
}
