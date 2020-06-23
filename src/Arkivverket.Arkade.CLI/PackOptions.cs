using CommandLine;

namespace Arkivverket.Arkade.CLI
{
    [Verb("pack", HelpText = "Pack archive data in accordance with a specified standard. Run this command followed by '--help' for more detailed info.")]
    public class PackOptions : ArchiveProcessingOptions
    {
        [Option('i', "information-package-type", HelpText = "Optional. Valid values: SIP, AIP. Default: SIP", Default = "SIP")]
        public string InformationPackageType { get; set; }

        [Option('m', "metadata-file", HelpText = "File with metadata to include in package.", Required = true)]
        public string MetadataFile { get; set; }
    }
}
