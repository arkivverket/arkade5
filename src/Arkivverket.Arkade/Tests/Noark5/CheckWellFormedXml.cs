using System;
using System.IO;
using System.Xml;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Tests.Noark5
{
    public class CheckWellFormedXml : BaseTest
    {
        protected override void Test(ArchiveExtraction archive)
        {

            using(var reader = XmlReader.Create(archive.GetDescriptionFileName()))
            {
                try
                {
                    while (reader.Read()) { }
                    Console.WriteLine("Wellformed XML: Pass");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Wellformed XML: Fail: " + ex.Message);
                }
            }

        }

    }
}
