using System;
using System.Collections.Generic;
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


        public NumberOfClassificationSystems() : base(TestType.Content)
        {
        }

        protected override void Test(ArchiveExtraction archive)
        {
            using (var reader = XmlReader.Create(archive.GetContentDescriptionFileName()))
            {
                int counter = 0;
                if (reader.ReadToDescendant("klassifikasjonssystem"))
                {
                    counter++;
                    while (reader.ReadToNextSibling("klassifikasjonssystem"))
                    {
                        counter++;
                    }
                }

                AddAnalysisResult(AnalysisKeyClassificationSystems, counter.ToString());

                TestSuccess($"Found {counter} classification systems.");
            }
        }

    }
}
