using CommandLine;

namespace Arkivverket.Arkade.CLI
{
    [Verb("test", HelpText = "Test archive data in accordance with a specified standard. Run this command followed by '--help' for more detailed info.")]
    public class TestOptions : ArchiveProcessingOptions
    {
        //All Options inherited
    }
}
