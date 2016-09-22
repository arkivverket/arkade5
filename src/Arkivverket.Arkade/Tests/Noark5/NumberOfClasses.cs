using System;
using System.Xml;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    class NumberOfClasses : BaseTest
    {
        public NumberOfClasses(IArchiveContentReader archiveReader) : base(TestType.Content, archiveReader)
        {
        }

        protected override void Test(ArchiveExtraction archive)
        {
            using (var reader = XmlReader.Create(archive.GetContentDescriptionFileName()))
            {
                int counter = 0;

                while (reader.Read())
                {
                    if (reader.IsNodeTypeAndName(XmlNodeType.Element, "klasse"))
                    {
                        counter++;
                    }
                }
                Console.WriteLine("Number of classes: " + counter);
                TestSuccess($"Found {counter} classes.");
            }
        }
    }
}
