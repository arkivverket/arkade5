using CommandLine;

namespace Arkivverket.Arkade.CLI
{
    public abstract class Options
    {
        [Option('p', "processing-area", HelpText = "Directory to place temporary files and logs.", Required = true)]
        public string ProcessingArea { get; set; }
    }
}
