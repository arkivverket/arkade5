using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.CLI
{
    public static class Noark5TestSelectionFileReader
    {
        public static List<TestId> GetUserSelectedTestIds(string filePath)
        {
            return ParseFileContent(File.ReadAllLines(filePath));
        }

        public static List<TestId> ParseFileContent(string[] content)
        {
            var list = new List<TestId>();
            foreach (string line in content)
            {
                if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
                    continue;

                Match lineMatch = Regex.Match(line, @"N5\.\d{2}");
                if (lineMatch.Success)
                {
                    list.Add(TestId.Create(lineMatch.Value));
                }
            }

            return list;
        }
    }
}
