using CommandLine;

namespace Arkivverket.Arkade.CLI
{
    [Verb("process", HelpText = "Process archive data in accordance with specified standard. Run this command followed by '--help' for more detailed info.")]
    public class ProcessOptions : ArchiveProcessingOptions
    {
        [Option('i', "information-package-type", HelpText = "Optional. Valid values: SIP, AIP. Default: SIP", Default = "SIP")]
        public string InformationPackageType { get; set; }

        [Option('m', "metadata-file", HelpText = "File with metadata to include in package.", Required = true)]
        public string MetadataFile { get; set; }
    }
}
