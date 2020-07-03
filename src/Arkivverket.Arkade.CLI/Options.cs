using CommandLine;

namespace Arkivverket.Arkade.CLI
{
    public abstract class Options
    {
        [Option('o', "output-directory", HelpText = "Directory to place processing results.", Required = true)]
        public string OutputDirectory { get; set; }
    }
}
