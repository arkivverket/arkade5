using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Arkivverket.Arkade.CLI.Options
{
    [Verb("test", HelpText = "Test archive data in accordance with a specified standard. Run this command followed by '--help' for more detailed info.")]
    public class TestOptions : ArchiveProcessingOptions
    {
        [Option('s', "noark5-test-selection", HelpText = "Optional. Selection of Noark 5 tests to be run. Omit to run all tests.")]
        public string TestSelectionFile { get; set; }

        [Option('r', "result-display-limit", HelpText = "Optional. Set maximum amount of test results to be displayed per test point.", Default = 100)]
        public int TestResultDisplayLimit { get; set; }

        [Usage(ApplicationAlias = "arkade")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Run all tests on archive data",
                    OptionsConfig.FormatStyle,
                    new TestOptions
                    {
                        Archive = "noark5ArchiveDirectory",
                        ArchiveType = "noark5",
                        ProcessingArea = "processDirectory",
                        OutputDirectory = "outputDirectory",

                    });
            }
        }
    }
}
