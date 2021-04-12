using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Arkivverket.Arkade.CLI
{
    [Verb("generate", HelpText = "Generate a specified file. Run this command followed by '--help' for more detailed info.")]
    public class GenerateOptions : Options
    {
        [Option('m', "metadata-example", Group = "file-type", 
            HelpText = "Generate json file with example metadata.")]
        public bool GenerateMetadataExample { get; set; }

        [Option('s', "noark5-test-selection", Group = "file-type",
            HelpText = "Generate text file with list of noark5 tests.")]
        public bool GenerateNoark5TestSelectionFile { get; set; }

        [Usage(ApplicationAlias = "arkade")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Generate json file with metadata example",
                    new GenerateOptions
                    {
                        OutputDirectory = "outputDirectory",
                        GenerateMetadataExample = true
                    });
                yield return new Example("Generate text file with list of noark5-test",
                    new GenerateOptions
                    {
                        OutputDirectory = "outputDirectory",
                        GenerateNoark5TestSelectionFile = true
                    });
                yield return new Example("Generate both files",
                    new GenerateOptions
                    {
                        OutputDirectory = "outputDirectory",
                        GenerateMetadataExample = true,
                        GenerateNoark5TestSelectionFile = true
                    });
            }
        }
    }
}
