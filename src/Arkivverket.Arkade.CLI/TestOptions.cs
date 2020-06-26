using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Arkivverket.Arkade.CLI
{
    [Verb("test", HelpText = "Test archive data in accordance with a specified standard. Run this command followed by '--help' for more detailed info.")]
    public class TestOptions : ArchiveProcessingOptions
    {
        [Option('l', "noark5-test-list", HelpText = "Optional. List of noark5 tests to be run. Omit to run all tests.")]
        public string TestListFile { get; set; }

        [Usage(ApplicationAlias = "arkade")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Run all tests on archive data",
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
