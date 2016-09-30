using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests.Noark5
{
    /// <summary>
    /// Noark5 - test #4
    /// </summary>
    public class NumberOfClassificationSystems : BaseTest
    {
        public const string AnalysisKeyClassificationSystems = "ClassificationSystems";


        public NumberOfClassificationSystems(IArchiveContentReader archiveReader) : base(TestType.Content, archiveReader)
        {
        }

        protected override void Test(Archive archive)
        {
            using (Stream content = ArchiveReader.GetContentAsStream(archive))
            {
                using (var reader = XmlReader.Create(content))
                {
                    int counter = 0;
                    if (reader.ReadToDescendant("klassifikasjonssystem"))
                    {
                        counter++;
                        while (reader.ReadToFollowing("klassifikasjonssystem"))
                        {
                            counter++;
                        }
                    }

                    AddAnalysisResult(AnalysisKeyClassificationSystems, counter.ToString());

                    TestSuccess($"Antall klassifikasjonssystemer: {counter}.");
                }
            }
        }
    }
}
