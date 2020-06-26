using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace Arkivverket.Arkade.CLI
{
    [Verb("process", HelpText = "Process archive data in accordance with specified standard. Run this command followed by '--help' for more detailed info.")]
    public class ProcessOptions : ArchiveProcessingOptions
    {
        [Option('i', "information-package-type", HelpText = "Optional. Valid values: SIP, AIP.", Default = "SIP")]
        public string InformationPackageType { get; set; }

        [Option('m', "metadata-file", HelpText = "File with metadata to include in package.", Required = true)]
        public string MetadataFile { get; set; }

        [Usage(ApplicationAlias = "arkade")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Process archive - run all tests and pack to SIP",
                    new ProcessOptions
                    {
                        Archive = "noark5ArchiveDirectory",
                        ArchiveType = "noark5",
                        ProcessingArea = "processDirectory",
                        OutputDirectory = "outputDirectory",
                        MetadataFile = "metadata.json"

                    });
                yield return new Example("Process archive - run all tests and pack to AIP",
                    new ProcessOptions
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
