using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Arkivverket.Arkade.CLI
{
    [Verb("validate-format", HelpText = "Validates the given file or directory against the specified format. Run this command followed by '--help' for more detailed info.")]
    public class ValidateOptions : ValidateBaseOptions
    {
        [Option('f', "format", HelpText = "The format which the file or directory is validated against. Available values: PDF/A, DIAS", Required = true)]
        public string Format { get; set; }

        [Usage(ApplicationAlias = "arkade")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Validate a specified file as PDF/A",
                    new ValidateOptions
                    {
                        Item = "/path/to/pdfA-file",
                        Format = "PDF/A"
                    });
            }
        }
    }

    [Verb("validate-dias", HelpText = "Validates that the given .tar or directory structurally fulfills the DIAS-specification")]
    public class ValidateDiasOptions : ValidateBaseOptions
    {
        [Option('p', "information-package-type", HelpText = "Valid values: SIP, AIP.", Required = true)]
        public string InformationPackageType { get; set; }

        [Usage(ApplicationAlias = "arkade")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Structurally validate a specified tar-file against DIAS",
                    new ValidateDiasOptions
                    {
                        Item = "/path/to/tar-archive",
                        InformationPackageType = "SIP"
                    });
            }
        }
    }

    public abstract class ValidateBaseOptions
    {
        [Option('i', "item", HelpText = "The file or directory to be validated", Required = true)]
        public string Item { get; set; }
    }
}
