using System;
using System.Xml;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class NumberOfArchives : BaseTest
    {
        public NumberOfArchives() : base(TestType.Content)
        {
        }

        protected override void Test(ArchiveExtraction archive)
        {
            using (var reader = XmlReader.Create(archive.GetContentDescriptionFileName()))
            {
                int counter = 0;
                while (reader.ReadToNextSibling("arkiv"))
                {
                    counter++;
                }
                Console.WriteLine("Number of archives: " + counter);
            }
        }

    }
}
