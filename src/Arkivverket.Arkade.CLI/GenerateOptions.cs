using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Arkivverket.Arkade.CLI
{
    [Verb("generate", HelpText = "Generate a specified file. Run this command followed by '--help' for more detailed info.")]
    public class GenerateOptions : Options
    {
        [Option('m', "metadata-example", HelpText = "Generate json file with example metadata. Argument is output file name.", Required = true)]
        public bool GenerateMetadataExample { get; set; }

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
            }
        }
    }
}
