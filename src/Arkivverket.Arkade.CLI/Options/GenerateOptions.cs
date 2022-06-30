using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Arkivverket.Arkade.CLI.Options
{
    [Verb("generate", HelpText = "Generate a specified file. Run this command followed by '--help' for more detailed info.")]
    public class GenerateOptions : OutputOptions
    {
        [Option('m', "metadata-example", Group = "file-type", 
            HelpText = "Generate a metadata example file.")]
        public bool GenerateMetadataExampleFile { get; set; }

        [Option('s', "noark5-test-selection", Group = "file-type",
            HelpText = "Generate a Noark 5 test selection file.")]
        public bool GenerateNoark5TestSelectionFile { get; set; }

        [Usage(ApplicationAlias = "arkade")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Generate a metadata example file",
                    new GenerateOptions
                    {
                        OutputDirectory = "outputDirectory",
                        GenerateMetadataExampleFile = true
                    });
                yield return new Example("Generate a Noark 5 test selection file",
                    new GenerateOptions
                    {
                        OutputDirectory = "outputDirectory",
                        GenerateNoark5TestSelectionFile = true
                    });
                yield return new Example("Generate a metadata example file and a Noark 5 test selection file",
                    new GenerateOptions
                    {
                        OutputDirectory = "outputDirectory",
                        GenerateMetadataExampleFile = true,
                        GenerateNoark5TestSelectionFile = true
                    });
            }
        }
    }
}
