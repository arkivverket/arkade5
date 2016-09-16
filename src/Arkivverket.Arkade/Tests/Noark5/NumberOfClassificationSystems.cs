using System;
using System.Xml;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfClassificationSystems : BaseTest
    {
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
                Console.WriteLine("Number of classifications systems: " + counter);
                TestSuccess($"Found {counter} classification systems.");
            }

        }
    }
}
