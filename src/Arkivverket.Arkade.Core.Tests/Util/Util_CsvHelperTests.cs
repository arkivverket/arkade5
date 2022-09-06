using FluentAssertions;
using Xunit;

namespace Arkivverket.Arkade.Core.Tests.Util
{
    public class Util_CsvHelperTests
    {
        [Fact]
        public void ShouldSplitStringsCorrectly()
        {
            const string recordDelimiter = "\n";
            string[] delimiters = { ";", "!=/HFA", "|", "#", ",", "-", "<=>" };
            string[] quotingStrings = { "\"", "\"\"\"", "*", "\"s", "as\\d5", "\\*+?{[()^$." };

            foreach (string d in delimiters)
            {
                foreach (string q in quotingStrings)
                {
                    string testStr = $"{q}{q}{d}" +
                                     $"{q}A{q}{q}B{q}{d}" +
                                     $"{q}C{q}{d}" +
                                     $"{q}D{q}{q}{d} asd{q}{d}" +
                                     $"{q}E{q}{q}noko{q}{q}{q}{d}" +
                                     $"F{d}" +
                                     $"{q}{q}{d}" +
                                     $"{q}{q}{q}{d}{q}{q}{q}{d}" +
                                     $"{q}{q}{q}encapsulated{d}value{q}{q} plus more{q}{d}" +
                                     $"{q}{q}" +
                                     $"{recordDelimiter}";

                    string[] splitString = Core.Util.CsvHelper.Split(testStr, recordDelimiter, d, q);

                    splitString[0].Should().Be("");
                    splitString[1].Should().Be($"A{q}B");
                    splitString[2].Should().Be("C");
                    splitString[3].Should().Be($"D{q}{d} asd");
                    splitString[4].Should().Be($"E{q}noko{q}");
                    splitString[5].Should().Be("F");
                    splitString[6].Should().Be("");
                    splitString[7].Should().Be($"{q}{d}{q}");
                    splitString[8].Should().Be($"{q}encapsulated{d}value{q} plus more");
                    splitString[9].Should().Be("");
                }
            }
        }
    }
}
