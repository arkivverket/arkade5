using CommandLine;

namespace Arkivverket.Arkade.CLI
{
    [Verb("generate", HelpText = "Generate a specified file. Run this command followed by '--help' for more detailed info.")]
    public class GenerateOptions : Options
    {
        [Option('m', "metadata-example", HelpText = "Generate example metadata file. Argument is output file name.", Required = true)]
        public string GenerateMetadataExample { get; set; }
    }
}
