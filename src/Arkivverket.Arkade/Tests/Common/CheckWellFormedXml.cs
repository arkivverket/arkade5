using System;
using System.Xml;

namespace Arkivverket.Arkade.Tests.Common
{
    public class CheckWellFormedXml
    {
        public void Test(string filename)
        {
            using (var reader = XmlReader.Create(filename))
            {
                try
                {
                    while (reader.Read())
                    {
                    }
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