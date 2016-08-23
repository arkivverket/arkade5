using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Arkivverket.Arkade.Tests.Noark5;

namespace Arkivverket.Arkade.Core
{
    public class TestEngine
    {





        public void LoadArchive(ArchiveExtraction archiveExtraction)
        {
            if (archiveExtraction.ArchiveType == ArchiveType.Noark5)
            {
                string filename = $"{archiveExtraction.WorkingDirectory}{Path.DirectorySeparatorChar}arkivstruktur.xml";

               // new NumberOfArchives().RunTest(archiveExtraction);
                new NumberOfArchiveParts().RunTest(archiveExtraction);
                /*
                var archives =
                    from el in SimpleStreamAxis(filename, "arkivdel")
                    select el;

                foreach (var str in archives)
                {
                    Console.WriteLine(str);
                }
                */

                //Parse(filename);
            }
        }


        private void Parse(string inputUrl)
        {
            using (var reader = XmlReader.Create(inputUrl))
            {
                int counter = 0;
                while (reader.ReadToNextSibling("arkiv"))
                {
                    counter++;
                }
                Console.WriteLine("Number of archives: " + counter);
            }
        }



        private static IEnumerable<string> SimpleStreamAxis(string inputUrl, string matchName)
        {
            using (var reader = XmlReader.Create(inputUrl))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == matchName)
                            {
                                var title = GetTitleForArkivdel(reader.ReadSubtree());
                                if (title != null)
                                    yield return title;
                                /*
                                var matchingElement = XNode.ReadFrom(reader) as XElement;
                                if (matchingElement != null)
                                    yield return matchingElement;
                                    */
                            }
                            break;
                    }
                }
                reader.Close();
            }
        }

        private static string GetTitleForArkivdel(XmlReader reader)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "tittel")
                        {
                            return reader.Value;
                        }
                        break;
                }
            }

            return null;
        }
    }
}