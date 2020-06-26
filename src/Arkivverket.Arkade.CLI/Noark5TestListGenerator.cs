using System;
using System.IO;
using System.Text;
using Arkivverket.Arkade.Core.Base;
using Arkivverket.Arkade.Core.Testing.Noark5;
using Arkivverket.Arkade.Core.Util;
using Serilog;

namespace Arkivverket.Arkade.CLI
{
    public static class Noark5TestListGenerator
    {
        public static void Generate(string outputFileName, bool allTestsEnabled = false)
        {
            const string commentSymbol = "#";
            var testList = new StringBuilder();

            testList.AppendLine($"{commentSymbol} {OutputStrings.Noark5TestListHeading}");
            testList.AppendLine($"{commentSymbol} {string.Format(OutputStrings.Noark5TestListDescription, commentSymbol)}");
            testList.AppendLine();

            foreach (TestId testId in Noark5TestProvider.GetAllTestIds())
            {
                string line = (allTestsEnabled ? string.Empty : commentSymbol) +
                              ArkadeTestNameProvider.GetDisplayName(testId);

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
