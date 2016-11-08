using System;
using System.Collections.Generic;
using System.Xml;
using Arkivverket.Arkade.Core;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Tests.Noark5
{
    internal class StatusOfArchiveParts : BaseTest
    {
        public StatusOfArchiveParts(IArchiveContentReader archiveReader) : base(TestType.Content, archiveReader)
        {
        }

        protected override void Test(Archive archive)
        {
            var trackResults = new TrackResults();

            using (var reader = XmlReader.Create(archive.GetContentDescriptionFileName()))
            {
                var isSearchingForArchiveStatus = false;

                while (reader.Read())
                {
                    if (reader.IsStartElement("arkivdel"))
                    {
                        isSearchingForArchiveStatus = true;
                    }
                    else if (reader.IsNodeTypeAndName(XmlNodeType.Element, "arkivdelstatus") && isSearchingForArchiveStatus)
                    {
                        reader.Read(); // Advance the xml reader to the content of the node
                        trackResults.Add(reader.Value);
                        isSearchingForArchiveStatus = false;
                    }
                    else if (reader.IsNodeTypeAndName(XmlNodeType.EndElement, "arkivdelstatus") && isSearchingForArchiveStatus)
                    {
                        trackResults.Add("ikke angitt");
                        isSearchingForArchiveStatus = false;
                    }
                }
            }

            TestSuccess(new Location(archive.Uuid.GetValue()), $"Arkivdelenes status og antall: \n{trackResults.ResultsToString()}");
        }
    }


    internal class TrackResults
    {
        private Dictionary<string, int> Results { get; }

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

        internal string ResultsToString()
        {
            var result = string.Empty;
            foreach (var pair in Results)
            {
                result = result + $"{pair.Key} ({pair.Value})\n";
            }
            return result;
        }
    }
}