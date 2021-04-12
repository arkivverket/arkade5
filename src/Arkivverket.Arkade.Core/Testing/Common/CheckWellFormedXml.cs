using System.Xml;

namespace Arkivverket.Arkade.Core.Testing.Common
{
    public class CheckWellFormedXml
    {
        public void Test(string filename)
        {
            using (var reader = XmlReader.Create(filename))
            {
                while (reader.Read())
                {
                }
            }
        }
    }
}