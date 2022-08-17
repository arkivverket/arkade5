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

        [Option('M', "metadata-example-filename",
            HelpText = "Optional. Provide a custom name for metadata example file.")]
        public string MetadataExampleFileName { get; set; }

        [Option('s', "noark5-test-selection", Group = "file-type",
            HelpText = "Generate a Noark 5 test selection file.")]
        public bool GenerateNoark5TestSelectionFile { get; set; }

        [Option('S', "noark5-test-selection-filename",
            HelpText = "Optional. Provide a custom name for noark5 test selection file.")]
        public string Noark5TestSelectionFileName { get; set; }

        [Usage(ApplicationAlias = "arkade")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Generate a metadata example file",
                    OptionsConfig.FormatStyle,
                    new GenerateOptions
                    {
                        OutputDirectory = "outputDirectory",
                        GenerateMetadataExampleFile = true,
                        MetadataExampleFileName = "my-metadata.json"

                    });
                yield return new Example("Generate a Noark 5 test selection file",
                    OptionsConfig.FormatStyle,
                    new GenerateOptions
                    {
                        OutputDirectory = "outputDirectory",
                        GenerateNoark5TestSelectionFile = true,
                        Noark5TestSelectionFileName = "my-test-selection.txt"
                    });
                yield return new Example("Generate a metadata example file and a Noark 5 test selection file",
                    OptionsConfig.FormatStyle,
                    new GenerateOptions
                    {
                        OutputDirectory = "outputDirectory",
                        GenerateMetadataExampleFile = true,
                        MetadataExampleFileName = "my-metadata.json",
                        GenerateNoark5TestSelectionFile = true,
                        Noark5TestSelectionFileName = "my-test-selection.txt"
                    });
            }
        }
    }
}
