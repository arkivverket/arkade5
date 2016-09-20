using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Xml;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    class StatusOfArchiveParts : BaseTest
    {
        public StatusOfArchiveParts() : base(TestType.Content)
        {
        }


        protected override void Test(ArchiveExtraction archive)
        {
            TrackResults trackResults = new TrackResults();

            using (var reader = XmlReader.Create(archive.GetContentDescriptionFileName()))
            {
                bool isSearchingForArchiveStatus = false;

                while (reader.Read())
                {
                    if (reader.IsStartElement("arkivdel"))
                    {
                        isSearchingForArchiveStatus = true;
                    }
                    else if (reader.IsNodeTypeAndName(XmlNodeType.Element, "arkivdelstatus") && isSearchingForArchiveStatus)
                    {
                        reader.Read(); // Adance the xml reader to the content of the node
                        trackResults.Add(reader.Value);
                        isSearchingForArchiveStatus = false;
                    }
                    else if (reader.IsNodeTypeAndName(XmlNodeType.EndElement, "arkivdelstatus") && isSearchingForArchiveStatus)
                    {
                        trackResults.Add("arkivdelstatus ikke definert i arkivdel");
                        isSearchingForArchiveStatus = false;
                    }
                }
            }

            trackResults.ConsoleLogResults();
            TestSuccess($"Found {trackResults.ResultsToString()} in archive.");
        }
    }


    internal class TrackResults
    {
        private Dictionary<string, int> Results { get; set; }

        internal TrackResults()
        {
            Results = new Dictionary<string, int>();
        }

        internal void Add(string resultEntry)
        {
            if (Results.ContainsKey(resultEntry))
            {
                Results[resultEntry] += 1;
            }
            else
            {
                Results.Add(resultEntry, 1);
            }
        }

        internal void ConsoleLogResults()
        {
            foreach (var pair in Results)
            {
                Console.WriteLine($"arkivdelstatus: {pair.Key}: {pair.Value} obersvasjoner");
            }
        }


        internal string ResultsToString()
        {
            string result = String.Empty;
            foreach (var pair in Results)
            {
                result = result + $"{pair.Key} ({pair.Value}) ";
            }
            return result;
        }



    }

}
