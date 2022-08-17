using System.IO;
using System.Text;

namespace Arkivverket.Arkade.Core.Util
{
    // Based on https://stackoverflow.com/a/45228117
    internal static class StreamReaderExtensions
    {
        public static string ReadLine(this StreamReader sr, string lineDelimiter)
        {
            StringBuilder line = new StringBuilder();
            var matchIndex = 0;

            while (sr.Peek() > 0)
            {
                var nextChar = (char)sr.Read();
                line.Append(nextChar);

                if (nextChar == lineDelimiter[matchIndex])
                {
                    if (matchIndex == lineDelimiter.Length - 1)
                    {
                        return line.ToString();
                    }
                    matchIndex++;
                }
                else
                {
                    matchIndex = 0;
                    //did we mistake one of the characters as the delimiter? If so let's restart our search with this character...
                    if (nextChar == lineDelimiter[matchIndex])
                    {
                        if (matchIndex == lineDelimiter.Length - 1)
                        {
                            return line.ToString();
                        }
                        matchIndex++;
                    }
                }
            }

            return line.Length == 0
                ? null
                : line.ToString();
        }
    }
}