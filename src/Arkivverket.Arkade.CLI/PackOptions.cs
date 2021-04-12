using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Arkivverket.Arkade.CLI
{
    [Verb("pack", HelpText = "Pack archive data in accordance with a specified standard. Run this command followed by '--help' for more detailed info.")]
    public class PackOptions : ArchiveProcessingOptions
    {
        [Option('i', "information-package-type", HelpText = "Optional. Valid values: SIP, AIP.", Default = "SIP")]
        public string InformationPackageType { get; set; }

        [Option('f', "document-file-format-check", HelpText = "Optional. Report document file PRONOM format information.")]
        public bool PerformFileFormatAnalysis { get; set; }

        [Option('m', "metadata-file", HelpText = "File with metadata to include in package.", Required = true)]
        public string MetadataFile { get; set; }

        [Usage(ApplicationAlias = "arkade")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Pack archive data to SIP",
                    new PackOptions
                    {
                        Archive = "noark5ArchiveDirectory",
                        ArchiveType = "noark5",
                        ProcessingArea = "processDirectory",
                        OutputDirectory = "outputDirectory",
                        MetadataFile = "metadata.json"
                    });
                yield return new Example("Pack archive data to AIP",
                    new PackOptions
                    {
                        Archive = "noark5ArchiveDirectory",
                        ArchiveType = "noark5",
                        ProcessingArea = "processDirectory",
                        OutputDirectory = "outputDirectory",
                        MetadataFile = "metadata.json",
                        InformationPackageType = "AIP"
                    });
            }
        }
    }
}
