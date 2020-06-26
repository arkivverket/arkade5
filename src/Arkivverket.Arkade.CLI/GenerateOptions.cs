using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Arkivverket.Arkade.CLI
{
    [Verb("generate", HelpText = "Generate a specified file. Run this command followed by '--help' for more detailed info.")]
    public class GenerateOptions : Options
    {
        [Option('m', "metadata-example", Group = "file-type", 
            HelpText = "Generate json file with example metadata. Argument is output file name.")]
        public bool GenerateMetadataExample { get; set; }

        [Option('l', "noark5-test-list", Group = "file-type",
            HelpText = "Generate text file with list of noark5 tests. Argument is output file name.")]
        public bool GenerateNoark5TestList { get; set; }

        [Usage(ApplicationAlias = "arkade")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Generate json file with metadata example",
                    new GenerateOptions
                    {
                        ProcessingArea = "processDirectory",
                        GenerateMetadataExample = true
                    });
                yield return new Example("Generate text file with list of noark5-test",
                    new GenerateOptions
                    {
                        ProcessingArea = "processDirectory",
                        GenerateNoark5TestList = true
                    });
                yield return new Example("Generate both files",
                    new GenerateOptions
                    {
                        ProcessingArea = "processDirectory",
                        GenerateMetadataExample = true,
                        GenerateNoark5TestList = true
                    });
            }
        }
    }
}
