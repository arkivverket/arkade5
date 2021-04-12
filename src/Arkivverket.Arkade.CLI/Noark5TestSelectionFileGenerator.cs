using System;
using System.Globalization;
using System.IO;
using System.Text;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Languages;
using Arkivverket.Arkade.Core.Testing.Noark5;
using Arkivverket.Arkade.Core.Util;
using Serilog;

namespace Arkivverket.Arkade.CLI
{
    public static class Noark5TestSelectionFileGenerator
    {
        public static void Generate(string outputFileName, SupportedLanguage language, bool allTestsEnabled = false)
        {
            const string commentSymbol = "#";
            var testList = new StringBuilder();

            testList.AppendLine($"{commentSymbol} {OutputStrings.Noark5TestSelectionFileHeading}");
            testList.AppendLine($"{commentSymbol} {string.Format(OutputStrings.Noark5TestSelectionFileDescription, commentSymbol)}");
            testList.AppendLine();

            foreach (TestId testId in Noark5TestProvider.GetAllTestIds())
            {
                string line = (allTestsEnabled ? string.Empty : commentSymbol) +
                              ArkadeTestNameProvider.GetDisplayName(testId, language);

                testList.AppendLine(line);
            }

            WriteTestListToFile(outputFileName, testList.ToString());
        }

        private static void WriteTestListToFile(string outputFileName, string testList)
        {
            try
            {
                var destination = new DirectoryInfo(outputFileName);
                File.WriteAllText(destination.FullName, testList);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Could not write file {outputFileName}: {e.Message}");
            }
        }
    }
}
