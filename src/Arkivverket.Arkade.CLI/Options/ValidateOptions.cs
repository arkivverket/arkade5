using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Arkivverket.Arkade.CLI.Options
{
    [Verb("validate", HelpText = "Validates the given file or directory against the specified format. Run this command followed by '--help' for more detailed info.")]
    public class ValidateOptions
    {
        [Option('i', "item", HelpText = "The file or directory to be validated", Required = true)]
        public string Item { get; set; }

        [Option('f', "format", HelpText = "The format which the file or directory is validated against. Available values: PDF/A, DIAS-SIP, DIAS-AIP, DIAS-SIP-Noark5, DIAS-AIP-Noark5", Required = true)]
        public string Format { get; set; }

        [Option('o', "output-directory", 
            HelpText = "Directory to place Arkade output files. Required if -i/--item is a directory.", 
            Required = false)]
        public string OutputDirectory { get; set; }

        [Usage(ApplicationAlias = "arkade")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Validate a specified file as PDF/A",
                    OptionsConfig.FormatStyle,
                    new ValidateOptions
                    {
                        Item = "/path/to/pdfA-file",
                        Format = "PDF/A"
                    });

                yield return new Example("Validate a specified tar or directory to fulfill the DIAS standard",
                    OptionsConfig.FormatStyle,
                    new ValidateOptions
                    {
                        Item = "/path/to/dias-tar-or-directory",
                        Format = "DIAS SIP"
                    });
            }
        }
    }
}
